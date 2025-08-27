using Clusters.Data.Models;
using Clusters.Hashing;
using System.Numerics;

namespace Clusters.Clusterization;

public static class DbscanInlined
{
    private const double epsilon1 = 0.7;
    private const double epsilon2 = 0.6;
    private const double epsilon3 = 0.85;

    public static void Clusterize(EventModel[] events)
    {
        Parallel.ForEach(events, @event =>
        {
            @event.SimHash1 = SimHashService.DoSIMD(@event.Text!);
            @event.SimHash2 = SimHashService.DoSIMD(@event.AlertKey!);
            @event.SimHash3 = SimHashService.DoSIMD(@event.CorrelationName!);
        });

        var clusterId = 1;

        for (int i = 0; i < events.Length; i++)
        {
            if (events[i].ClusterId == 0)
            {
                var neighborsCount = 0;

                for (int j = i + 1; j < events.Length; j++)
                {
                    if (events[j].ClusterId != 0)
                        continue;

                    bool isNeighbor =
                        HammingDistanceRatio(events[i].SimHash1, events[j].SimHash1) >= epsilon1 &&
                        HammingDistanceRatio(events[i].SimHash2, events[j].SimHash2) >= epsilon2 &&
                        HammingDistanceRatio(events[i].SimHash3, events[j].SimHash3) >= epsilon3;

                    if (isNeighbor)
                    {
                        events[j].ClusterId = clusterId;
                        neighborsCount++;
                    }
                }

                events[i].ClusterId = neighborsCount > 0 ? clusterId : -1;

                clusterId++;
            }
        }
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }
}
