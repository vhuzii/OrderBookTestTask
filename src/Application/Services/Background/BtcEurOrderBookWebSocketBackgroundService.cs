using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderBookTestTask.Application.Abstract.Services.Background;
using OrderBookTestTask.Application.Constants;
using OrderBookTestTask.Application.Hubs;
using OrderBookTestTask.Application.Interfaces;

namespace OrderBookTestTask.Application.Services.Background;

public class BtcEurOrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext,
    IOrderBookService orderBookService, IOrderBookWebSocketService orderBookWebSocketService, ILogger<OrderBookWebSocketBackgroundService> logger) 
    : OrderBookWebSocketBackgroundService(hubContext, orderBookService, orderBookWebSocketService, logger)
{
    public override string TradingPair => TradingPairs.BtcEur;
}

