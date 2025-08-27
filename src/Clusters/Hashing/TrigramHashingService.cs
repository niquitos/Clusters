
using MurmurHash;
using MurmurHash.Net;
using System;
using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text;

namespace Clusters.Hashing;

public class TrigramHashingService : HashingService
{
    public void HashMD5(string input)
    {
        Span<byte> buffer = stackalloc byte[input.Length * 2];
        var bytesWritten = Encoding.UTF8.GetBytes(input.AsSpan(), buffer);

        var slice = buffer[..bytesWritten];
        var hashCode = MD5.HashData(slice);
    }

    public void HashMurmur(string input)
    {

        Span<byte> buffer = stackalloc byte[input.Length * 2];
        var bytesWritten = Encoding.UTF8.GetBytes(input.AsSpan(), buffer);

        var slice = buffer[..bytesWritten];
        var hash = MurmurHash3.Hash32(bytes: slice, seed: 123456U);
    }

    public ulong HashXx64(string input)
    {
        Span<byte> buffer = stackalloc byte[input.Length * 4];
        var bytesWritten = Encoding.UTF8.GetBytes(input.AsSpan(), buffer);

        var slice = buffer[..bytesWritten];
        return XxHash64.HashToUInt64(slice);
    }

    public ulong HashXx64(byte[] input)
    {
        return XxHash64.HashToUInt64(input);
    }

    public uint HashMurmur(byte[] input)
    {
        return MurmurHash3.Hash32(bytes: input, seed: 123456U);
    }

    public void HashFnv1aSimpleFor(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = Fnv1aHashSimpleFor(slice);
    }

    public void HashFnv1aUnrolled(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = Fnv1aHashUnrolled(slice);
    }

    public void HashFnv1aUnsafe(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = Fnv1aHashUnsafe(slice);

    }

    public void HashFnv1aText(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = Fnv1aHashSimd(slice);

    }

    public void HashGetHashCodeSubstrings(string input)
    {

        var slice = input[..3];
        var hashCode = slice.GetHashCode();

    }

    public void HashGetHashCodeSpans(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = string.GetHashCode(slice);

    }
}
