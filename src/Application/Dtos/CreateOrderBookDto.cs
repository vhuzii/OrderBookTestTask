namespace OrderBookTestTask.Application.Dtos;

public class CreateOrderBookDto
{
    public string[][] Bids { get; set; } = null!;
    public string[][] Asks { get; set; } = null!;
    public string TradingPair { get; set; } = null!;
}
