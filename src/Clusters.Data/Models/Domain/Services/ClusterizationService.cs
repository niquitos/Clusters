using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace Clusters.Data.Models.Domain.Services;

public class ClusterizationService
{
    private readonly ClusterizationOperation _operation;
    private readonly Dictionary<string, double> _similarities = [];

    public static readonly Guid NoNeighborsClusterId = Guid.Parse("00000000-0000-0000-0000-000000000001");

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

        _operation.CreateClusters(clusters);
    }

    private static void SetEventsHashes(IEnumerable<string> fields, List<ClusterEvent> events)
    {
        Parallel.ForEach(events, @event =>
        {
            foreach (var field in fields)
            {
                var value = @event.Payload[field];

                var hashValue = HashService.DoSIMD((string)value!);

                @event.AddHash(field, hashValue);
            }
        });
    }

    private List<ClusterEvent> CreateClusters(List<ClusterEvent> events)
    {
        for (var i = 0; i < events.Count; i++)
        {
            if (events[i].ClusterId.HasValue)
                continue;

            events[i].SetClusterId(Guid.NewGuid());

            var neighborsCount = 0;

            for (var j = i + 1; j < events.Count; j++)
            {
                if (events[j].ClusterId.HasValue)
                    continue;

                var isNeighbor = false;

                foreach (var (key, hash) in events[i].Hashes)
                {
                    var otherHash = events[j].Hashes[key];

                    if (hash == 0 || otherHash == 0)
                    {
                        isNeighbor = hash == 0 && otherHash == 0;
                        break;
                    }

                    var similarity = HammingDistanceRatio(hash, otherHash);

                    isNeighbor = similarity >= _similarities[key];
                }

                if (!isNeighbor)
                    continue;

                events[j].SetClusterId(events[i].ClusterId!.Value);
                neighborsCount++;
            }

            if (neighborsCount == 0)
                events[i].SetClusterId(Guid.Empty);
        }

        return events;
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

public abstract class ClusterizationServiceBase
{
    protected readonly ClusterizationOperation _operation;
    protected readonly Dictionary<string, double> _similarities = [];

    protected ClusterizationServiceBase(ClusterizationOperation operation)
    {
        _operation = operation;

        foreach (var field in operation.ClusterizationCriteria.Fields)
        {
            _similarities[field.Name.Value] = field.Similarity.Value / 100.0;
        }
    }

    public void Clusterize(List<ClusterEvent> events)
    {

        SetEventsHashes(_similarities.Keys, events);

        var clusters = CreateClusters(events);

        _operation.CreateClusters(clusters);
    }

    protected static void SetEventsHashes(IEnumerable<string> fields, List<ClusterEvent> events)
    {
        Parallel.ForEach(events, @event =>
        {
            foreach (var field in fields)
            {
                var value = @event.Payload[field];
                var hashValue = HashService.CalculateSimhash(value?.ToString()!);
                @event.AddHash(field, hashValue);
            }
        });
    }

    protected List<ClusterEvent> CreateClusters(List<ClusterEvent> events)
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

    protected static void AssignCluster(ClusterEvent @event, IEnumerable<ClusterEvent> neighbors, Guid clusterId)
    {
        @event.SetClusterId(clusterId);

        foreach (var neighbor in neighbors)
        {
            neighbor.SetClusterId(clusterId);
        }
    }

    protected abstract IEnumerable<ClusterEvent> GetNeighbors(List<ClusterEvent> events, ClusterEvent @event);
}

public class HammingDistanceClusterizationService : ClusterizationServiceBase
{
    public HammingDistanceClusterizationService(ClusterizationOperation operation) : base(operation)
    {

    }

    protected override IEnumerable<ClusterEvent> GetNeighbors(List<ClusterEvent> events, ClusterEvent @event)
    {
        return events
            .Where(x => x != @event && !x.ClusterId.HasValue && _similarities.All(kvp =>
            {
                var hash = @event.Hashes[kvp.Key];
                var otherHash = x.Hashes[kvp.Key];

                if (hash == 0 || otherHash == 0)
                    return hash == 0 && otherHash == 0;

                var similarity = HammingDistanceRatio(hash, otherHash);

                return similarity >= kvp.Value;
            }));
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }
}