using BenchmarkDotNet.Attributes;

namespace Clusters.Benchmarks.Hashing;

[MemoryDiagnoser]
public class Fnv1aBenchmark
{

    [Benchmark]
    public void Simple()
    {
        Fnv1aSimple();
    }

    [Benchmark]
    public void Unrolled()
    {
        Fnv1aUnrolled();
    }

    [Benchmark]
    public void Unsafe()
    {
        Fnv1aUnsafe();
    }

    private const string trigram = "abc";
    const ulong offsetBasis = 14695981039346656037;
    const ulong prime = 1099511628211;

    private ulong Fnv1aSimple()
    {
        var hash = offsetBasis;

        for (var i = 0; i < trigram.Length; i++)
        {
            hash ^= trigram[i];
            hash *= prime;
        }

        return hash;
    }

    private ulong Fnv1aUnrolled()
    {
        var hash = offsetBasis;

        hash ^= trigram[0];
        hash *= prime;

        hash ^= trigram[1];
        hash *= prime;

        hash ^= trigram[2];
        hash *= prime;

        return hash;
    }

    public unsafe ulong Fnv1aUnsafe()
    {
        ulong hash = offsetBasis;

        fixed (char* start = trigram)
        {
            var ptr = start;
            var end = ptr + trigram.Length;

            while (ptr < end)
            {
                hash ^= *ptr;
                hash *= prime;
                ptr++;
            }
        }

        return hash;
    }
}
