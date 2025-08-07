using Clusters.Data.Models;
using Clusters.Hashing;
using System.Numerics;
using ZLinq;
using ZLinq.Linq;

namespace Clusters.Clusterization;

public static class DbscanNoRecursion
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
                AssignCluster(@event, neighbors, clusterId);
                //ExpandCluster(events, neighbors, clusterId);

                clusterId++;
            }
        }
    }

    private static void AssignCluster(EventModel @event, IEnumerable<EventModel> neighbors, int clusterId)
    {
        @event.ClusterId = clusterId;

        foreach (var neighbor in neighbors)
        {
            neighbor.ClusterId = clusterId;
        }
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

    private static IEnumerable<EventModel> GetNeighbors(EventModel @event, EventModel[] events)
    {
        foreach (var other in events)
        {
            if (other != @event && other.ClusterId == 0 &&
                HammingDistanceRatio(@event.SimHash1, other.SimHash1) >= epsilon1 &&
                HammingDistanceRatio(@event.SimHash2, other.SimHash2) >= epsilon2 &&
                HammingDistanceRatio(@event.SimHash3, other.SimHash3) >= epsilon3)
            {
                yield return other;
            }
        }
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }

    private static double CalculateJaccardSimilarity(ulong hash1, ulong hash2)
    {
        var intersection = hash1 & hash2;
        var union = hash1 | hash2;

        var intersectionCount = BitOperations.PopCount(intersection);
        var unionCount = BitOperations.PopCount(union);

        return (double)intersectionCount / unionCount;
    }
}

public static class DbscanSplit2
{
    private const double epsilon1 = 0.88;
    private const double epsilon2 = 0.6;
    private const double epsilon3 = 0.85;

    public static void Clusterize(EventModel[] events)
    {
        Parallel.ForEach(events, @event =>
        {
            @event.SimHash1 = SimHashService.BitHackSplit2(@event.Text!);
            //@event.SimHash2 = SimHashService.BitHackSplit2(@event.AlertKey!);
            //@event.SimHash3 = SimHashService.BitHackSplit2(@event.CorrelationName!);
        });

        var clusterId = 1;
        foreach (var @event in events)
        {
            if (@event.ClusterId == 0)
            {
                var neighbors = GetNeighbors(events, @event);
                if (neighbors.Length == 0)
                {
                    @event.ClusterId = -1;
                    continue;
                }
                AssignCluster(@event, neighbors, clusterId);
                //ExpandCluster(events, neighbors, clusterId);
                clusterId++;
            }
        }
    }

    private static void ExpandCluster(EventModel[] events, EventModel[] neighbors, int clusterId)
    {
        foreach (var @event in neighbors)
        {
            var newNeighbors = GetNeighbors(events, @event);

            if (newNeighbors.Length != 0)
            {
                AssignCluster(@event, newNeighbors, clusterId);
                ExpandCluster(events, newNeighbors, clusterId);
            }
        }
    }

    private static void AssignCluster(EventModel @event, IEnumerable<EventModel> neighbors, int clusterId)
    {
        @event.ClusterId = clusterId;

        foreach (var neighbor in neighbors)
        {
            neighbor.ClusterId = clusterId;
        }
    }

