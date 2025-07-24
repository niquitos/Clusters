using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Clusters.Hashing;

public static class SimHashService
{
    private const ulong offsetBasis = 14695981039346656037;
    private const ulong prime = 1099511628211;

    public static ulong DoSimple()
    {
        Span<int> shingle = stackalloc int[64];
        var span = StringHelper.AllSymbolsInput.AsSpan();

        for (var i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);

            for (var j = 0; j < 64; j++)
            {
                var bit1 = (hashCode & (1UL << j)) != 0;

                shingle[j] += bit1 ? 1 : -1;
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

    public static ulong DoUnrolled()
    {
        Span<int> shingle = stackalloc int[64];
        var span = StringHelper.AllSymbolsInput.AsSpan();

        for (var i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);

            shingle[0] += ((hashCode & (1UL << 0)) != 0) ? 1 : -1;
            shingle[1] += ((hashCode & (1UL << 1)) != 0) ? 1 : -1;
            shingle[2] += ((hashCode & (1UL << 2)) != 0) ? 1 : -1;
            shingle[3] += ((hashCode & (1UL << 3)) != 0) ? 1 : -1;
            shingle[4] += ((hashCode & (1UL << 4)) != 0) ? 1 : -1;
            shingle[5] += ((hashCode & (1UL << 5)) != 0) ? 1 : -1;
            shingle[6] += ((hashCode & (1UL << 6)) != 0) ? 1 : -1;
            shingle[7] += ((hashCode & (1UL << 7)) != 0) ? 1 : -1;
            shingle[8] += ((hashCode & (1UL << 8)) != 0) ? 1 : -1;
            shingle[9] += ((hashCode & (1UL << 9)) != 0) ? 1 : -1;
            shingle[10] += ((hashCode & (1UL << 10)) != 0) ? 1 : -1;
            shingle[11] += ((hashCode & (1UL << 11)) != 0) ? 1 : -1;
            shingle[12] += ((hashCode & (1UL << 12)) != 0) ? 1 : -1;
            shingle[13] += ((hashCode & (1UL << 13)) != 0) ? 1 : -1;
            shingle[14] += ((hashCode & (1UL << 14)) != 0) ? 1 : -1;
            shingle[15] += ((hashCode & (1UL << 15)) != 0) ? 1 : -1;
            shingle[16] += ((hashCode & (1UL << 16)) != 0) ? 1 : -1;
            shingle[17] += ((hashCode & (1UL << 17)) != 0) ? 1 : -1;
            shingle[18] += ((hashCode & (1UL << 18)) != 0) ? 1 : -1;
            shingle[19] += ((hashCode & (1UL << 19)) != 0) ? 1 : -1;
            shingle[20] += ((hashCode & (1UL << 20)) != 0) ? 1 : -1;
            shingle[21] += ((hashCode & (1UL << 21)) != 0) ? 1 : -1;
            shingle[22] += ((hashCode & (1UL << 22)) != 0) ? 1 : -1;
            shingle[23] += ((hashCode & (1UL << 23)) != 0) ? 1 : -1;
            shingle[24] += ((hashCode & (1UL << 24)) != 0) ? 1 : -1;
            shingle[25] += ((hashCode & (1UL << 25)) != 0) ? 1 : -1;
            shingle[26] += ((hashCode & (1UL << 26)) != 0) ? 1 : -1;
            shingle[27] += ((hashCode & (1UL << 27)) != 0) ? 1 : -1;
            shingle[28] += ((hashCode & (1UL << 28)) != 0) ? 1 : -1;
            shingle[29] += ((hashCode & (1UL << 29)) != 0) ? 1 : -1;
            shingle[30] += ((hashCode & (1UL << 30)) != 0) ? 1 : -1;
            shingle[31] += ((hashCode & (1UL << 31)) != 0) ? 1 : -1;
            shingle[32] += ((hashCode & (1UL << 32)) != 0) ? 1 : -1;
            shingle[33] += ((hashCode & (1UL << 33)) != 0) ? 1 : -1;
            shingle[34] += ((hashCode & (1UL << 34)) != 0) ? 1 : -1;
            shingle[35] += ((hashCode & (1UL << 35)) != 0) ? 1 : -1;
            shingle[36] += ((hashCode & (1UL << 36)) != 0) ? 1 : -1;
            shingle[37] += ((hashCode & (1UL << 37)) != 0) ? 1 : -1;
            shingle[38] += ((hashCode & (1UL << 38)) != 0) ? 1 : -1;
            shingle[39] += ((hashCode & (1UL << 39)) != 0) ? 1 : -1;
            shingle[40] += ((hashCode & (1UL << 40)) != 0) ? 1 : -1;
            shingle[41] += ((hashCode & (1UL << 41)) != 0) ? 1 : -1;
            shingle[42] += ((hashCode & (1UL << 42)) != 0) ? 1 : -1;
            shingle[43] += ((hashCode & (1UL << 43)) != 0) ? 1 : -1;
            shingle[44] += ((hashCode & (1UL << 44)) != 0) ? 1 : -1;
            shingle[45] += ((hashCode & (1UL << 45)) != 0) ? 1 : -1;
            shingle[46] += ((hashCode & (1UL << 46)) != 0) ? 1 : -1;
            shingle[47] += ((hashCode & (1UL << 47)) != 0) ? 1 : -1;
            shingle[48] += ((hashCode & (1UL << 48)) != 0) ? 1 : -1;
            shingle[49] += ((hashCode & (1UL << 49)) != 0) ? 1 : -1;
            shingle[50] += ((hashCode & (1UL << 50)) != 0) ? 1 : -1;
            shingle[51] += ((hashCode & (1UL << 51)) != 0) ? 1 : -1;
            shingle[52] += ((hashCode & (1UL << 52)) != 0) ? 1 : -1;
            shingle[53] += ((hashCode & (1UL << 53)) != 0) ? 1 : -1;
            shingle[54] += ((hashCode & (1UL << 54)) != 0) ? 1 : -1;
            shingle[55] += ((hashCode & (1UL << 55)) != 0) ? 1 : -1;
            shingle[56] += ((hashCode & (1UL << 56)) != 0) ? 1 : -1;
            shingle[57] += ((hashCode & (1UL << 57)) != 0) ? 1 : -1;
            shingle[58] += ((hashCode & (1UL << 58)) != 0) ? 1 : -1;
            shingle[59] += ((hashCode & (1UL << 59)) != 0) ? 1 : -1;
            shingle[60] += ((hashCode & (1UL << 60)) != 0) ? 1 : -1;
            shingle[61] += ((hashCode & (1UL << 61)) != 0) ? 1 : -1;
            shingle[62] += ((hashCode & (1UL << 62)) != 0) ? 1 : -1;
            shingle[63] += ((hashCode & (1UL << 63)) != 0) ? 1 : -1;
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

    public static unsafe ulong DoSimd(string input)
    {
        if (input is null)
            return 0;

        var text = input.Trim();

        if (text.Length == 0)
            return 0;

        Span<int> shingles = stackalloc int[64];
        var span = text.AsSpan();

        return Avx2.IsSupported ? ComputeAvx(shingles, span) : throw new NotSupportedException("no supported platform found");
    }

    public static unsafe ulong DoSimdSimpler(string input)
    {
        if (input is null)
            return 0;

        var text = input.Trim();

        if (text.Length == 0)
            return 0;

        Span<int> shingles = stackalloc int[64];
        var span = text.AsSpan();

        return Avx2.IsSupported ? ComputeAvxMultipletrigrams(shingles, span) : throw new NotSupportedException("no supported platform found");
    }

    private static unsafe ulong ComputeAvx(Span<int> shingle, ReadOnlySpan<char> span)
    {
        const int batchSize = 8;

        Span<Vector256<int>> accumulators = stackalloc Vector256<int>[batchSize];
        for (int i = 0; i < batchSize; i++)
        {
            accumulators[i] = Vector256<int>.Zero;
        }

        for (int i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);

            for (var j = 0; j < batchSize; j++)
            {
                var bits = (hashCode >> (j * batchSize)) & 0xFF;
                Vector256<int> bitMask = Vector256.Create(
                    (int)((bits >> 0) & 1),
                    (int)((bits >> 1) & 1),
                    (int)((bits >> 2) & 1),
                    (int)((bits >> 3) & 1),
                    (int)((bits >> 4) & 1),
                    (int)((bits >> 5) & 1),
                    (int)((bits >> 6) & 1),
                    (int)((bits >> 7) & 1)
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

    private static unsafe ulong ComputeAvxMultipletrigrams(Span<int> shingle, ReadOnlySpan<char> span)
    {
        const int batchSize = 8;
        const int trigramsPerBatch = 4;

        Span<Vector256<int>> accumulators = stackalloc Vector256<int>[batchSize];
        for (int i = 0; i < batchSize; i++)
        {
            accumulators[i] = Vector256<int>.Zero;
        }

        var index = 0;
        for (; index < span.Length - 2 - trigramsPerBatch; index += trigramsPerBatch)
        {
            var slice1 = span.Slice(index + 0, 3);
            var slice2 = span.Slice(index + 1, 3);
            var slice3 = span.Slice(index + 2, 3);
            var slice4 = span.Slice(index + 3, 3);

            var hash1 = ComputeFnv1aHash(slice1);
            var hash2 = ComputeFnv1aHash(slice2);
            var hash3 = ComputeFnv1aHash(slice3);
            var hash4 = ComputeFnv1aHash(slice4);

            for (var i = 0; i < batchSize; i++)
            {
                var bitMask0 = CreateBitMask(hash1, i);
                var bitMask1 = CreateBitMask(hash2, i);
                var bitMask2 = CreateBitMask(hash3, i);
                var bitMask3 = CreateBitMask(hash4, i);

                var combinedMask = Avx2.Add(
                    Avx2.Add(bitMask0, bitMask1),
                    Avx2.Add(bitMask2, bitMask3)
                );

                var update = Avx2.Subtract(Avx2.ShiftLeftLogical(combinedMask, 1),
                    Vector256.Create(trigramsPerBatch));

                accumulators[i] = Avx2.Add(accumulators[i], update);
            }
        }

        for (; index < span.Length - 2; index++)
        {
            var slice = span.Slice(index, 3);
            var hashCode = ComputeFnv1aHash(slice);

            for (var i = 0; i < batchSize; i++)
            {
                var bits = (hashCode >> (i * batchSize)) & 0xFF;
                var bitMask = CreateBitMask(hashCode, i);

                var update = Avx2.Subtract(Avx2.ShiftLeftLogical(bitMask, 1), Vector256<int>.One);

                accumulators[i] = Avx2.Add(accumulators[i], update);
            }
        }

        // Final accumulation into shingle
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

    private static Vector256<int> CreateBitMask(ulong hashCode, int j)
    {
        var bits = (hashCode >> (j * 8)) & 0xFF;
        return Vector256.Create(
            (int)((bits >> 0) & 1),
            (int)((bits >> 1) & 1),
            (int)((bits >> 2) & 1),
            (int)((bits >> 3) & 1),
            (int)((bits >> 4) & 1),
            (int)((bits >> 5) & 1),
            (int)((bits >> 6) & 1),
            (int)((bits >> 7) & 1)
        );
    }

    private static ulong ComputeFnv1aHash(ReadOnlySpan<char> trigram)
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
