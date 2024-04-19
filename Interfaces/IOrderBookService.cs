using OrderBookTestTask.Dtos;
using OrderBookTestTask.Models;

namespace OrderBookTestTask.Interfaces;
public interface IOrderBookService
{
    public Task<OrderBook> GetOrderBookAsync(string tradingPair);
    public Task CreateOrderBookAsync(CreateOrderBookDto createOrderBookDto);
}