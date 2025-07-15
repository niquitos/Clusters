using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
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
        Fnv1aUnrolled();
    }

    [Benchmark]
    public void SIMD()
    {
        Fnv1aSimd();
    }

    private const string trigram = "abc";
    private ulong Fnv1aSimple()
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

        var hash = offsetBasis;

        for (int i = 0; i < trigram.Length; i++)
        {
            hash ^= trigram[i];
            hash *= prime;
        }

        return hash;
    }

    private ulong Fnv1aUnrolled()
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

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
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

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
    public unsafe ulong Fnv1aSimd()
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

        var hash = offsetBasis;

        fixed (char* start = trigram)
        {
            var ptr = start;
            var end = start + trigram.Length;

            if (Sse2.IsSupported)
            {
                // Process 8 chars at a time with SSE2
                while (ptr <= end - 8)
                {
                    var vector = Sse2.LoadVector128((ushort*)ptr);
                    for (int i = 0; i < 8; i++)
                    {
                        hash ^= vector.GetElement(i);
                        hash *= prime;
                    }
                    ptr += 8;
                }
            }

            // Process remaining characters (if any)
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
