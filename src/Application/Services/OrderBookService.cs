using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderBookTestTask.Application.Dtos;
using OrderBookTestTask.Application.Interfaces;
using OrderBookTestTask.Data.Interfaces;
using OrderBookTestTask.Data.Models;

namespace OrderBookTestTask.Application.Services;

public class OrderBookService(IOrderBookRepository orderBookRepository) : IOrderBookService
{
    private readonly IOrderBookRepository _orderBookRepository = orderBookRepository;

    public async Task<OrderBookJsonResponseDto> GetOrderBookAsync(DateTime dateTime, string tradingPair)
    {
        var orderBook = await _orderBookRepository.GetOrderBookAsync(dateTime, tradingPair);
        return new OrderBookJsonResponseDto
        {
            Asks = orderBook.Asks,
            Bids = orderBook.Bids,
            TimeStamp = orderBook.Created
        };
    }

    public async Task CreateOrderBookAsync(CreateOrderBookDto createOrderBookDto)
    {
        var orderBook = ConvertToOrderBook(createOrderBookDto);
        await _orderBookRepository.CreateOrderBookAsync(orderBook);
    }

    private static OrderBook ConvertToOrderBook(CreateOrderBookDto createOrderBookDto)
    {
        return new OrderBook
        {
            Id = Guid.NewGuid().ToString(),
            Asks = createOrderBookDto.Asks,
            Bids = createOrderBookDto.Bids,
            TradingPair = createOrderBookDto.TradingPair,
            Created = DateTime.Now,
        };
    }
}