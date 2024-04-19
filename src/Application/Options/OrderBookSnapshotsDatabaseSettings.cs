namespace OrderBookTestTask.Application.Options;

public class OrderBookSnapshotsDatabaseOptions 
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OrderBookCollectionNamePrefix { get; set; } = null!;
}