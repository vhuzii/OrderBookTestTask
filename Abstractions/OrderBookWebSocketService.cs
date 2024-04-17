using Microsoft.AspNetCore.SignalR;
using OrderBookTestTask.Hubs;
using OrderBookTestTask.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace OrderBookTestTask.Abstractions;

public abstract class OrderBookWebSocketBackgroundService : BackgroundService 
{
    private const string WebsocketUrl = "wss://ws.bitstamp.net";
    
    public abstract string TradingPair { get; }

    private readonly IHubContext<OrderBookHub> _hubContext;
    private readonly IOrderBookService _orderBookService;
    private readonly ClientWebSocket _clientWebSocket = new();

    public OrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext, 
        IOrderBookService orderBookService)
    {
        _hubContext = hubContext;
        _orderBookService = orderBookService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _clientWebSocket.ConnectAsync(new Uri(WebsocketUrl), CancellationToken.None);

        var websocketMessage = Encoding.UTF8.GetBytes(@"{
            ""event"": ""bts:subscribe"",
            ""data"": {
                ""channel"": ""order_book_" + TradingPair + @"""
            }
        }");
        await _clientWebSocket.SendAsync(websocketMessage, WebSocketMessageType.Text, true, CancellationToken.None);

        var receiving = Receiving(_clientWebSocket);

        await receiving;
    }

    private async Task Receiving(ClientWebSocket clientWebSocket)
    {
        var buffer = new byte[2048];

        while (true)
        {
            var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var orderBook = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await _hubContext.Clients.All.SendAsync("ReceiveOrderBook", orderBook);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                break;
            }

        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _clientWebSocket.Dispose();
        base.Dispose();
    }
}