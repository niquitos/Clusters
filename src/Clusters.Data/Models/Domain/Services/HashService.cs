using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Clusters.Data.Models.Domain.Services;

public static class HashService
{
    private const ulong offsetBasis = 14695981039346656037;
    private const ulong prime = 1099511628211;

    private static readonly HashSet<char> _delimeters = [' ', '\\', '/', '@', '"', '.', '|'];

    public static ulong CalculateSimhash(string input)
    {
        if (input is null)
            return 0;

        var text = input.Trim();

        if (text.Length == 0)
            return 0;

        Span<int> shingle = stackalloc int[64];

        var span = text.ToLower().AsSpan();
        var start = 0;

        for (var i = 0; i <= span.Length; i++)
        {
            if (i != span.Length && !_delimeters.Contains(span[i]))
                continue;

            if (start < i)
            {
                var token = span[start..i];
                var hashCode = ComputeFnv1aHash(token);

                for (var j = 0; j < 64; j++)
                {
                    shingle[j] += ((int)((hashCode >> j) & 1) * 2) - 1;
                }
            }
            start = i + 1;
        }

        ulong simhash = 0L;
        for (var i = 0; i < 64; i++)
        {
            if (shingle[i] > 0)
            {
                simhash |= 1UL << i;
            }
        }

        return simhash;
    }

    private static ulong ComputeFnv1aHash(ReadOnlySpan<char> text)
    {
        ulong hash = offsetBasis;

        foreach (var ch in text)
        {
            hash ^= ch;
            hash *= prime;
        }

        return hash;
    }

    public static ulong CalculateSimHashTrigramSIMD(string input)
    {
        if (input is null)
            return 0;

        var text = input.Trim();

        if (text.Length == 0)
            return 0;

        Span<int> shingles = stackalloc int[64];
        var span = text.ToLower().AsSpan();

        return Avx2.IsSupported
            ? ComputeAvx(shingles, span)
            : ComputeBitHack(shingles, span);
    }

    public static unsafe ulong DoSIMD(string input)
    {
        if (input is null)
            return 0;

        var text = input.Trim();

        if (text.Length == 0)
            return 0;

        Span<int> shingle = stackalloc int[64];
        var span = text.AsSpan();

        if (Avx2.IsSupported)
            return ComputeAvx(shingle, span);
        else
            return ComputeBitHack(shingle, span);
    }

    private static unsafe ulong ComputeAvx(Span<int> shingle, ReadOnlySpan<char> span)
    {
        var batchSize = Vector256<int>.Count;
        var accumulators = stackalloc Vector256<int>[batchSize];
        for (int i = 0; i < span.Length - 2; i++)
        {
            var hashCode = ComputeFnv1aTrigramHash(span.Slice(i, 3));
            for (var j = 0; j < batchSize; j++)
            {
                var bits = (hashCode >> (j * batchSize)) & 0b11111111;
                var bitMask = Vector256.Create((int)((bits >> 0) & 1), (int)((bits >> 1) & 1), (int)((bits >> 2) & 1), (int)((bits >> 3) & 1),
                                               (int)((bits >> 4) & 1), (int)((bits >> 5) & 1), (int)((bits >> 6) & 1), (int)((bits >> 7) & 1)
                );

                var update = Avx2.Subtract(Avx2.ShiftLeftLogical(bitMask, 1), Vector256<int>.One);
                accumulators[j] = Avx2.Add(accumulators[j], update);
            }
        }

        for (var i = 0; i < batchSize; i++)
        {
            for (var j = 0; j < batchSize; j++)
            {
                var bitPos = (i * batchSize) + j;
                shingle[bitPos] += accumulators[i].GetElement(j);
            }
        }

        var simhash = 0UL;
        for (var i = 0; i < 64; i++)
        {
            if (shingle[i] > 0)
            {
                simhash |= 1UL << i;
            }
        }
        return simhash;
    }

    public static ulong ComputeBitHack(Span<int> shingle, ReadOnlySpan<char> span)
    {
        for (var i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aTrigramHash(slice);

            for (var j = 0; j < 64; j += 4)
            {
                shingle[j] += ((int)((hashCode & (1UL << j)) >> j) * 2) - 1;
                shingle[j + 1] += ((int)((hashCode & (1UL << (j + 1))) >> (j + 1)) * 2) - 1; 
                shingle[j + 2] += ((int)((hashCode & (1UL << (j + 2))) >> (j + 2)) * 2) - 1; 
                shingle[j + 3] += ((int)((hashCode & (1UL << (j + 3))) >> (j + 3)) * 2) - 1; 
            }
        }

        ulong simhash = 0L;
        for (var i = 0; i < 64; i++)
        {
            if (shingle[i] > 0)
            {
                simhash |= 1UL << i;
            }
        }

        return simhash;
    }

    private static ulong ComputeFnv1aTrigramHash(ReadOnlySpan<char> trigram)
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


}