using BenchmarkDotNet.Attributes;
using Clusters.Clusterization;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;

namespace Clusters.Benchmarks;

[MemoryDiagnoser]
public class DbscanBenchmark
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


    //[Benchmark]
    public void Classic()
    {
        DbscanClassic.Clusterize([.. _records]);
    }

    //[Benchmark]
    public void Array()
    {
        DbscanArray.Clusterize([.. _records]);
    }

    //[Benchmark]
    public void Parallel()
    {
        DbscanParallel.Clusterize([.. _records]);
    }

    //[Benchmark]
    public void YieldReturn()
    {
        DbscanYield.Clusterize([.. _records]);
    }

    [Benchmark]
    public void No_Recursion()
    {
        DbscanNoRecursion.Clusterize([.. _records]);
    }

    [Benchmark]
    public void Split()
    {
        DbscanSplit2.Clusterize([.. _records]);
    }

    //[Benchmark]
    public void Test()
    {
        DbscanTest.Clusterize([.. _records]);
    }
}

[MemoryDiagnoser]
public class DbscanParametrizedBenchmark
{
    [Params(50_000, 100_000, 200_000, 400_000, 800_000, 1_600_000, 3_200_000)]
    public int Number;

    private List<EventModel> _originalData = null!;
    private List<EventModel> _records = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var reader = new CsvTextDataReader();
        _originalData = [.. reader.ReadTextData("Data\\sample-full.csv", 0, Number)];
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
    public void Classic()
    {
        DbscanClassic.Clusterize([.. _records]);
    }
}
