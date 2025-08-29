using System.Numerics;

namespace Clusters.Clusterization.MeanShift;

public static class CentroidsCalculator
{

    public static ulong Simple(List<ulong> hashes)
    {
        ulong majority = 0;
        int count = hashes.Count;

        for (int i = 0; i < 64; i++)
        {
            int bitCount = 0;
            foreach (var hash in hashes)
            {
                if (((hash >> i) & 1) == 1)
                {
                    bitCount++;
                }
            }

            if (bitCount > count / 2)
            {
                majority |= 1UL << i;
            }
        }

        return majority;
    }

    public static ulong SIMD(List<ulong> hashes)
    {
        // Initialize the result
        ulong majority = 0;

        // Determine the number of hashes
        int count = hashes.Count;

        // Process each bit position (0 to 63)
        for (int bitPosition = 0; bitPosition < 64; bitPosition++)
        {
            // Use SIMD to calculate the number of bits set at the current bit position
            int bitCount = 0;

            // Process hashes in chunks of Vector<ulong>.Count
            int simdLength = Vector<ulong>.Count;
            int i = 0;

            // Create a mask for the current bit position
            Vector<ulong> bitMask = new Vector<ulong>(1UL << bitPosition);

            // Process hashes in SIMD chunks
            for (; i <= hashes.Count - simdLength; i += simdLength)
            {
                // Load a vector of hashes from the list
                Vector<ulong> hashVector = new Vector<ulong>(hashes.ToArray(), i);

                // Check if the bit at the current position is set
                Vector<ulong> masked = Vector.BitwiseAnd(hashVector, bitMask);
                Vector<ulong> comparison = Vector.Equals(masked, bitMask);

                // Count the number of true values in the comparison vector
                for (int j = 0; j < simdLength; j++)
                {
                    if (comparison[j] != 0)
                    {
                        bitCount++;
                    }
                }
            }

            // Process remaining hashes that don't fit into a full SIMD vector
            for (; i < hashes.Count; i++)
            {
                if (((hashes[i] >> bitPosition) & 1) == 1)
                {
                    bitCount++;
                }
            }

            // Determine if the majority of bits are set at this position
            if (bitCount > count / 2)
            {
                majority |= 1UL << bitPosition;
            }
        }

        return majority;
    }

    public static ulong InParallel(List<ulong> hashes)
    {
        // Initialize the result
        ulong majority = 0;
        int count = hashes.Count;

        // Process each bit position (0 to 63) in parallel
        Parallel.For(0, 64, bitPosition =>
        {
            int bitCount = 0;

            // Use SIMD to process hashes in chunks
            int simdLength = Vector<ulong>.Count;
            int i = 0;

            // Create a mask for the current bit position
            Vector<ulong> bitMask = new Vector<ulong>(1UL << bitPosition);

            // Process hashes in SIMD chunks
            for (; i <= hashes.Count - simdLength; i += simdLength)
            {
                // Load a vector of hashes from the list
                Vector<ulong> hashVector = new Vector<ulong>(hashes.ToArray(), i);

                // Check if the bit at the current position is set
                Vector<ulong> masked = Vector.BitwiseAnd(hashVector, bitMask);
                Vector<ulong> comparison = Vector.Equals(masked, bitMask);

                // Count the number of true values in the comparison vector
                for (int j = 0; j < simdLength; j++)
                {
                    if (comparison[j] != 0)
                    {
                        bitCount++;
                    }
                }
            }

            // Process remaining hashes that don't fit into a full SIMD vector
            for (; i < hashes.Count; i++)
            {
                if (((hashes[i] >> bitPosition) & 1) == 1)
                {
                    bitCount++;
                }
            }

            // Determine if the majority of bits are set at this position
            if (bitCount > count / 2)
            {
                // Use Interlocked to safely update the majority variable
                ulong mask = 1UL << bitPosition;
                System.Threading.Interlocked.Or(ref majority, mask);
            }
        });

        return majority;
    }
}