using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

namespace Clusters.Hashing;

public class HashingService
{
    const ulong offsetBasis = 14695981039346656037;
    const ulong prime = 1099511628211;

    public ulong Fnv1aHashSimpleFor(ReadOnlySpan<char> data)
    {
        var hash = offsetBasis;

        for (int i = 0; i < data.Length; i++)
        {
            hash ^= data[i];
            hash *= prime;
        }

        return hash;
    }

    public unsafe ulong Fnv1aHashUnsafe(ReadOnlySpan<char> data)
    {
        var hash = offsetBasis;

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

    public unsafe ulong Fnv1aHashSimd(ReadOnlySpan<char> data)
    {
        ulong hash = offsetBasis;

        fixed (char* start = data)
        {
            var ptr = start;

            var vector = Sse2.LoadVector128((ushort*)ptr);

            hash ^= vector.GetElement(0);
            hash *= prime;

            hash ^= vector.GetElement(1);
            hash *= prime;

            hash ^= vector.GetElement(2);
            hash *= prime;
        }

        return hash;
    }

    public unsafe ulong Fnv1aHashUnrolled(ReadOnlySpan<char> data)
    {
        ulong hash = offsetBasis;
        fixed (char* start = data)
        {
            char* ptr = start;

            hash ^= ptr[0];
            hash *= prime;

            hash ^= ptr[1];
            hash *= prime;

            hash ^= ptr[2];
            hash *= prime;
        }

        return hash;
    }
}