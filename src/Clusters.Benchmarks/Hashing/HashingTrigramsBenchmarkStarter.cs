using BenchmarkDotNet.Attributes;
using Clusters.Hashing;
using Service = Clusters.Hashing.TrigramHashingService;

namespace Clusters.Benchmarks.Hashing;

[MemoryDiagnoser]
public class HashingTrigramsBenchmarkStarter
{
    private readonly Service _service;
    private const string Input = "abc";

    public HashingTrigramsBenchmarkStarter()
    {
        _service = new Service();
    }

    [Benchmark]
    public void MD5() => _service.HashMD5(Input);

    [Benchmark]
    public void Xx64() => _service.HashXx64(Input);

    [Benchmark]
    public void Fnv1a_Simple() => _service.HashFnv1aSimple(Input);

    [Benchmark]
    public void Fnv1a_Simple_Trigram() => _service.HashFnv1aSimple(Input);

    [Benchmark]
    public void Fnv1a_Unsafe() => _service.HashFnv1aUnsafe(Input);

    [Benchmark]
    public void Fnv1a_Simd() => _service.HashFnv1aText(Input);

    [Benchmark]
    public void Fnv1a_Trigrams() => _service.HashFnv1aTrigram(Input);

    [Benchmark]
    public void GetHashCode_Substrings() => _service.HashGetHashCodeSubstrings(Input);

    [Benchmark]
    public void GetHashCode_Spans() => _service.HashGetHashCodeSpans(Input);
}
