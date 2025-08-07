using Newtonsoft.Json.Linq;

namespace Clusters.Data.Models.Domain;

public class ClusterEvent
{
    private readonly Dictionary<string, ulong> _hashes = [];

    public Guid Id { get; }
    public Guid? ClusterId { get; private set; }
    public JToken Payload { get; private set; }

    public IReadOnlyDictionary<string, ulong> Hashes => _hashes;

    public ClusterEvent(JToken payload)
    {
        Id = Guid.NewGuid();
        Payload = payload;
    }

    public void SetClusterId(Guid? id)
    {
        ClusterId = id;
    }

    public void AddHash(string field, ulong hash) => _hashes[field] = hash;
}