using OrderBookTestTask.Data.Models;

namespace OrderBookTestTask.Data.Interfaces;

public interface IOrderBookRepository
{
    Task CreateOrderBookAsync(OrderBook orderBook);
    Task<OrderBook> GetOrderBookAsync(DateTime dateTime, string tradingPair);
}