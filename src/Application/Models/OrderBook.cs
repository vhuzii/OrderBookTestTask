using MongoDB.Bson.Serialization.Attributes;

namespace OrderBookTestTask.Application.Models;

public class OrderBook 
{
    [BsonId]
    public string Id { get; set; } = null!;
    public DateTime Created { get; set; }
    public string TradingPair { get; set; } = null!;
    public string[][] Bids { get; set; } = null!;
    public string[][] Asks { get; set; } = null!;
}