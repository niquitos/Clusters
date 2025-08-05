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

    [Benchmark]
    public void BitHack()
    {
        SimHashService.DoBitHack();
    }

    //[Benchmark]
    public void UnrolledBitHack()
    {
        SimHashService.DoBitHackUnrolled();
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

    //[Benchmark]
    public void BitHackNoOverlap()
    {
        SimHashService.DoBitHackNonOverlapping();
    }

    [Benchmark]
    public void BitHackSplit()
    {
        SimHashService.BitHackSplit(StringHelper.AllSymbolsInput);
    }

    [Benchmark]
    public void BitHackSplit2()
    {
        SimHashService.BitHackSplit2(StringHelper.AllSymbolsInput);
    }

    //[Benchmark]
    public void Simd_Simpler()
    {
        SimHashService.DoSimdSimpler(StringHelper.AllSymbolsInput);
    }
}
