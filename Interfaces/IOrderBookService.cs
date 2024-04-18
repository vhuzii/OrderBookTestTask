using OrderBookTestTask.Dtos;
using OrderBookTestTask.Models;

namespace OrderBookTestTask.Interfaces;
public interface IOrderBookService
{
    Task<string> GetOrderBookAsync();
    Task CreateOrderBookAsync(OrderBookDto orderBook);
}