using BenchmarkDotNet.Attributes;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;
using Clusters.ML.Net;

namespace Clusters.Benchmarks.ML.Net;

[MemoryDiagnoser]
public class MlNetTrigramBenchmarkStarter
{
    private ClusterTrigramsService _service;
    private readonly EventModel[] _records;

    public MlNetTrigramBenchmarkStarter()
    {
        _service = new ClusterTrigramsService();
        var reader = new CsvTextDataReader();
        _records = reader.ReadTextData("Data\\sample-full.csv", 0, 30_000);
    }

    [Benchmark]
    public void MlNet_Trigram_10() => _service.ClusterizeSingleField10(_records);

    [Benchmark]
    public void MlNet_Trigram_20() => _service.ClusterizeSingleField20(_records);

    [Benchmark]
    public void MlNet_Trigram_30() => _service.ClusterizeSingleField30(_records);
}
