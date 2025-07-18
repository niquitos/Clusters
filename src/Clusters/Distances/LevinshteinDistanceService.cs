using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Clusters.Distances;

public static unsafe class LevinshteinDistanceService
{
    public static int LevenshteinDistanceRecursive(string text, string other)
    {
        if (text.Length == 0)
            return other.Length;
        if (other.Length == 0)
            return text.Length;

        int cost = text[^1] == other[^1] ? 0 : 1;

        return Math.Min(
            Math.Min(
                LevenshteinDistanceRecursive(text[..^1], other) + 1,
                LevenshteinDistanceRecursive(text, other[..^1]) + 1
            ),
            LevenshteinDistanceRecursive(text[..^1], other[..^1]) + cost
        );
    }

    public static int ComputeLevenshteinDistance(string text, string other)
    {
        int[,] dp = new int[text.Length + 1, other.Length + 1];

        for (int i = 0; i <= text.Length; i++)
            dp[i, 0] = i;

        for (int j = 0; j <= other.Length; j++)
            dp[0, j] = j;

        for (int i = 1; i <= text.Length; i++)
        {
            for (int j = 1; j <= other.Length; j++)
            {
                int cost = text[i - 1] == other[j - 1] ? 0 : 1;
                dp[i, j] = Math.Min(
                    Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + cost
                );
            }
        }

        return dp[text.Length, other.Length];
    }

    public static int ComputeLevenshteinDistanceOptimized(string text, string other)
    {
        if (text.Length == 0)
            return other.Length;
        if (other.Length == 0)
            return text.Length;

        if (text.Length > other.Length)
        {
            (text, other) = (other, text);
        }

        var previousRow = new int[text.Length + 1];
        var currentRow = new int[text.Length + 1];

        for (int i = 0; i <= text.Length; i++)
        {
            previousRow[i] = i;
        }

        for (int j = 1; j <= other.Length; j++)
        {
            currentRow[0] = j;

            for (int i = 1; i <= text.Length; i++)
            {
                int cost = text[i - 1] == other[j - 1] ? 0 : 1;
                currentRow[i] = Math.Min(
                    Math.Min(currentRow[i - 1] + 1, previousRow[i] + 1),
                    previousRow[i - 1] + cost
                );
            }

            (previousRow, currentRow) = (currentRow, previousRow);
        }

        return previousRow[text.Length];
    }

    public static int ComputeLevenshteinDistanceOptimized_1(string text, string other)
    {
        if (text == other) return 0;
        if (text.Length == 0) return other.Length;
        if (other.Length == 0) return text.Length;

        // Ensure text is the shorter string
        if (text.Length > other.Length)
        {
            (text, other) = (other, text);
        }

        var textSpan = text.AsSpan();
        var otherSpan = other.AsSpan();
        var currentRow = new int[text.Length + 1];

        // Initialize first row
        for (int i = 0; i <= text.Length; i++)
        {
            currentRow[i] = i;
        }

        for (int j = 1; j <= other.Length; j++)
        {
            int previousDiagonal = currentRow[0];
            currentRow[0] = j;
            char otherChar = otherSpan[j - 1];

            for (int i = 1; i <= text.Length; i++)
            {
                int previousAbove = currentRow[i];
                int cost = textSpan[i - 1] == otherChar ? 0 : 1;
                currentRow[i] = Math.Min(
                    Math.Min(currentRow[i - 1] + 1, previousAbove + 1),
                    previousDiagonal + cost
                );
                previousDiagonal = previousAbove;
            }
        }

        return currentRow[text.Length];
    }

    public static int ComputeLevenshteinDistanceOptimized_1(ReadOnlySpan<char> text, ReadOnlySpan<char> other)
    {

        if (text.Length == 0) return other.Length;
        if (other.Length == 0) return text.Length;

        return text.SequenceEqual(other)
            ? 0
            : text.Length <= other.Length
            ? ComputeWithShorterFirst(text, other)
            : ComputeWithShorterFirst(other, text);
    }

