using Microsoft.AspNetCore.SignalR;
using OrderBookTestTask.Abstractions;
using OrderBookTestTask.Constants;
using OrderBookTestTask.Hubs;
using OrderBookTestTask.Interfaces;

namespace OrderBookTestTask.Services.Background;

public class BtcEurOrderBookWebSocketBackgroundService : OrderBookWebSocketBackgroundService
{
    public BtcEurOrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext, 
        IOrderBookService orderBookService) : base(hubContext, orderBookService)
    {
    }

    public override string TradingPair => TradingPairs.BtcEur;
}

