using MongoDB.Bson.Serialization.Attributes;

namespace OrderBookTestTask.Models;

public class OrderBook 
{
    [BsonId]
    public string Id { get; set; }
    public DateTime Created { get; set; }
    public string TradingPair { get; set; }
    public string[][] Bids { get; set; }
    public string[][] Asks { get; set; }
}