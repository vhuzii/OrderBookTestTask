using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderBookTestTask.Dtos;
using OrderBookTestTask.Interfaces;
using OrderBookTestTask.Models;

namespace OrderBookTestTask.Services;

public class OrderBookService : IOrderBookService
{
    private readonly IMongoCollection<OrderBook> _orderBooksCollection;
    public OrderBookService(IOptions<OrderBookSnapshotsDatabaseSettings> orderBookSnapshotsDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            orderBookSnapshotsDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            orderBookSnapshotsDatabaseSettings.Value.DatabaseName);

        _orderBooksCollection = mongoDatabase.GetCollection<OrderBook>(
            orderBookSnapshotsDatabaseSettings.Value.OrderBookCollectionName);
    }
    public Task<string> GetOrderBookAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CreateOrderBookAsync(OrderBookDto orderBookDto)
    {
        await _orderBooksCollection.InsertOneAsync(ConvertToOrderBook(orderBookDto));
    }

    private static OrderBook ConvertToOrderBook(OrderBookDto orderBookDto)
    {
        return new OrderBook
        {
            Id = Guid.NewGuid().ToString(),
            Asks = orderBookDto.Data.Asks,
            Bids = orderBookDto.Data.Bids,
            Created = DateTime.UtcNow,
            TradingPair = orderBookDto.TradingPair
        };
    }
}