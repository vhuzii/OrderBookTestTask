using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderBookTestTask.Dtos;
using OrderBookTestTask.Interfaces;
using OrderBookTestTask.Models;

namespace OrderBookTestTask.Services;

public class OrderBookService : IOrderBookService
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IOptions<OrderBookSnapshotsDatabaseSettings> _orderBookSnapshotsDatabaseSettings;
    public OrderBookService(IOptions<OrderBookSnapshotsDatabaseSettings> orderBookSnapshotsDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            orderBookSnapshotsDatabaseSettings.Value.ConnectionString);

        _mongoDatabase = mongoClient.GetDatabase(
            orderBookSnapshotsDatabaseSettings.Value.DatabaseName);
        
        _orderBookSnapshotsDatabaseSettings = orderBookSnapshotsDatabaseSettings;

    }
    
    public async Task<OrderBook> GetOrderBookAsync(string tradingPair)
    {
        var orderBooksCollection = GetCollection(tradingPair);
        var orderBook = await orderBooksCollection.Find(orderBook => true).FirstOrDefaultAsync();
        return orderBook;
    }

    public async Task CreateOrderBookAsync(CreateOrderBookDto createOrderBookDto)
    {
        var orderBooksCollection = GetCollection(createOrderBookDto.TradingPair);
        await orderBooksCollection.InsertOneAsync(ConvertToOrderBook(createOrderBookDto));
    }

    private static OrderBook ConvertToOrderBook(CreateOrderBookDto createOrderBookDto)
    {
        return new OrderBook
        {
            Id = Guid.NewGuid().ToString(),
            Asks = createOrderBookDto.Asks,
            Bids = createOrderBookDto.Bids,
            Created = DateTime.UtcNow,
            TradingPair = createOrderBookDto.TradingPair
        };
    }

    private IMongoCollection<OrderBook> GetCollection(string tradingPair) 
    {
        return _mongoDatabase.GetCollection<OrderBook>(_orderBookSnapshotsDatabaseSettings.Value.OrderBookCollectionNamePrefix + tradingPair);
    }
}