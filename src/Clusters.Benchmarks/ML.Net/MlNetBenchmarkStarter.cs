using BenchmarkDotNet.Attributes;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;
using Clusters.ML.Net;

namespace Clusters.Benchmarks.ML.Net;

[MemoryDiagnoser]
public class MlNetBenchmarkStarter
{
    private ClusteringService _mlservice;

    private readonly EventModel[] _records;

    public MlNetBenchmarkStarter()
    {
        _mlservice = new ClusteringService();
        var reader = new CsvTextDataReader();
        _records = reader.ReadTextData("Data\\sample-full.csv", 0, 30_000);
    }

    [Benchmark]
    public void MlNet_KMeans_Naive_10() => _mlservice.ClusterizeSingleFieldNaive10(_records);

    [Benchmark]
    public void MlNet_KMeans_Naive_20() => _mlservice.ClusterizeSingleFieldNaive20(_records);

    [Benchmark]
    public void MlNet_KMeans_Naive_30() => _mlservice.ClusterizeSingleFieldNaive30(_records);
}
