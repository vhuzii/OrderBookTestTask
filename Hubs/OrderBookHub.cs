using Microsoft.AspNetCore.SignalR;

namespace OrderBookTestTask.Hubs
{
    public class OrderBookHub : Hub
    {
        public async Task SendOrderBook(string orderBook)
        {
            await Clients.All.SendAsync("ReceiveOrderBook", orderBook);
        }
    }
}