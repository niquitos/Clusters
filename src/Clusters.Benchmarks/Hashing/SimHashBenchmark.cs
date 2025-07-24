using BenchmarkDotNet.Attributes;
using Clusters.Hashing;

namespace Clusters.Benchmarks.Hashing;

[MemoryDiagnoser]
public class SimHashBenchmark
{

    //[Benchmark]
    public void Simple()
    {
        SimHashService.DoSimple();
    }

    //[Benchmark]
    public void Unrolled()
    {
        SimHashService.DoUnrolled();
    }

    [Benchmark]
    public void Simd()
    {
        SimHashService.DoSimd(StringHelper.AllSymbolsInput);
    }

    [Benchmark]
    public void Simd_Simpler()
    {
        SimHashService.DoSimdSimpler(StringHelper.AllSymbolsInput);
    }
}
