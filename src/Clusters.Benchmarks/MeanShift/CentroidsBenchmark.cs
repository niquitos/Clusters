using BenchmarkDotNet.Attributes;
using Clusters.Clusterization.MeanShift;
using Clusters.Data.DataAccess;
using Clusters.Hashing;

namespace Clusters.Benchmarks.MeanShift;

[MemoryDiagnoser]
public class CentroidsBenchmark
{
    private List<ulong> _data;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData("Data\\sample-full.csv", 0, 30_000).ToList();

        Parallel.ForEach(records, @event =>
        {
            @event.SimHash1 = SimHashService.DoSIMD(@event.Text!);
            //@event.SimHash2 = SimHashService.DoSIMD(@event.AlertKey!);
            //@event.SimHash3 = SimHashService.DoSIMD(@event.CorrelationName!);
        });

        _data = records.Select(x => x.SimHash1).ToList();
    }


    [Benchmark]
    public void Simple()
    {
        CentroidsCalculator.Simple(_data);
    }

    [Benchmark]
    public void SIMD()
    {
        CentroidsCalculator.SIMD(_data);
    }

    [Benchmark]
    public void InParallel()
    {
        CentroidsCalculator.InParallel(_data);
    }
}
