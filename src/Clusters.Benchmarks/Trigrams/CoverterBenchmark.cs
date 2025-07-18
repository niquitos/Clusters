using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Clusters.Benchmarks.Trigrams;

[MemoryDiagnoser]
public class CoverterBenchmark
{


    [Benchmark]
    public void GetBytes_Simple()
    {
        GetBytes();
    }

    [Benchmark]
    public void GetBytes_SpanBuffer()
    {
        DoSpanBuffer();
    }

    const string trigram = "abc";

    private void GetBytes()
    {
        var bytes = Encoding.UTF8.GetBytes(trigram);
    }

    private void DoSpanBuffer()
    {
        Span<byte> buffer = stackalloc byte[3];
        Encoding.UTF8.GetBytes(trigram.AsSpan(), buffer);
    }
}
