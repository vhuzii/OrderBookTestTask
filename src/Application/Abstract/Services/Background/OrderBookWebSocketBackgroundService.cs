using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderBookTestTask.Application.Dtos;
using OrderBookTestTask.Application.Hubs;
using OrderBookTestTask.Application.Interfaces;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace OrderBookTestTask.Application.Abstract.Services.Background;

public abstract class OrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext,
    IOrderBookService orderBookService, IOrderBookWebSockerService orderBookWebSockerService, ILogger<OrderBookWebSocketBackgroundService> logger) 
        : BackgroundService 
{
    private const string WebSockerResultEvent = "data";
    private const int DelayMilliseconds = 200;

    private static readonly JsonSerializerOptions deserealizeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly IHubContext<OrderBookHub> _hubContext = hubContext;
    private readonly IOrderBookService _orderBookService = orderBookService;
    private readonly IOrderBookWebSockerService _orderBookWebSockerService = orderBookWebSockerService;
    private readonly ILogger<OrderBookWebSocketBackgroundService> _logger = logger;

    public abstract string TradingPair { get; }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _orderBookWebSockerService.StopWebSocket();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _orderBookWebSockerService.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            await StartWebSocket(stoppingToken);
        }
    }

    private CreateOrderBookDto GetCreateOrderBookDto(OrderBookJsonResponseDto orderBook) => new()
    {
        Asks = orderBook.Data.Asks,
        Bids = orderBook.Data.Bids,
        TradingPair = TradingPair
    };

    private async Task StartWebSocket(CancellationToken cancellationToken)
    {
        try
        {
            await _orderBookWebSockerService.Subscribe(TradingPair, cancellationToken);
            await Receiving(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while connecting to the WebSocket");
        }
    }

    private async Task Receiving(CancellationToken cancellationToken)
    {

        while (true)
        {
            await Task.Delay(DelayMilliseconds, cancellationToken);
            var (result, buffer) = await _orderBookWebSockerService.ReceiveAsync(cancellationToken);
            if (result.Count == 0)
            {
                continue;
            }
            
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var orderBookString = Encoding.UTF8.GetString(buffer, index: 0, result.Count);
                var orderBookResponse = JsonSerializer.Deserialize<OrderBookJsonResponseDto>(orderBookString, deserealizeOptions);
                if (orderBookResponse?.Event != WebSockerResultEvent)
                {
                    continue;
                }
                await _hubContext.Clients.Group(TradingPair).SendAsync(Constants.SignalR.Methods.ReceiveOrderBook, 
                    orderBookResponse.Data.Asks, orderBookResponse.Data.Bids);
                await _orderBookService.CreateOrderBookAsync(GetCreateOrderBookDto(orderBookResponse));
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await StopAsync(cancellationToken);
                break;
            }

        }
    }
}