using Clusters.Data.Models;
using Clusters.Hashing;
using System.Numerics;

namespace Clusters.Clusterization.MeanShift;

public static class MeanShiftClassic
{
    private const int _bandwidth = 10;


    // Mean Shift clustering
    public static void Clusterize(EventModel[] events)
    {
        Parallel.ForEach(events, @event =>
        {
            @event.SimHash1 = SimHashService.DoSIMD(@event.Text!);
            //@event.SimHash2 = SimHashService.DoSIMD(@event.AlertKey!);
            //@event.SimHash3 = SimHashService.DoSIMD(@event.CorrelationName!);
        });

        var data = events.Select(x => x.SimHash1);
        var centroids = new HashSet<ulong>(data);
        var convergedCentroids = new HashSet<ulong>();

        var iterationsCount = 0;

        while (centroids.Count > 0)
        {
            var newCentroids = new HashSet<ulong>();

            foreach (var centroid in centroids)
            {
                var neighbors = data.Where(hash => HammingDistanceRatio(centroid, hash) <= _bandwidth).ToList();

                if (neighbors.Count == 0)
                    continue;

                ulong newCentroid = CentroidsCalculator.SIMD(neighbors);

                newCentroids.Add(newCentroid);
            }

            if (newCentroids.SetEquals(centroids) || iterationsCount > 100)
            {
                convergedCentroids.UnionWith(newCentroids);
                break;
            }

            centroids = newCentroids;
            iterationsCount++;
        }

        var clusterId = 0;

        foreach (var centroid in convergedCentroids)
        {
            var clusterPoints = events.Where(e => e.ClusterId == 0 && HammingDistanceRatio(centroid, e.SimHash1) <= _bandwidth).ToList();

            clusterPoints.ForEach(x => x.ClusterId = clusterId);
            clusterId++;
        }
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        var xorResult = hash1 ^ hash2;
        return BitOperations.PopCount(xorResult);
    }
}
