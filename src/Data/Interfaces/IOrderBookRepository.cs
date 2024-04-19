using OrderBookTestTask.Data.Models;

namespace OrderBookTestTask.Data.Interfaces;

public interface IOrderBookRepository
{
    Task<OrderBook> GetOrderBookAsync(string tradingPair);
    Task CreateOrderBookAsync(OrderBook orderBook);
}