namespace Clusters.Data.Models;

public class EventModel
{
    public string? Time { get; set; }
    public string? EventSrcHost { get; set; }
    public string? Text { get; set; }
    public string? AlertKey { get; set; }
    public string? CorrelationName { get; set; }


    public int ClusterId { get; set; }
    public ulong SimHash1 { get; set; }
    public ulong SimHash2 { get; set; }
    public ulong SimHash3{ get; set; }

    public (ulong,ulong) SimHash128_1 { get; set; }
    public (ulong, ulong) SimHash128_2 { get; set; }
    public (ulong, ulong) SimHash128_3 { get; set; }
}
