namespace OrderBookTestTask.Models;

public class OrderBookSnapshotsDatabaseSettings 
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OrderBookCollectionNamePrefix { get; set; } = null!;
}