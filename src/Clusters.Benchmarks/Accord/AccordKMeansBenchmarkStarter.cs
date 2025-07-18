using BenchmarkDotNet.Attributes;
using Clusters.Accord;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;

namespace Clusters.Benchmarks.Accord;

[MemoryDiagnoser]
public class AccordKMeansBenchmarkStarter
{
    private ClusterKMeansAccordService _service;
    private readonly EventModel[] _records;

    public AccordKMeansBenchmarkStarter()
    {
        _service = new ClusterKMeansAccordService();
        var reader = new CsvTextDataReader();
        _records = reader.ReadTextData("Data\\sample-full.csv");
    }

    [Benchmark]
    public void Accord_KMeans_10() => _service.ClusterizeSingleField10(_records);

    [Benchmark]
    public void Accord_KMeans_20() => _service.ClusterizeSingleField20(_records);

    [Benchmark]
    public void Accord_KMeans_30() => _service.ClusterizeSingleField30(_records);
}
