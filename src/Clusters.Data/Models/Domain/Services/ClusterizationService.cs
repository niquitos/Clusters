using System.Numerics;

namespace Clusters.Data.Models.Domain.Services;

public class ClusterizationService
{
    private readonly ClusterizationOperation _operation;
    private readonly Dictionary<string, double> _similarities = [];

    public ClusterizationService(ClusterizationOperation operation)
    {
        _operation = operation;
        foreach (var field in operation.ClusterizationCriteria.Fields)
        {
            _similarities[field.Name.Value] = (field.Similarity.Value / 100.0);
        }
    }

    public void Clusterize(List<ClusterEvent> events)
    {
        SetEventsHashes(_similarities.Keys, events);

        var clusters = CreateClusters(events);

        _operation.SetClusters(clusters);
    }

    private static void SetEventsHashes(IEnumerable<string> fields, List<ClusterEvent> events)
    {
        Parallel.ForEach(events, @event =>
        {
            foreach (var field in fields)
            {
                var value = @event.Payload[field];

                var hashValue = HashService.CalculateSimhash((string)value!);

                @event.AddHash(field, hashValue);
            }
        });
    }

    private List<ClusterEvent> CreateClusters(List<ClusterEvent> events)
    {
        foreach (var @event in events)
        {
            if (!@event.ClusterId.HasValue)
            {
                var neighbors = GetNeighbors(events, @event);
                if (!neighbors.Any())
                {
                    @event.SetClusterId(Guid.Empty);
                    continue;
                }

                AssignCluster(@event, neighbors, Guid.NewGuid());
            }
        }

        return events;
    }

    private static void AssignCluster(ClusterEvent @event, IEnumerable<ClusterEvent> neighbors, Guid clusterid)
    {
        @event.SetClusterId(clusterid);

        foreach (var neighbor in neighbors)
        {
            neighbor.SetClusterId(clusterid);
        }
    }

    private IEnumerable<ClusterEvent> GetNeighbors(List<ClusterEvent> events, ClusterEvent @event)
    {
        return events
            .Where(x => x != @event && !x.ClusterId.HasValue && @event.Hashes.All(kvp =>
                {
                    var hash = kvp.Value;
                    var otherHash = x.Hashes[kvp.Key];

                    if (hash == 0 || otherHash == 0)
                        return hash == 0 && otherHash == 0;

                    var similarity = HammingDistanceRatio(hash, otherHash);

                    return similarity >= _similarities[kvp.Key];
                }));
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }

    private static double CalculateJaccardRatio(ulong hash1, ulong hash2)
    {
        var intersect = hash1 & hash2;
        var union = hash1 | hash2;

        return (double)BitOperations.PopCount(intersect) / BitOperations.PopCount(union);
    }
}