    private static EventModel[] GetNeighbors(EventModel[] events, EventModel @event)
    {
        return events
            .Where(other => other != @event && other.ClusterId == 0 &&
                HammingDistanceRatio(@event.SimHash1, other.SimHash1) >= epsilon1
                //&&
                //HammingDistanceRatio(@event.SimHash2, other.SimHash2) >= epsilon2
                //&&
                //HammingDistanceRatio(@event.SimHash3, other.SimHash3) >= epsilon3
                )
            .ToArray();
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

public static class DbscanSplitZlinq
{
    private const double epsilon1 = 0.70;
    private const double epsilon2 = 0.6;
    private const double epsilon3 = 0.85;

    public static void Clusterize(EventModel[] events)
    {
        var valueEvents = events.AsValueEnumerable();
        foreach (var @event in valueEvents) 
        {
            @event.SimHash1 = SimHashService.BitHackSplit2(@event.Text!);
            @event.SimHash2 = SimHashService.BitHackSplit2(@event.AlertKey!);
            @event.SimHash3 = SimHashService.BitHackSplit2(@event.CorrelationName!);
        }

        var clusterId = 1;
        
        foreach (var @event in valueEvents)
        {
            if (@event.ClusterId == 0)
            {
                var neighbors = valueEvents.Where(other => 
                    other != @event && 
                    other.ClusterId == 0 &&
                    CalculateJaccardRatio(@event.SimHash1, other.SimHash1) >= epsilon1 &&
                    CalculateJaccardRatio(@event.SimHash2, other.SimHash2) >= epsilon2 &&
                    CalculateJaccardRatio(@event.SimHash3, other.SimHash3) >= epsilon3);

                if (!neighbors.Any())
                {
                    @event.ClusterId = -1;
                    continue;
                }

                @event.ClusterId = clusterId;

                foreach (var neighbor in neighbors)
                {
                    neighbor.ClusterId = clusterId;
                }

                clusterId++;
            }
        }
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

public static class DbscanSplit
{
    private const double epsilon1 = 0.7;
    private const double epsilon2 = 0.6;
    private const double epsilon3 = 0.85;

    public static void Clusterize(EventModel[] events)
    {
        Parallel.ForEach(events, @event =>
        {
            @event.SimHash1 = SimHashService.BitHackSplit(@event.Text!);
            @event.SimHash2 = SimHashService.BitHackSplit(@event.AlertKey!);
            @event.SimHash3 = SimHashService.BitHackSplit(@event.CorrelationName!);
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
                AssignCluster(@event, neighbors, clusterId);
                //ExpandCluster(events, neighbors, clusterId);

                clusterId++;
            }
        }
    }

    private static void AssignCluster(EventModel @event, IEnumerable<EventModel> neighbors, int clusterId)
    {
        @event.ClusterId = clusterId;

        foreach (var neighbor in neighbors)
        {
            neighbor.ClusterId = clusterId;
        }
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

    private static IEnumerable<EventModel> GetNeighbors(EventModel @event, EventModel[] events)
    {
        foreach (var other in events)
        {
            if (other != @event && other.ClusterId == 0 &&
                HammingDistanceRatio(@event.SimHash1, other.SimHash1) >= epsilon1 &&
                HammingDistanceRatio(@event.SimHash2, other.SimHash2) >= epsilon2 &&
                HammingDistanceRatio(@event.SimHash3, other.SimHash3) >= epsilon3)
            {
                yield return other;
            }
        }
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }
}

public static class DbscanSplit128
{
    private const double epsilon1 = 0.70;
    private const double epsilon2 = 0.6;
    private const double epsilon3 = 0.85;

    public static void Clusterize(EventModel[] events)
    {
        //Parallel.ForEach(events, @event =>
        //{
        //    @event.SimHash128_1 = SimHashService.BitHackSplit128(@event.Text!);
        //    @event.SimHash128_2 = SimHashService.BitHackSplit128(@event.AlertKey!);
        //    @event.SimHash128_3 = SimHashService.BitHackSplit128(@event.CorrelationName!);
        //});

        foreach (var @event in events)
        {
            @event.SimHash128_1 = SimHashService.BitHackSplit128(@event.Time!);
            //@event.SimHash128_2 = SimHashService.BitHackSplit128(@event.AlertKey!);
            //@event.SimHash128_3 = SimHashService.BitHackSplit128(@event.CorrelationName!);
        }

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
                AssignCluster(@event, neighbors, clusterId);
                //ExpandCluster(events, neighbors, clusterId);

                clusterId++;
            }
        }
    }

    private static void AssignCluster(EventModel @event, IEnumerable<EventModel> neighbors, int clusterId)
    {
        @event.ClusterId = clusterId;

        foreach (var neighbor in neighbors)
        {
            neighbor.ClusterId = clusterId;
        }
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

    private static IEnumerable<EventModel> GetNeighbors(EventModel @event, EventModel[] events)
    {
        foreach (var other in events)
        {
            if (other != @event && other.ClusterId == 0 &&
                CalculateJaccardSimilarity(@event.SimHash128_1, other.SimHash128_1) >= epsilon1
                //&&
                //HammingDistanceRatio(@event.SimHash128_2, other.SimHash128_2) >= epsilon2 
                //&&
                //HammingDistanceRatio(@event.SimHash128_3, other.SimHash128_3) >= epsilon3
                )
            {
                yield return other;
            }
        }
    }

    private static double HammingDistanceRatio((ulong, ulong) hash1, (ulong, ulong) hash2)
    {
        ulong xor1 = hash1.Item1 ^ hash2.Item1;
        ulong xor2 = hash1.Item2 ^ hash2.Item2;

        var sum = BitOperations.PopCount(xor1) + BitOperations.PopCount(xor2);

        return 1 - ((double)sum / 128);
    }

    private static double CalculateJaccardSimilarity((ulong, ulong) hash1, (ulong, ulong) hash2)
    {
        var intersection1 = hash1.Item1 & hash2.Item1;
        var intersection2 = hash1.Item2 & hash2.Item2;

        var union1 = hash1.Item1 | hash2.Item1;
        var union2 = hash1.Item1 | hash2.Item1;

        var intersectionCount = BitOperations.PopCount(intersection1) + BitOperations.PopCount(intersection2);
        var unionCount = BitOperations.PopCount(union1) + BitOperations.PopCount(union2);

        return (double)intersectionCount / unionCount;
    }
}
