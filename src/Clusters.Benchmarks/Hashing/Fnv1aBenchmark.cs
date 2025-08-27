using BenchmarkDotNet.Attributes;
using Clusters.Hashing;

namespace Clusters.Benchmarks.Hashing;

[MemoryDiagnoser]
public class Fnv1aBenchmark
{
    private static TrigramHashingService _hashingService = new();

    [Benchmark]
    public void SimpleFor()
    {
        _hashingService.HashFnv1aSimpleFor("abc");
    }

    [Benchmark]
    public void Unsafe()
    {
        _hashingService.HashFnv1aUnsafe("abc");
    }

    [Benchmark]
    public void Unrolled()
    {
        _hashingService.HashFnv1aUnrolled("abc");
    }

    [Benchmark]
    public void SIMD()
    {
        _hashingService.Fnv1aHashSimd("abc");
    }
}
