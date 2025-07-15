namespace Clusters.Data.Models;

public class EventModel
{
    public string? Time { get; set; }
    public string? EventSrcHost { get; set; }
    public string? Text { get; set; }
    public string? AlertKey { get; set; }
    public string? AlertIocDescription { get; set; }


    public uint ClusterId { get; set; }
    public ulong SimHash{ get; set; }
}
