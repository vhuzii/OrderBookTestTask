using Microsoft.AspNetCore.SignalR;
using OrderBookTestTask.Abstractions;
using OrderBookTestTask.Constants;
using OrderBookTestTask.Hubs;
using OrderBookTestTask.Interfaces;

namespace OrderBookTestTask.Services.Background;

public class BtcEurOrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext,
    IOrderBookService orderBookService, IConfiguration configuration) 
    : OrderBookWebSocketBackgroundService(hubContext, orderBookService, configuration)
{
    public override string TradingPair => TradingPairs.BtcEur;
}

