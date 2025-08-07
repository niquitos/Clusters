using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Clusters.Hashing;

public static class SimHashService
{
    private const ulong offsetBasis = 14695981039346656037;
    private const ulong prime = 1099511628211;

    private static HashSet<char> _delimeters = [' ', '\\', '/', '@', '"', '.', '|'];

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

    public static ulong DoBitHack()
    {
        Span<int> shingle = stackalloc int[64];
        var span = StringHelper.AllSymbolsInput.AsSpan();

        for (var i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);

            for (var j = 0; j < 64; j++)
            {
                shingle[j] += ((int)((hashCode >> j) & 1) * 2) - 1;
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

    public static ulong DoBitHackUnrolled()
    {
        Span<int> shingle = stackalloc int[64];
        var span = StringHelper.AllSymbolsInput.AsSpan();

        for (var i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);

            shingle[0] += ((int)((hashCode >> 0) & 1) * 2) - 1;
            shingle[1] += ((int)((hashCode >> 1) & 1) * 2) - 1;
            shingle[2] += ((int)((hashCode >> 2) & 1) * 2) - 1;
            shingle[3] += ((int)((hashCode >> 3) & 1) * 2) - 1;
            shingle[4] += ((int)((hashCode >> 4) & 1) * 2) - 1;
            shingle[5] += ((int)((hashCode >> 5) & 1) * 2) - 1;
            shingle[6] += ((int)((hashCode >> 6) & 1) * 2) - 1;
            shingle[7] += ((int)((hashCode >> 7) & 1) * 2) - 1;
            shingle[8] += ((int)((hashCode >> 8) & 1) * 2) - 1;
            shingle[9] += ((int)((hashCode >> 9) & 1) * 2) - 1;
            shingle[10] += ((int)((hashCode >> 10) & 1) * 2) - 1;
            shingle[11] += ((int)((hashCode >> 11) & 1) * 2) - 1;
            shingle[12] += ((int)((hashCode >> 12) & 1) * 2) - 1;
            shingle[13] += ((int)((hashCode >> 13) & 1) * 2) - 1;
            shingle[14] += ((int)((hashCode >> 14) & 1) * 2) - 1;
            shingle[15] += ((int)((hashCode >> 15) & 1) * 2) - 1;
            shingle[16] += ((int)((hashCode >> 16) & 1) * 2) - 1;
            shingle[17] += ((int)((hashCode >> 17) & 1) * 2) - 1;
            shingle[18] += ((int)((hashCode >> 18) & 1) * 2) - 1;
            shingle[19] += ((int)((hashCode >> 19) & 1) * 2) - 1;
            shingle[20] += ((int)((hashCode >> 20) & 1) * 2) - 1;
            shingle[21] += ((int)((hashCode >> 21) & 1) * 2) - 1;
            shingle[22] += ((int)((hashCode >> 22) & 1) * 2) - 1;
            shingle[23] += ((int)((hashCode >> 23) & 1) * 2) - 1;
            shingle[24] += ((int)((hashCode >> 24) & 1) * 2) - 1;
            shingle[25] += ((int)((hashCode >> 25) & 1) * 2) - 1;
            shingle[26] += ((int)((hashCode >> 26) & 1) * 2) - 1;
            shingle[27] += ((int)((hashCode >> 27) & 1) * 2) - 1;
            shingle[28] += ((int)((hashCode >> 28) & 1) * 2) - 1;
            shingle[29] += ((int)((hashCode >> 29) & 1) * 2) - 1;
            shingle[30] += ((int)((hashCode >> 30) & 1) * 2) - 1;
            shingle[31] += ((int)((hashCode >> 31) & 1) * 2) - 1;
            shingle[32] += ((int)((hashCode >> 32) & 1) * 2) - 1;
            shingle[33] += ((int)((hashCode >> 33) & 1) * 2) - 1;
            shingle[34] += ((int)((hashCode >> 34) & 1) * 2) - 1;
            shingle[35] += ((int)((hashCode >> 35) & 1) * 2) - 1;
            shingle[36] += ((int)((hashCode >> 36) & 1) * 2) - 1;
            shingle[37] += ((int)((hashCode >> 37) & 1) * 2) - 1;
            shingle[38] += ((int)((hashCode >> 38) & 1) * 2) - 1;
            shingle[39] += ((int)((hashCode >> 39) & 1) * 2) - 1;
            shingle[40] += ((int)((hashCode >> 40) & 1) * 2) - 1;
            shingle[41] += ((int)((hashCode >> 41) & 1) * 2) - 1;
            shingle[42] += ((int)((hashCode >> 42) & 1) * 2) - 1;
            shingle[43] += ((int)((hashCode >> 43) & 1) * 2) - 1;
            shingle[44] += ((int)((hashCode >> 44) & 1) * 2) - 1;
            shingle[45] += ((int)((hashCode >> 45) & 1) * 2) - 1;
            shingle[46] += ((int)((hashCode >> 46) & 1) * 2) - 1;
            shingle[47] += ((int)((hashCode >> 47) & 1) * 2) - 1;
            shingle[48] += ((int)((hashCode >> 48) & 1) * 2) - 1;
            shingle[49] += ((int)((hashCode >> 49) & 1) * 2) - 1;
            shingle[50] += ((int)((hashCode >> 50) & 1) * 2) - 1;
            shingle[51] += ((int)((hashCode >> 51) & 1) * 2) - 1;
            shingle[52] += ((int)((hashCode >> 52) & 1) * 2) - 1;
            shingle[53] += ((int)((hashCode >> 53) & 1) * 2) - 1;
            shingle[54] += ((int)((hashCode >> 54) & 1) * 2) - 1;
            shingle[55] += ((int)((hashCode >> 55) & 1) * 2) - 1;
            shingle[56] += ((int)((hashCode >> 56) & 1) * 2) - 1;
            shingle[57] += ((int)((hashCode >> 57) & 1) * 2) - 1;
            shingle[58] += ((int)((hashCode >> 58) & 1) * 2) - 1;
            shingle[59] += ((int)((hashCode >> 59) & 1) * 2) - 1;
            shingle[60] += ((int)((hashCode >> 60) & 1) * 2) - 1;
            shingle[61] += ((int)((hashCode >> 61) & 1) * 2) - 1;
            shingle[62] += ((int)((hashCode >> 62) & 1) * 2) - 1;
            shingle[63] += ((int)((hashCode >> 63) & 1) * 2) - 1;
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

    public static ulong DoBitHackNonOverlapping()
    {
        Span<int> shingle = stackalloc int[64];
        var span = StringHelper.AllSymbolsInput.AsSpan();

        for (var i = 0; i < span.Length - 2; i += 3)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);

            for (var j = 0; j < 64; j++)
            {
                shingle[j] += ((int)((hashCode >> j) & 1) * 2) - 1;
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
        var batchSize = Vector256<int>.Count;

        var accumulators = stackalloc Vector256<int>[batchSize];

        for (int i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);

            for (var j = 0; j < batchSize; j++)
            {
                var bits = (hashCode >> (j * batchSize)) & 0b11111111;
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
        int batchSize = Vector256<int>.Count;
        const int trigramsPerBatch = 4;

        Span<Vector256<int>> accumulators = stackalloc Vector256<int>[batchSize];
        //for (int i = 0; i < batchSize; i++)
        //{
        //    accumulators[i] = Vector256<int>.Zero;
        //}

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
                var bitMask = CreateBitMask(hashCode, i);

                var update = Avx2.Subtract(Avx2.ShiftLeftLogical(bitMask, 1), Vector256<int>.One);

                accumulators[i] = Avx2.Add(accumulators[i], update);
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

    private static Vector256<int> CreateBitMask(ulong hashCode, int j)
    {
        var bits = (hashCode >> (j * 8)) & 0b11111111;
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

    public static ulong BitHackSplit(string input)
    {
        Span<int> shingle = stackalloc int[64];

        char[] delimiters = { ' ', '\\', '/', '@', '"', '.', '|' };
        var tokens = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            var hashCode = ComputeFnv1aHash2(token.AsSpan());

            for (var j = 0; j < 64; j++)
            {
                shingle[j] += ((int)((hashCode >> j) & 1) * 2) - 1;
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

    public static ulong BitHackSplit2(string input)
    {
        Span<int> shingle = stackalloc int[64];

        var span = input.AsSpan();
        int start = 0;

        for (int i = 0; i <= span.Length; i++)
        {
            if (i == span.Length || _delimeters.Contains(span[i]))
            {
                if (start < i)
                {
                    var token = span[start..i];
                    var hashCode = ComputeFnv1aHash2(token);

                    for (var j = 0; j < 64; j++)
                    {
                        shingle[j] += ((int)((hashCode >> j) & 1) * 2) - 1;
                    }
                }
                start = i + 1;
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

    private static ulong ComputeFnv1aHash2(ReadOnlySpan<char> text)
    {
        ulong hash = offsetBasis;

        foreach (var ch in text)
        {
            hash ^= ch;
            hash *= prime;
        }

        return hash;
    }

    public static (ulong, ulong) BitHackSplit128(string input)
    {
        Span<int> shingle = stackalloc int[128];

        var span = input.AsSpan();
        int start = 0;

        for (int i = 0; i <= span.Length; i++)
        {
            if (i == span.Length || _delimeters.Contains(span[i]))
            {
                if (start < i)
                {
                    var token = span[start..i];
                    var (hash1, hash2) = ComputeFnv1aHash128(token);

                    for (var j = 0; j < 64; j++)
                    {
                        shingle[j] += ((int)((hash1 >> j) & 1) * 2) - 1;
                    }

                    for (var j = 0; j < 64; j++)
                    {
                        shingle[j + 64] += ((int)((hash2 >> j) & 1) * 2) - 1;
                    }
                }
                start = i + 1;
            }
        }

        ulong simhash1 = 0L;
        ulong simhash2 = 0L;

        for (var i = 0; i < 64; i++)
        {
            if (shingle[i] > 0)
            {
                simhash1 |= 1UL << i;
            }
        }

        for (var i = 0; i < 64; i++)
        {
            if (shingle[i + 64] > 0)
            {
                simhash2 |= 1UL << i;
            }
        }

        return (simhash1, simhash2);
    }

    private static (ulong, ulong) ComputeFnv1aHash128(ReadOnlySpan<char> text)
    {
        ulong prime1 = 0x0000000001000000;
        ulong prime2 = 0x000000000000013B;
        ulong offsetBasis1 = 0x6C62272E07BB0142;
        ulong offsetBasis2 = 0x62B821756295C58D;

        ulong hash1 = offsetBasis1;
        ulong hash2 = offsetBasis2;

        foreach (var ch in text)
        {
            hash1 ^= ch;
            hash2 ^= ch;

            var temp1 = hash1 * prime1;
            var temp2 = hash1 * prime2;
            var temp3 = hash2 * prime1;
            var temp4 = hash2 * prime2;

            hash1 = temp1 ^ temp4;
            hash2 = temp2 ^ temp3;
        }

        return (hash1, hash2);
    }
}
