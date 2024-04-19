using Microsoft.AspNetCore.SignalR;
using OrderBookTestTask.Dtos;
using OrderBookTestTask.Hubs;
using OrderBookTestTask.Interfaces;
using OrderBookTestTask.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace OrderBookTestTask.Abstractions;

public abstract class OrderBookWebSocketBackgroundService : BackgroundService 
{
    private const int WebSocketResultBufferSize = 10240;
    private const string WebSockerResultEvent = "data";

    private static readonly JsonSerializerOptions deserealizeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    private readonly IHubContext<OrderBookHub> _hubContext;
    private readonly IOrderBookService _orderBookService;
    private readonly ClientWebSocket _clientWebSocket = new();
    private readonly string _websocketUrl;

    public abstract string TradingPair { get; }

    public OrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext, 
        IOrderBookService orderBookService, IConfiguration configuration)
    {
        _hubContext = hubContext;
        _orderBookService = orderBookService;
        _websocketUrl = configuration["WebSocketUrl"];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _clientWebSocket.ConnectAsync(new Uri(_websocketUrl), CancellationToken.None);

        var websocketMessage = Encoding.UTF8.GetBytes(GetMessage(TradingPair));
        await _clientWebSocket.SendAsync(websocketMessage, WebSocketMessageType.Text, true, CancellationToken.None);

        var receiving = Receiving(_clientWebSocket);

        await receiving;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopWebSocket();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _clientWebSocket.Dispose();
        base.Dispose();
    }

    private static string GetMessage(string tradingPair) => @"{
        ""event"": ""bts:subscribe"",
        ""data"": {
            ""channel"": ""order_book_" + tradingPair + @"""
        }
    }";

    private static CreateOrderBookDto GetCreateOrderBookDto(OrderBookDtoJsonResponse orderBook) => new()
    {
        Asks = orderBook.Data.Asks,
        Bids = orderBook.Data.Bids,
        TradingPair = orderBook.TradingPair
    };

    private async Task Receiving(ClientWebSocket clientWebSocket)
    {
        var buffer = new byte[WebSocketResultBufferSize];

        while (true)
        {
            var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.Count == 0)
            {
                continue;
            }
            
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var orderBookString = Encoding.UTF8.GetString(buffer, index: 0, result.Count);
                var orderBookResponse = JsonSerializer.Deserialize<OrderBookDtoJsonResponse>(orderBookString, deserealizeOptions);
                orderBookResponse!.TradingPair = TradingPair;
                if (orderBookResponse.Event != WebSockerResultEvent)
                {
                    continue;
                }

                await _hubContext.Clients.Group(TradingPair).SendAsync(Constants.SignalR.Methods.ReceiveOrderBook, 
                    orderBookResponse.Data.Asks, orderBookResponse.Data.Bids);
                await _orderBookService.CreateOrderBookAsync(GetCreateOrderBookDto(orderBookResponse));
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await StopWebSocket();
                break;
            }

        }
    }
    
    private async Task StopWebSocket()
    {
        await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }
}