using BenchmarkDotNet.Attributes;
using Clusters.Hashing;
using System;
using System.Text;

namespace Clusters.Benchmarks.Hashing;

[MemoryDiagnoser]
public class HashTrigramsBenchmark
{
    private static TrigramHashingService _hashingService = new();
    private const string _trigram = "abc";
    private readonly byte[] data = Encoding.UTF8.GetBytes(_trigram);

    [Benchmark]
    public void XxHash64()
    {
        _hashingService.HashXx64(data);
    }


    [Benchmark]
    public void Murmur3()
    {
        _hashingService.HashMurmur(data);
    }

    [Benchmark]
    public void Fnv1a()
    {
        _hashingService.HashFnv1aSimpleFor(_trigram);
    }
}
