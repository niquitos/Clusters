using System;
using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text;

namespace Clusters.Hashing;

public class TextHashingService : HashingService
{
    public void HashMD5(string input)
    {
        Span<byte> buffer = stackalloc byte[input.Length*2];
        Encoding.UTF8.GetBytes(input.AsSpan(), buffer);

        var hashCode = MD5.HashData(buffer);
    }

    public void HashXx64(string input)
    {
        Span<byte> buffer = stackalloc byte[input.Length*2];
        Encoding.UTF8.GetBytes(input.AsSpan(), buffer);

        var hashCode = XxHash64.HashToUInt64(buffer);
    }

    public void HashFnv1aSimple(string input)
    {
        var span = input.AsSpan();
        var hashCode = ComputeFnv1aHashSimpleFor(span);
    }

    public void HashFnv1aUnsafe(string input)
    {
        var span = input.AsSpan();
        var hashCode = ComputeFnv1aHashUnsafe(span);
    }

    public void HashFnv1aText(string input)
    {
        var span = input.AsSpan();
        var hashCode = ComputeFnv1aHashSimd(span);
    }

    public void HashGetHashCode(string input)
    {
        var hashCode = input.GetHashCode();
    }

    public void HashGetHashCodeSpans(string input)
    {
        var hashCode = string.GetHashCode(input.AsSpan());
    }
}
