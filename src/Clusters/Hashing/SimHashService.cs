using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Clusters.Hashing;

public static class SimHashService
{
    private const ulong offsetBasis = 14695981039346656037;
    private const ulong prime = 1099511628211;

    private static HashSet<char> _delimeters = [' ', '\\', '/', '@', '"', '.', '|'];

    public static ulong DoSimple(string input = StringHelper.AllSymbolsInput)
    {
        Span<int> shingle = stackalloc int[64];
        var span = input.AsSpan();

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

    public static ulong DoBitHack(string input = StringHelper.AllSymbolsInput)
    {
        Span<int> shingle = stackalloc int[64];
        var span = input.AsSpan();

        for (var i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);

            for (var j = 0; j < 64; j++)
            {
                shingle[j] += ((int)((hashCode & (1UL << j)) >> j) * 2) - 1;
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

    public static ulong DoBitHack2(string input = StringHelper.AllSymbolsInput)
    {
        Span<int> shingle = stackalloc int[64];
        var span = input.AsSpan();

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

    public static unsafe ulong DoUnsafe(string input)
    {
        Span<int> shingle = stackalloc int[64];
        var span = input.AsSpan();
        for (var i = 0; i < span.Length - 2; i++)
        {
            var slice = span.Slice(i, 3);
            var hashCode = ComputeFnv1aHash(slice);
            fixed (int* pShingle = shingle)
            {
                int* pCurrent = pShingle;
                for (var j = 0; j < 64; j++)
                {
                    *pCurrent += ((int)((hashCode >> j) & 1) * 2) - 1;
                    pCurrent++;
                }
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

    public static unsafe ulong DoUnrolled(string input)
    {
        Span<int> shingle = stackalloc int[64];
        var span = input.AsSpan();

        fixed (int* pShingle = shingle)
        {
            for (var i = 0; i < span.Length - 2; i++)
            {
                var slice = span.Slice(i, 3);
                var hashCode = ComputeFnv1aHash(slice);

                int* pCurrent = pShingle;

                *pCurrent += ((int)((hashCode >> 0) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 1) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 2) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 3) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 4) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 5) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 6) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 7) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 8) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 9) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 10) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 11) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 12) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 13) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 14) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 15) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 16) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 17) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 18) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 19) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 20) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 21) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 22) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 23) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 24) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 25) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 26) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 27) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 28) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 29) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 30) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 31) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 32) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 33) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 34) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 35) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 36) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 37) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 38) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 39) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 40) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 41) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 42) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 43) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 44) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 45) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 46) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 47) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 48) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 49) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 50) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 51) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 52) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 53) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 54) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 55) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 56) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 57) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 58) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 59) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 60) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 61) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 62) & 1) * 2) - 1; pCurrent++;
                *pCurrent += ((int)((hashCode >> 63) & 1) * 2) - 1;
            }
        }

        ulong simhash = 0L;
        fixed (int* pShingle = shingle)
        {
            int* pCurrent = pShingle;
            for (var i = 0; i < 64; i++)
            {
                if (*pCurrent > 0)
                {
                    simhash |= 1UL << i;
                }
                pCurrent++;
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

    public static ulong DoSimd(string input)
    {
        if (input is null)
            return 0;

        var text = input.Trim();

        if (text.Length == 0)
            return 0;

        Span<int> shingles = stackalloc int[64];
        var span = text.AsSpan();

        return ComputeAvx(shingles, span);
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


    public static unsafe ulong DoSIMD(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return 0;

        Span<int> shingle = stackalloc int[64];
        var span = input.AsSpan();

        var batchSize = Vector256<int>.Count;
        var accumulators = stackalloc Vector256<int>[batchSize];
        for (int i = 0; i < span.Length - 2; i++)
        {
            var hashCode = ComputeFnv1aHash(span.Slice(i, 3));
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
}

public static class SimHashService2
{
    private const int HashSize = 64; // SimHash size in bits
    private const ulong offsetBasis = 14695981039346656037;
    private const ulong prime = 1099511628211;

    public static ulong ComputeSimHashVector(string input)
    {
        Span<int> shingle = stackalloc int[HashSize];
        var span = input.AsSpan();

        int batchSize = Vector<int>.Count; 
        var accumulators = new Vector<int>[batchSize];

        for (int i = 0; i < span.Length - 2; i++)
        {
            var hashCode = ComputeFnv1aHash(span.Slice(i, 3));

            for (int j = 0; j < batchSize; j++)
            {
                var bits = (hashCode >> (j * batchSize)) & ((1UL << batchSize) - 1);

                Span<int> maskArray = stackalloc int[batchSize];
                for (int k = 0; k < batchSize; k++)
                {
                    maskArray[k] = (int)((bits >> k) & 1);
                }
                var bitMask = new Vector<int>(maskArray);

                var update = (bitMask * 2) - Vector<int>.One;

                accumulators[j] += update;
            }
        }

        for (int i = 0; i < batchSize; i++)
        {
            for (int j = 0; j < batchSize; j++)
            {
                var bitPos = (i * batchSize) + j;
                if (bitPos < HashSize)
                {
                    shingle[bitPos] += accumulators[i][j];
                }
            }
        }

        ulong simHash = 0;
        for (int i = 0; i < HashSize; i++)
        {
            if (shingle[i] > 0)
            {
                simHash |= 1UL << i;
            }
        }

        return simHash;
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

    private static Vector<long> CreateMask(ulong hash)
    {
        long[] maskArray = new long[Vector<long>.Count];
        for (int i = 0; i < HashSize; i++)
        {
            // Use the bit hack to calculate -1 or 1 for each bit
            long value = ((long)((hash >> i) & 1UL) * 2L) - 1L;
            maskArray[i / 64] |= value << (i % 64);
        }
        return new Vector<long>(maskArray);
    }
}
