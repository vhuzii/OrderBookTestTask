using OrderBookTestTask.Application.Dtos;
using OrderBookTestTask.Application.Models;

namespace OrderBookTestTask.Application.Interfaces;
public interface IOrderBookService
{
    public Task<OrderBook> GetOrderBookAsync(string tradingPair);
    public Task CreateOrderBookAsync(CreateOrderBookDto createOrderBookDto);
}