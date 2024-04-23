namespace OrderBookTestTask.Application.Dtos;

public class OrderBookJsonResponseDto
{
    public string[][] Bids { get; set; } = null!;
    public string[][] Asks { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
}
