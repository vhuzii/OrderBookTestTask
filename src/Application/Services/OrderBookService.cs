using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderBookTestTask.Application.Dtos;
using OrderBookTestTask.Application.Interfaces;
using OrderBookTestTask.Data.Interfaces;
using OrderBookTestTask.Data.Models;

namespace OrderBookTestTask.Application.Services;

public class OrderBookService(IOrderBookRepository orderBookRepository) : IOrderBookService
{
    private readonly IOrderBookRepository orderBookRepository = orderBookRepository;

    public async Task<OrderBook> GetOrderBookAsync(string tradingPair)
    {
        return await orderBookRepository.GetOrderBookAsync(tradingPair);
    }

    public async Task CreateOrderBookAsync(CreateOrderBookDto createOrderBookDto)
    {
        var orderBook = ConvertToOrderBook(createOrderBookDto);
        await orderBookRepository.CreateOrderBookAsync(orderBook);
    }

    private static OrderBook ConvertToOrderBook(CreateOrderBookDto createOrderBookDto)
    {
        return new OrderBook
        {
            Id = Guid.NewGuid().ToString(),
            Asks = createOrderBookDto.Asks,
            Bids = createOrderBookDto.Bids,
            TradingPair = createOrderBookDto.TradingPair,
            Created = DateTime.UtcNow,
        };
    }
}