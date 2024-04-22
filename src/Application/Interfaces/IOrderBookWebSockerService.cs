
using System.Net.WebSockets;

namespace OrderBookTestTask.Application.Interfaces;

public interface IOrderBookWebSockerService : IDisposable
{
    Task Subscribe(string tradingPair, CancellationToken cancellationToken);
    Task<(WebSocketReceiveResult webSocketResult, byte[] buffer)> ReceiveAsync(CancellationToken cancellationToken); 
    Task StopWebSocket();
}