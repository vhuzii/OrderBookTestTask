namespace OrderBookTestTask.Application.Dtos;

public class OrderBookChartsDto
{
    public List<ChartDto> BidsChart { get; set; } = null!;
    public List<ChartDto> AsksChart { get; set; } = null!;
}
