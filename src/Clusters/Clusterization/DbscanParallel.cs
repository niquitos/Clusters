using Clusters.Data.Models;
using Clusters.Hashing;
using System.Numerics;

namespace Clusters.Clusterization;

public static class DbscanParallel
{
    private const double epsilon1 = 0.7;
    private const double epsilon2 = 0.6;
    private const double epsilon3 = 0.85;

    public static void Clusterize(EventModel[] events)
    {
        Parallel.ForEach(events, @event =>
        {
            @event.SimHash1 = SimHashService.DoSimd(@event.Text!);
            @event.SimHash2 = SimHashService.DoSimd(@event.AlertKey!);
            @event.SimHash3 = SimHashService.DoSimd(@event.CorrelationName!);
        });

        var clusterId = 1;

        foreach (var @event in events)
        {
            if (@event.ClusterId == 0)
            {
                var neighbors = GetNeighbors(@event, events);

                if (!neighbors.Any())
                {
                    @event.ClusterId = -1;
                    continue;
                }
                else
                {
                    AssignCluster(@event, neighbors, clusterId);
                    ExpandCluster(events, neighbors, clusterId);

                    clusterId++;
                }
            }
        }
    }

    private static void AssignCluster(EventModel @event, IEnumerable<EventModel> neighbors, int clusterId)
    {
        @event.ClusterId = clusterId;

        Parallel.ForEach(neighbors, neighbor => { neighbor.ClusterId = clusterId; });
    }

    private static void ExpandCluster(EventModel[] events, IEnumerable<EventModel> neighbors, int clusterId)
    {
        foreach (var @event in neighbors)
        {
            var newNeighbors = GetNeighbors(@event, events);

            if (newNeighbors.Any())
            {
                AssignCluster(@event, newNeighbors, clusterId);
                ExpandCluster(events, newNeighbors, clusterId);
            }
        }
    }

    private static ParallelQuery<EventModel> GetNeighbors(EventModel @event, EventModel[] events)
    {
        return events
            .AsParallel()
            .Where(x => x != @event && x.ClusterId == 0)
            .Where(x =>
                HammingDistanceRatio(@event.SimHash1, x.SimHash1) >= epsilon1 &&
                HammingDistanceRatio(@event.SimHash2, x.SimHash2) >= epsilon2 &&
                HammingDistanceRatio(@event.SimHash3, x.SimHash3) >= epsilon3);
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }
}

