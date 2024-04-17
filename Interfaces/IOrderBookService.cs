namespace OrderBookTestTask.Interfaces;
public interface IOrderBookService
{
    Task<string> GetOrderBookAsync();
    Task CreateOrderBookAsync();
}