using BenchmarkDotNet.Attributes;
using Clusters.Distances;
using System.Text;

namespace Clusters.Benchmarks.Distances;

[MemoryDiagnoser]
public class DistancesBenchmarkStarter
{
    private static readonly Random _random = new();

    private string? _text;
    private string? _other;

    [Params(3000)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        _text = GenerateRandomUtf8String(Length - 2);
        _other = GenerateRandomUtf8String(Length + 1);
    }

    [Benchmark]
    public void LevinshteinDistanceRatio() => DistancesService.LevenshteinDistanceRatio(_text!, _other!);

    [Benchmark]
    public void HammingDistanceRatio() => DistancesService.HammingDistanceRatio(_text!, _other!);

    [Benchmark]
    public void JaccardRatio() => DistancesService.JaccardRatio(_text!, _other!);

    public static string GenerateRandomUtf8String(int length)
    {
        var sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {

            int codePoint = _random.Next(0, 0x10FFFF + 1);
            if (codePoint is >= 0xD800 and <= 0xDFFF)
            {
                i--;
                continue;
            }
            sb.Append(char.ConvertFromUtf32(codePoint));
        }

        return sb.ToString();
    }
}




