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
    IOrderBookService orderBookService, IOrderBookWebSocketService orderBookWebSocketService, ILogger<OrderBookWebSocketBackgroundService> logger) 
        : BackgroundService 
{
    private const string WebSocketResultEvent = "data";
    private const int DelayMilliseconds = 200;

    private static readonly JsonSerializerOptions deserealizeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly IHubContext<OrderBookHub> _hubContext = hubContext;
    private readonly IOrderBookService _orderBookService = orderBookService;
    private readonly IOrderBookWebSocketService _orderBookWebSocketService = orderBookWebSocketService;
    private readonly ILogger<OrderBookWebSocketBackgroundService> _logger = logger;

    public abstract string TradingPair { get; }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _orderBookWebSocketService.StopWebSocket();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _orderBookWebSocketService.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            await StartWebSocket(cancellationToken);
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
            await _orderBookWebSocketService.Subscribe(TradingPair, cancellationToken);
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
            var (webSocketResult, buffer) = await _orderBookWebSocketService.ReceiveAsync(cancellationToken);
            if (webSocketResult.Count == 0)
            {
                continue;
            }
            
            if (webSocketResult.MessageType == WebSocketMessageType.Text)
            {
                var orderBookString = Encoding.UTF8.GetString(buffer, index: 0, webSocketResult.Count);
                var orderBookResponse = JsonSerializer.Deserialize<OrderBookJsonResponseDto>(orderBookString, deserealizeOptions);
                if (orderBookResponse?.Event != WebSocketResultEvent)
                {
                    continue;
                }
                await _hubContext.Clients.Group(TradingPair).SendAsync(Constants.SignalR.Methods.ReceiveOrderBook, 
                    orderBookResponse.Data.Asks, orderBookResponse.Data.Bids);
                await _orderBookService.CreateOrderBookAsync(GetCreateOrderBookDto(orderBookResponse));
            }
            else if (webSocketResult.MessageType == WebSocketMessageType.Close)
            {
                await StopAsync(cancellationToken);
                break;
            }

        }
    }
}