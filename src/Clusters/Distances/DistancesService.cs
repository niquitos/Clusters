using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

namespace Clusters.Distances;

public static class DistancesService
{
    public static double LevenshteinDistanceRatio(string text, string other)
    {
        var distance = LevinshteinDistanceService.ComputeLevenshteinDistanceOptimizedSIMD(text, other);
        var largest = Math.Max(text.Length, other.Length);
        return 1 - ((double)distance / largest);
    }

    public static double HammingDistanceRatio(string text, string other)
    {
        var hash1 = FeaturizeText(text);
        var hash2 = FeaturizeText(other);

        ulong xorResult = hash1 ^ hash2;
        return 1 - ((double)BitOperations.PopCount(xorResult) / 64);
    }

    public static double JaccardRatio(string text, string other)
    {
        var hash1 = FeaturizeText(text);
        var hash2 = FeaturizeText(other);

        ulong intersection = hash1 & hash2;
        int intersectionCount = BitOperations.PopCount(intersection);

        ulong union = hash1 | hash2;
        int unionCount = BitOperations.PopCount(union);

        if (unionCount == 0)
            return 0.0;

        return (double)intersectionCount / unionCount;
    }

    private static ulong FeaturizeText(string text)
    {
        ulong vector = 0;

        foreach (var symbol in text)
        {
            unchecked
            {
                int hash = symbol.GetHashCode();
                uint unsignedHash = (uint)hash;
                int index = (int)(unsignedHash % 64);
                vector |= 1UL << index;
            }
        }

        return vector;
    }

}
