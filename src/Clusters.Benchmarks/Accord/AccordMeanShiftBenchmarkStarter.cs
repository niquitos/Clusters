using BenchmarkDotNet.Attributes;
using Clusters.Accord;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;

namespace Clusters.Benchmarks.Accord;

[MemoryDiagnoser]
public class AccordMeanShiftBenchmarkStarter
{
    private ClusterMeanShiftAccordService _service;
    private readonly EventModel[] _records;

    public AccordMeanShiftBenchmarkStarter()
    {
        _service = new ClusterMeanShiftAccordService();
        var reader = new CsvTextDataReader();
        _records = reader.ReadTextData("Data\\sample-full.csv");
    }

    [Benchmark]
    public void Accord_MeanShift() => _service.ClusterizeSingleField(_records);
}
