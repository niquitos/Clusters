using System;
using BenchmarkDotNet.Attributes;

namespace Clusters.Benchmarks.Trigrams;

[MemoryDiagnoser]

public class TrigramsBenchmark
{
    [Benchmark]
    public int Substring()
    {
        return DoSubstrings();
    }

    [Benchmark]
    public int Slice()
    {
        return DoSlice();
    }

    private int DoSubstrings()
    {
        var sum = 0;
        for (int i = 0; i < StringHelper.AllSymbolsInput.Length - 2; i++)
        {
            var substring = StringHelper.AllSymbolsInput.Substring(i, 3);
            sum += substring[0];//если не использовать результат, то Jit может оптимизировать все тело цикла
        }

        return sum;
    }

    private int DoSlice()
    {
        var span = StringHelper.AllSymbolsInput.AsSpan();
        var sum = 0;
        for (int i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            sum += slice[0];//если не использовать результат, то Jit может оптимизировать все тело цикла
        }

        return sum;
    }
}
