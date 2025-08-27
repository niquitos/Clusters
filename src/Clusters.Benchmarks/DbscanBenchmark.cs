using BenchmarkDotNet.Attributes;
using Clusters.Clusterization;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;
using Clusters.Data.Models.Domain;
using Clusters.Data.Models.Domain.Services;
using Newtonsoft.Json.Linq;

namespace Clusters.Benchmarks;

[MemoryDiagnoser]
public class DbscanBenchmark
{
    private List<EventModel> _originalData = null!;
    private List<EventModel> _records = null!;
    private List<ClusterEvent> _domainRecords = null!;
    private ClusterizationOperation _operation = null!;
    private ClusterizationService _service;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var reader = new CsvTextDataReader();
        _originalData = [.. reader.ReadTextData("Data\\sample-full.csv", 0, 30_000)];

        var domainReader = new DomainCsvTextDataReader();
        _domainRecords = MapToDomainEvents(_originalData);

        _operation = new ClusterizationOperation(
            new ClusterizationCriteria(
                [
                    new FieldSimilarity("text", 70),
                    new FieldSimilarity("alert.key", 60),
                    new FieldSimilarity("correlation_name", 85)
                ]),
                [new SortCriteria("text", true)],
               new ClustersLimit(1000));

        _service = new ClusterizationService(_operation);
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

        _domainRecords.ForEach(x => x.SetClusterId(Guid.Empty));
    }


    [Benchmark]
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
    public void Inlined()
    {
        DbscanInlined.Clusterize([.. _records]);
    }

    //[Benchmark]
    public void Split()
    {
        DbscanSplit2.Clusterize([.. _records]);
    }

    [Benchmark]
    public void Domain()
    {
        _service.Clusterize(_domainRecords);
    }

    //[Benchmark]
    public void Zlinq()
    {
        DbscanSplitZlinq.Clusterize([.. _records]);
    }

    //[Benchmark]
    public void Split128()
    {
        DbscanSplit128.Clusterize([.. _records]);
    }

    //[Benchmark]
    public void Test()
    {
        DbscanTest.Clusterize([.. _records]);
    }

    private List<ClusterEvent> MapToDomainEvents(IEnumerable<EventModel> events)
    {
        var jArray = new JArray();
        foreach (var @event in events)
        {
            var jObject = new JObject()
            {
                ["text"] = @event.Text,
                ["event_src.host"] = @event.EventSrcHost,
                ["alert.key"] = @event.AlertKey,
                ["correlation_name"] = @event.CorrelationName,
            };

            jArray.Add(jObject);
        }

        return [.. jArray.Select(j => new ClusterEvent(j))];
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
