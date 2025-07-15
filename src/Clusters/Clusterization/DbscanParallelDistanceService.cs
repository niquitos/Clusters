using Clusters.Data.Models;
using Clusters.Hashing;
using System.Numerics;

namespace Clusters.Clusterization;

public static class DbscanParallelDistanceService
{
    private const double epsilon = 0.7;
    private const int density = 2;

    public static void Clusterize(List<EventModel> events)
    {
        Parallel.ForEach(events, @event =>
        {
            @event.SimHash = SimHashService.DoSimd(@event.Text!);
        });

        uint clusterId = 1;
        var assignedEvents = new HashSet<EventModel>();

        foreach (var @event in events)
        {
            if (!assignedEvents.Contains(@event))
            {
                var neighbors = GetNeighbors(@event, events, assignedEvents);

                if (neighbors.Count < density)
                {
                    continue;
                }
                else
                {
                    AssignCluster(@event, neighbors, clusterId, assignedEvents);
                    ExpandCluster(events, neighbors, clusterId, assignedEvents);

                    clusterId++;
                }
            }
        }
    }

    private static void AssignCluster(EventModel @event,
        List<EventModel> neighbors,
        uint clusterId,
        HashSet<EventModel> assignedEvents)
    {
        @event.ClusterId = clusterId;
        assignedEvents.Add(@event);

        foreach (var neighbor in neighbors)
        {
            neighbor.ClusterId = clusterId;
            assignedEvents.Add(neighbor);
        }
    }

    private static void ExpandCluster(List<EventModel> events,
        List<EventModel> neighbors,
        uint clusterId,
        HashSet<EventModel> assignedEvents)
    {
        foreach (var @event in neighbors)
        {
            if (@event.ClusterId == 0)
            {
                var newNeighbors = GetNeighbors(@event, events, assignedEvents);

                if (newNeighbors.Count >= density)
                {
                    AssignCluster(@event, newNeighbors, clusterId, assignedEvents);
                    ExpandCluster(events, newNeighbors, clusterId, assignedEvents);
                }
            }
        }
    }

    private static List<EventModel> GetNeighbors(EventModel @event,
        List<EventModel> events,
        HashSet<EventModel> assignedEvents)
    {
        return events
            .AsParallel()
            .Where(x => x != @event && !assignedEvents.Contains(x))
            .Where(x => HammingDistanceRatio(@event.SimHash, x.SimHash) >= epsilon)
            .ToList();
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }
}
