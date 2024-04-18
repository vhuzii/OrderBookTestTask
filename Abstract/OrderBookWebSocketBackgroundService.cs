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
    
    public abstract string TradingPair { get; }

    private readonly IHubContext<OrderBookHub> _hubContext;
    private readonly IOrderBookService _orderBookService;
    private readonly ClientWebSocket _clientWebSocket = new();
    private readonly string _websocketUrl;


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
        var buffer = new byte[WebSocketResultBufferSize];

        while (true)
        {
            var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var orderBookString = Encoding.UTF8.GetString(buffer, index: 0, result.Count);
                var orderBook = JsonSerializer.Deserialize<OrderBookDto>(orderBookString, deserealizeOptions);
                orderBook!.TradingPair = TradingPair;
                if (orderBook.Event != WebSockerResultEvent)
                {
                    continue;
                }

                await _hubContext.Clients.All.SendAsync("ReceiveOrderBook", orderBookString);
                await _orderBookService.CreateOrderBookAsync(orderBook);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                break;
            }

        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _clientWebSocket.Dispose();
        base.Dispose();
    }
}