using BenchmarkDotNet.Attributes;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;
using Clusters.ML.Net;

namespace Clusters.Benchmarks.ML.Net;

[MemoryDiagnoser]
public class MlNetNaiveBenchmarkStarter
{
    private ClusterNaive _service;
    private ClusterTrigramsService _trigramService;
    private readonly EventModel[] _records;

    public MlNetNaiveBenchmarkStarter()
    {
        _service = new ClusterNaive();
        _trigramService = new ClusterTrigramsService();
        var reader = new CsvTextDataReader();
        _records = reader.ReadTextData("Data\\sample-full.csv", 0, 30_000);
    }

    [Benchmark]
    public void MlNet_KMeans_Naive_10() => _service.ClusterizeSingleField10(_records);

    [Benchmark]
    public void MlNet_KMeans_Naive_20() => _service.ClusterizeSingleField20(_records);

    [Benchmark]
    public void MlNet_KMeans_Naive_30() => _service.ClusterizeSingleField30(_records);
}
