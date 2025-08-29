using BenchmarkDotNet.Attributes;
using Clusters.Clusterization.MeanShift;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;
using Clusters.Data.Models.Domain;
using Clusters.Data.Models.Domain.Services;
using Newtonsoft.Json.Linq;

namespace Clusters.Benchmarks.MeanShift;

[MemoryDiagnoser]
public class MeanShiftBenchmark
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
        MeanShiftClassic.Clusterize([.. _records]);
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
