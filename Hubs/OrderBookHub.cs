using Microsoft.AspNetCore.SignalR;

namespace OrderBookTestTask.Hubs
{
    public class OrderBookHub : Hub
    {
        public async Task JoinRoom(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}