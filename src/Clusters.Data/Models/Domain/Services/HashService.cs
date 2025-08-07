using System;
using System.Collections.Generic;

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
                    shingle[j] += (int)(hashCode >> j & 1) * 2 - 1;
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
}