    private static int ComputeWithShorterFirst(ReadOnlySpan<char> shorter, ReadOnlySpan<char> longer)
    {
        var currentRow = new int[shorter.Length + 1];

        for (int i = 0; i <= shorter.Length; i++)
        {
            currentRow[i] = i;
        }

        for (int j = 1; j <= longer.Length; j++)
        {
            int previousDiagonal = currentRow[0];
            currentRow[0] = j;
            char longerChar = longer[j - 1];

            for (int i = 1; i <= shorter.Length; i++)
            {
                int previousAbove = currentRow[i];
                int cost = shorter[i - 1] == longerChar ? 0 : 1;

                currentRow[i] = Math.Min(
                    Math.Min(currentRow[i - 1] + 1, previousAbove + 1),
                    previousDiagonal + cost
                );
                previousDiagonal = previousAbove;
            }
        }

        return currentRow[shorter.Length];
    }

    public static int ComputeLevenshteinDistanceOptimizedSIMD(ReadOnlySpan<char> text, ReadOnlySpan<char> other)
    {
        if (text.Length == 0) return other.Length;
        if (other.Length == 0) return text.Length;
        return text.SequenceEqual(other)
            ? 0
            : text.Length <= other.Length
            ? ComputeWithShorterFirstSIMD(text, other)
            : ComputeWithShorterFirstSIMD(other, text);
    }

    private static unsafe int ComputeWithShorterFirstSIMD(ReadOnlySpan<char> shorter, ReadOnlySpan<char> longer)
    {
        if (shorter.Length < 16)
        {
            return ComputeWithShorterFirst(shorter, longer);
        }
        
        int* currentRow = stackalloc int[shorter.Length + 1 + 7]; 

        int* alignedCurrentRow = (int*)(((nint)currentRow + 31) & ~31);
        if ((nint)alignedCurrentRow - (nint)currentRow > 7)
        {
            alignedCurrentRow = currentRow; 
        }

        for (int i = 0; i <= shorter.Length; i++)
        {
            alignedCurrentRow[i] = i;
        }

        fixed (char* shorterPtr = shorter)
        fixed (char* longerPtr = longer)
        {
            for (int j = 1; j <= longer.Length; j++)
            {
                int previousDiagonal = alignedCurrentRow[0];
                alignedCurrentRow[0] = j;
                char longerChar = longerPtr[j - 1];

                int i = 1;

                if (Avx2.IsSupported && shorter.Length >= 8)
                {
                    var costVector = Vector256.Create((ushort)longerChar);
                    var onesVector = Vector256.Create(1);
                    var zeroVector = Vector256<int>.Zero;

                    for (; i <= shorter.Length - 8; i += 8)
                    {
                        var currentVector = Avx.LoadVector256(alignedCurrentRow + i);

                        var shorterChars = Sse2.LoadVector128((ushort*)(shorterPtr + i - 1));
                        var nextShorterChars = Sse2.LoadVector128((ushort*)(shorterPtr + i + 3));
                        var charVector = Vector256.Create(shorterChars, nextShorterChars);
                        var costMask = Avx2.CompareEqual(charVector, costVector);
                        var costs = Avx2.ShiftRightLogical(costMask.AsInt32(), 31);
                        costs = Avx2.Xor(costs, Vector256.Create(-1));

                        var left = Avx2.Add(Avx.LoadVector256(alignedCurrentRow + i - 1), onesVector);
                        var up = Avx2.Add(currentVector, onesVector);
                        var diag = Avx2.Add(Vector256.Create(previousDiagonal), costs);

                        var min1 = Avx2.Min(left, up);
                        var min2 = Avx2.Min(min1, diag);

                        Avx.Store(alignedCurrentRow + i, min2);

                        previousDiagonal = alignedCurrentRow[i + 7];
                    }
                }

                for (; i <= shorter.Length; i++)
                {
                    int previousAbove = alignedCurrentRow[i];
                    int cost = shorterPtr[i - 1] == longerChar ? 0 : 1;

                    alignedCurrentRow[i] = Math.Min(
                        Math.Min(alignedCurrentRow[i - 1] + 1, previousAbove + 1),
                        previousDiagonal + cost
                    );
                    previousDiagonal = previousAbove;
                }
            }
        }

        return alignedCurrentRow[shorter.Length];
    }
}
