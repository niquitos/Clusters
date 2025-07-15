using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

namespace Clusters.Hashing;

public class HashingService
{
    public ulong ComputeFnv1aHashSimpleFor(ReadOnlySpan<char> data)
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

        var hash = offsetBasis;

        for (int i = 0; i < data.Length; i++)
        {
            hash ^= data[i];
            hash *= prime;
        }

        return hash;
    }

    public ulong ComputeFnv1aHashSimpleTrigram(ReadOnlySpan<char> data)
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

        ulong hash = offsetBasis;

        hash ^= data[0];
        hash *= prime;

        hash ^= data[1];
        hash *= prime;

        hash ^= data[2];
        hash *= prime;

        return hash;
    }

    public unsafe ulong ComputeFnv1aHashUnsafe(ReadOnlySpan<char> data)
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

        ulong hash = offsetBasis;

        fixed (char* start = data)
        {
            var ptr = start;
            var end = ptr + data.Length;

            while (ptr < end)
            {
                hash ^= *ptr;
                hash *= prime;
                ptr++;
            }
        }

        return hash;
    }

    public unsafe ulong ComputeFnv1aHashSimd(ReadOnlySpan<char> data)
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

        var hash = offsetBasis;

        fixed (char* start = data)
        {
            var ptr = start;
            var end = start + data.Length;

            if (Avx2.IsSupported)
            {
                // Process 16 chars at a time with AVX2
                while (ptr <= end - 16)
                {
                    var vector = Avx2.LoadVector256((ushort*)ptr);
                    for (int i = 0; i < 16; i++)
                    {
                        hash ^= vector.GetElement(i);
                        hash *= prime;
                    }
                    ptr += 16;
                }
            }
            else if (Sse2.IsSupported)
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

    public unsafe ulong ComputeFnv1aUnsafeTrigramHash(ReadOnlySpan<char> data)
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;

        fixed (char* ptr = data)
        {
            ulong hash = offsetBasis;

            hash ^= ptr[0];
            hash *= prime;

            hash ^= ptr[1];
            hash *= prime;

            hash ^= ptr[2];
            hash *= prime;

            return hash;
        }
    }
}