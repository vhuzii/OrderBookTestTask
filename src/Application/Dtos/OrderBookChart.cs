namespace OrderBookTestTask.Application.Dtos;

public class OrderBookChartDto
{
    public List<ChartDto> BidsChart { get; set; } = null!;
    public List<ChartDto> AsksChart { get; set; } = null!;
}
