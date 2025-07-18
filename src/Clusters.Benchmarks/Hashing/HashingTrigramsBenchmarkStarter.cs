using System.IO.Hashing;
using System.Text;
using BenchmarkDotNet.Attributes;
using System.Security.Cryptography;

namespace Clusters.Benchmarks.Hashing;

[MemoryDiagnoser]
public class HashTrigramsBenchmark
{

    [Benchmark]
    public void GetHashCode_Algorithm()
    {
        trigram.GetHashCode();
    }

    [Benchmark]
    public void MD5_Algorithm()
    {
        HashMD5();
    }

    [Benchmark]
    public void XxHash64_Algorithm()
    {
        HashXx64();
    }

    [Benchmark]
    public void Fnv1a_Algorithm()
    {
        Fnv1a();
    }

    private const string trigram = "abc";

    public byte[] HashMD5()
    {
        Span<byte> buffer = stackalloc byte[trigram.Length * 2];
        Encoding.UTF8.GetBytes(trigram.AsSpan(), buffer);

        var slice = buffer[..3];
        return MD5.HashData(slice);
    }

    public byte[] HashXx64()
    {
        Span<byte> buffer = stackalloc byte[trigram.Length * 2];
        Encoding.UTF8.GetBytes(trigram.AsSpan(), buffer);

        var slice = buffer[..3];
        return XxHash64.Hash(slice);
    }
    
    private ulong Fnv1a()
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
}
