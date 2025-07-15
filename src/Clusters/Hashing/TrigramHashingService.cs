using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text;

namespace Clusters.Hashing;

public class TrigramHashingService : HashingService
{
    public void HashMD5(string input)
    {
        Span<byte> buffer = stackalloc byte[input.Length * 2];
        Encoding.UTF8.GetBytes(input.AsSpan(), buffer);

        var slice = buffer[..3];
        var hashCode = MD5.HashData(slice);
    }

    public void HashXx64(string input)
    {
        Span<byte> buffer = stackalloc byte[input.Length * 2];
        Encoding.UTF8.GetBytes(input.AsSpan(), buffer);
        
        var slice = buffer[..3];
        var hashCode = XxHash64.Hash(slice);
    }

    public void HashFnv1aSimple(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = ComputeFnv1aHashSimpleFor(slice);
    }

    public void HashFnv1aSimpleTrigram(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = ComputeFnv1aHashSimpleTrigram(slice);
    }

    public void HashFnv1aUnsafe(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = ComputeFnv1aHashUnsafe(slice);

    }

    public void HashFnv1aText(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = ComputeFnv1aHashSimd(slice);

    }

    public void HashFnv1aTrigram(string input)
    {
        var span = input.AsSpan();

        var slice = span[..3];
        var hashCode = ComputeFnv1aUnsafeTrigramHash(slice);

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
