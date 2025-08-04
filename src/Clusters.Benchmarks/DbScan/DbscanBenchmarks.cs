using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;
using System.IO.Hashing;
using System.Numerics;
using System.Text;

namespace Clusters.Benchmarks.DbScan;

[MemoryDiagnoser]
[MediumRunJob(RuntimeMoniker.Net80, BenchmarkDotNet.Environments.Jit.RyuJit, Platform.X64)]
public class DbscanBenchmarks
{
    private List<EventModel> _originalData = null!;
    private List<EventModel> _records = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var reader = new CsvTextDataReader();
        _originalData = [.. reader.ReadTextData("Data\\sample-full.csv", 0, 30_000)];
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _records = [.. _originalData.Select(r => new EventModel
        {
            Time = r.Time,
            EventSrcHost = r.EventSrcHost,
            Text = r.Text,
            CorrelationName = r.CorrelationName,
            AlertKey = r.AlertKey
        })];
    }

    [Benchmark]
    public void Naive()
    {
        DbscanNaive.Clusterize([.. _records]);
    }

}

public static class DbscanNaive
{
    private const double epsilon1 = 0.7;
    private const double epsilon2 = 0.6;
    private const double epsilon3 = 0.85;

    public static void Clusterize(EventModel[] events)
    {
        foreach (var e in events)
        {
            e.SimHash1 = CalculateSimHash(e.Text!);
            e.SimHash2 = CalculateSimHash(e.AlertKey!);
            e.SimHash3 = CalculateSimHash(e.CorrelationName!);
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
                ExpandCluster(events, neighbors, clusterId);

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
        return events.Where(other =>
            other != @event
            && other.ClusterId == 0
            && HammingDistanceRatio(@event.SimHash1, other.SimHash1) >= epsilon1
            && HammingDistanceRatio(@event.SimHash2, other.SimHash2) >= epsilon2
            && HammingDistanceRatio(@event.SimHash3, other.SimHash3) >= epsilon3);
    }

    public static ulong CalculateSimHash(string input)
    {
        //Normalize
        if (input is null)
            return 0;

        var text = input.Trim();

        if (text.Length == 0)
            return 0;

        //Featurize
        Span<int> shingle = stackalloc int[64];
        var span = input.AsSpan();

        for (var i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeHashXx64(slice);

            for (var j = 0; j < 64; j++)
            {
                var bit1 = (hashCode & (1UL << j)) != 0;

                shingle[j] += bit1 ? 1 : -1;
            }
        }

        ulong simhash = 0L;
        for (var i = 0; i < 64; i++)
        {
            if (shingle[i] > 0)
            {
                simhash |= 1UL << i;
            }
        }

        return simhash;
    }

    public static ulong ComputeHashXx64(ReadOnlySpan<char> trigram)
    {
        Span<byte> buffer = stackalloc byte[trigram.Length * 2];
        Encoding.UTF8.GetBytes(trigram, buffer);

        var slice = buffer[..3];

        return XxHash64.HashToUInt64(slice);
    }

    private static double HammingDistanceRatio(ulong hash1, ulong hash2)
    {
        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }
}
