using Microsoft.AspNetCore.SignalR;
using OrderBookTestTask.Hubs;
using OrderBookTestTask.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace OrderBookTestTask.Services.Background;

public class OrderBookWebSocketService<TradingPair> : BackgroundService 
    where TradingPair : Abstractions.TradingPair, new()
{
    private const string Url = "wss://ws.bitstamp.net";
        
    TradingPair _tradingPair = new TradingPair();

    private readonly IHubContext<OrderBookHub> _hubContext;
    private readonly IOrderBookService _orderBookService;

    public OrderBookWebSocketService(IHubContext<OrderBookHub> hubContext, 
        IOrderBookService orderBookService)
    {
        _hubContext = hubContext;
        _orderBookService = orderBookService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri(Url), CancellationToken.None);
            await ws.SendAsync(Encoding.UTF8.GetBytes(@"{
                ""event"": ""bts:subscribe"",
                ""data"": {
                    ""channel"": ""order_book_" + _tradingPair.Name + @"""
                }
            }"), WebSocketMessageType.Text, true, CancellationToken.None);

        var receiving = Receiving(ws);

        await receiving;
    }

    private async Task Receiving(ClientWebSocket ws)
    {
        var buffer = new byte[2048];

        while (true)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var orderBook = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await _hubContext.Clients.All.SendAsync("ReceiveOrderBook", orderBook);
                //_orderBookService.CreateOrderBookAsync(orderBook);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                break;
            }

        }
    }
}