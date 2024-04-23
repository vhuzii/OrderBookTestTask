using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderBookTestTask.Data.Models;
using OrderBookTestTask.Data.Options;

namespace OrderBookTestTask.Data.Interfaces;

public class OrderBookRepository : IOrderBookRepository
{

    private readonly IMongoDatabase _mongoDatabase;
    private IMongoCollection<OrderBook> _orderBookCollection;

    public OrderBookRepository(IOptions<OrderBookSnapshotsDatabaseOptions> orderBookSnapshotsDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            orderBookSnapshotsDatabaseSettings.Value.ConnectionString);

        _mongoDatabase = mongoClient.GetDatabase(
            orderBookSnapshotsDatabaseSettings.Value.DatabaseName);

        _orderBookCollection = _mongoDatabase.GetCollection<OrderBook>(orderBookSnapshotsDatabaseSettings.Value.OrderBookCollectionName);
    }

    public async Task CreateOrderBookAsync(OrderBook orderBook)
    {
        await _orderBookCollection.InsertOneAsync(orderBook);
    }

    public Task<OrderBook> GetOrderBookAsync(DateTime dateTime, string tradingPair)
    {
        return _orderBookCollection.Find(x => x.Created < dateTime && x.TradingPair == tradingPair)
            .SortByDescending(x => x.Created)
            .FirstOrDefaultAsync();
    }
}