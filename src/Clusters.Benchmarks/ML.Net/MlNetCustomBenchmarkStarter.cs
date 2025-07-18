using BenchmarkDotNet.Attributes;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;
using Clusters.ML.Net;

namespace Clusters.Benchmarks.ML.Net;

[MemoryDiagnoser]
public class MlNetCustomBenchmarkStarter
{
    private ClusterCustomMapping _service;
    private readonly EventModel[] _records;

    public MlNetCustomBenchmarkStarter()
    {
        _service = new ClusterCustomMapping();
        var reader = new CsvTextDataReader();
        _records = reader.ReadTextData("Data\\sample-full.csv", 0, 30_000);
    }

    [Benchmark]
    public void MlNet_KMeans_Custom_10() => _service.ClusterizeSingleField10(_records);

    [Benchmark]
    public void MlNet_KMeans_Custom_20() => _service.ClusterizeSingleField20(_records);

    [Benchmark]
    public void MlNet_KMeans_Custom_30() => _service.ClusterizeSingleField30(_records);
}
