namespace OrderBookTestTask.Data.Options;

public class OrderBookSnapshotsDatabaseOptions 
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OrderBookCollectionName { get; set; } = null!;
}