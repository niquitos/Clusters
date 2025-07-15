using Clusters.Data.Models;
using Clusters.Hashing;
using System.Collections;
using System.Numerics;

namespace Clusters.Clusterization;

public static class DbscanBitArrayService
{
    private const double epsilon = 0.7;
    private const int density = 2;

    public static void Clusterize(EventModel[] events)
    {
        Parallel.ForEach(events, (ev, state, index) =>
        {
            ev.SimHash = SimHashService.DoSimd(ev.Text!);
        });

        uint clusterId = 1;
        var assignedEvents = new BitArray(events.Length);

        for (int i = 0; i < events.Length; i++)
        {
            if (!assignedEvents[i])
            {
                var neighbors = GetNeighbors(i, events, assignedEvents);

                if (neighbors.Count < density)
                {
                    continue;
                }
                else
                {
                    AssignCluster(i, neighbors, clusterId, events, assignedEvents);
                    ExpandCluster(events, neighbors, clusterId, assignedEvents);

                    clusterId++;
                }
            }
        }
    }

    private static void AssignCluster(int eventIndex,
        List<int> neighbors,
        uint clusterId,
        EventModel[] events,
        BitArray assignedEvents)
    {
        events[eventIndex].ClusterId = clusterId;
        assignedEvents.Set(eventIndex, true);

        foreach (var neighborIndex in neighbors)
        {
            events[neighborIndex].ClusterId = clusterId;
            assignedEvents.Set(neighborIndex, true);
        }
    }

    private static void ExpandCluster(EventModel[] events,
        List<int> neighbors,
        uint clusterId,
        BitArray assignedEvents)
    {
        foreach (var eventIndex in neighbors)
        {
            if (events[eventIndex].ClusterId == 0)
            {
                var newNeighbors = GetNeighbors(eventIndex, events, assignedEvents);

                if (newNeighbors.Count >= density)
                {
                    AssignCluster(eventIndex, newNeighbors, clusterId, events, assignedEvents);
                    ExpandCluster(events, newNeighbors, clusterId, assignedEvents);
                }
            }
        }
    }

    private static List<int> GetNeighbors(int eventIndex,
        EventModel[] events,
        BitArray assignedEvents)
    {
        var neighbors = new List<int>();
        var @event = events[eventIndex];

        for (int i = 0; i < events.Length; i++)
        {
            if (i != eventIndex && !assignedEvents[i] && HammingDistanceRatio(@event.SimHash, events[i].SimHash) >= epsilon)
            {
                neighbors.Add(i);
            }
        }

        return neighbors;
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }
}
