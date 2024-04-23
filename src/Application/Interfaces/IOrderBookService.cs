using OrderBookTestTask.Application.Dtos;
using OrderBookTestTask.Data.Models;

namespace OrderBookTestTask.Application.Interfaces;
public interface IOrderBookService
{
    public Task CreateOrderBookAsync(CreateOrderBookDto createOrderBookDto);
    public Task<OrderBookJsonResponseDto> GetOrderBookAsync(DateTime dateTime, string tradingPair);
}