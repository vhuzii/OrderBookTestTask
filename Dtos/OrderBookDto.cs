namespace OrderBookTestTask.Dtos;

public class OrderBookDtoJsonResponse
{
    public Data Data { get; set; } = null!;
    public string Event { get; set; } = null!;
    public string TradingPair { get; set; } = null!;
}

public class Data 
{
    public string[][] Bids { get; set; } = null!;
    public string[][] Asks { get; set; } = null!;
}
