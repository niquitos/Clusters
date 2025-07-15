using System;
using BenchmarkDotNet.Attributes;
using Clusters.Clusterization;
using Clusters.Data.DataAccess;
using Clusters.Data.Models;

namespace Clusters.Benchmarks;

[MemoryDiagnoser]
public class DbscanBenchmark
{
    private List<EventModel> _originalData = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var reader = new CsvTextDataReader();
        _originalData = [.. reader.ReadTextData("Data\\sample-full.csv", 0, 30_000)];
    }

    [Benchmark]
    public void Classic()
    {
        List<EventModel> records = [.. _originalData.Select(r => new EventModel
        {
            Text = r.Text
        })];

        DbscanClassicService.Clusterize(records);
    }

    [Benchmark]
    public void Parallel_Distance()
    {
        List<EventModel> records = [.. _originalData.Select(r => new EventModel
        {
            Text = r.Text
        })];

        DbscanParallelDistanceService.Clusterize(records);
    }

    [Benchmark]
    public void BitArray()
    {
        List<EventModel> records = [.. _originalData.Select(r => new EventModel
        {
            Text = r.Text
        })];

        DbscanBitArrayService.Clusterize([.. records]);
    }

    [Benchmark]
    public void Spans()
    {
        List<EventModel> records = [.. _originalData.Select(r => new EventModel
        {
            Text = r.Text
        })];

        DbscanSpansService.Clusterize([.. records]);
    }
}
