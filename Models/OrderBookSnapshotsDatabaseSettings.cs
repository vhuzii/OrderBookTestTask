namespace OrderBookTestTask.Models;

public class OrderBookSnapshotsDatabaseSettings 
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OrderBookCollectionName { get; set; } = null!;
}