using Microsoft.AspNetCore.SignalR;
using OrderBookTestTask.Abstractions;
using OrderBookTestTask.Constants;
using OrderBookTestTask.Hubs;
using OrderBookTestTask.Interfaces;

namespace OrderBookTestTask.Services.Background;

public class BtcUsdOrderBookWebSocketBackgroundService : OrderBookWebSocketBackgroundService
{
    public BtcUsdOrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext, 
        IOrderBookService orderBookService) : base(hubContext, orderBookService)
    {
    }

    public override string TradingPair => TradingPairs.BtcUsd;
}

