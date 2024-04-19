using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderBookTestTask.Application.Abstract.Services.Background;
using OrderBookTestTask.Application.Constants;
using OrderBookTestTask.Application.Hubs;
using OrderBookTestTask.Application.Interfaces;

namespace OrderBookTestTask.Application.Services.Background;

public class BtcUsdOrderBookWebSocketBackgroundService(IHubContext<OrderBookHub> hubContext,
    IOrderBookService orderBookService, IConfiguration configuration, ILogger<OrderBookWebSocketBackgroundService> logger) 
    : OrderBookWebSocketBackgroundService(hubContext, orderBookService, configuration, logger)
{
    public override string TradingPair => TradingPairs.BtcUsd;
}

