using Microsoft.AspNetCore.SignalR;
using OrderBookTestTask.Abstract.Services.Background;
using OrderBookTestTask.Constants;
using OrderBookTestTask.Hubs;
using OrderBookTestTask.Interfaces;

namespace OrderBookTestTask.Services.Background;

public class BtcEurOrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext,
    IOrderBookService orderBookService, IConfiguration configuration, ILogger<OrderBookWebSocketBackgroundService> logger) 
    : OrderBookWebSocketBackgroundService(hubContext, orderBookService, configuration, logger)
{
    public override string TradingPair => TradingPairs.BtcEur;
}

