using BenchmarkDotNet.Attributes;
using Clusters.Hashing;

namespace Clusters.Benchmarks.Hashing;

[MemoryDiagnoser]
public class SimHashBenchmark
{

    [Benchmark]
    public void Simple()
    {
        SimHashService.DoSimple(StringHelper.AllSymbolsInput);
    }

    [Benchmark]
    public void BitHack()
    {
        SimHashService.DoBitHack();
    }

    [Benchmark]
    public void Unsafe()
    {
        SimHashService.DoUnsafe(StringHelper.AllSymbolsInput);
    }

    [Benchmark]
    public void Unrolled()
    {
        SimHashService.DoUnrolled(StringHelper.AllSymbolsInput);
    }

    //[Benchmark]
    //public void Unrolled()
    //{
    //    SimHashService.DoUnrolled();
    //}

    [Benchmark]
    public void Simd()
    {
        SimHashService.DoSIMD(StringHelper.AllSymbolsInput);
    }

    //[Benchmark]
    public void BitHackNoOverlap()
    {
        SimHashService.DoBitHackNonOverlapping();
    }

    //[Benchmark]
    public void BitHackSplit()
    {
        SimHashService.BitHackSplit(StringHelper.AllSymbolsInput);
    }

    //[Benchmark]
    public void BitHackSplit64()
    {
        SimHashService.BitHackSplit2(StringHelper.AllSymbolsInput);
    }

    //[Benchmark]
    public void BitHackSplit128()
    {
        SimHashService.BitHackSplit128(StringHelper.AllSymbolsInput);
    }

    //[Benchmark]
    public void Simd_Simpler()
    {
        SimHashService.DoSimdSimpler(StringHelper.AllSymbolsInput);
    }
}
