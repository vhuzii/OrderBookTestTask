using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Configuration;
using OrderBookTestTask.Application.Interfaces;

namespace OrderBookTestTask.Application.Services;

public class OrderBookWebSockerService(IConfiguration configuration) : IOrderBookWebSockerService
{
    private const int WebSocketResultBufferSize = 10240;

    private readonly string _websocketUrl = configuration["OrderBookWebSocketUrl"]!;
    private ClientWebSocket _clientWebSocket = new();
    private byte[] _buffer = new byte[WebSocketResultBufferSize];

    private static string GetMessage(string tradingPair) => @"{
        ""event"": ""bts:subscribe"",
        ""data"": {
            ""channel"": ""order_book_" + tradingPair + @"""
        }
    }";

    public void Dispose()
    {
        _clientWebSocket.Dispose();
    }

    public async Task<(WebSocketReceiveResult webSocketResult, byte[] buffer)> ReceiveAsync(CancellationToken cancellationToken)
    {
        var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(_buffer), cancellationToken);
        return (result, _buffer);
    }

    public Task StopWebSocket()
    {
        return _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }

    public async Task Subscribe(string tradingPair, CancellationToken cancellationToken)
    {
        await _clientWebSocket.ConnectAsync(new Uri(_websocketUrl), cancellationToken);
        await _clientWebSocket.SendAsync(Encoding.UTF8.GetBytes(GetMessage(tradingPair)), WebSocketMessageType.Text, true, cancellationToken);
    }

}