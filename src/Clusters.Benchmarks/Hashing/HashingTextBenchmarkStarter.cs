using BenchmarkDotNet.Attributes;
using Clusters.Hashing;
namespace Clusters.Benchmarks.Hashing;

[MemoryDiagnoser]
public class HashingTextBenchmarkStarter
{
    private const string Input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz" +
    "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ абвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
    "0123456789" +
    "!@#$%^&*()_+-=[]{}|;:'\",.<>/?" +
    "\n\r\t\0" +
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz" +
    "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ абвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
    "0123456789" +
    "!@#$%^&*()_+-=[]{}|;:'\",.<>/?" +
    "\n\r\t\0" +
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz" +
    "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ абвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
    "0123456789" +
    "!@#$%^&*()_+-=[]{}|;:'\",.<>/?" +
    "\n\r\t\0";

    private readonly TextHashingService _service;

    public HashingTextBenchmarkStarter()
    {
        _service = new TextHashingService();
    }

    //[Benchmark]
    //public void MD5() => _service.HashMD5(Input);

    //[Benchmark]
    //public void Xx64() => _service.HashXx64(Input);

    //[Benchmark]
    //public void Fnv1a_Simple() => _service.HashFnv1aSimple(Input);

    //[Benchmark]
    //public void Fnv1a_Unsafe() => _service.HashFnv1aUnsafe(Input);

    //[Benchmark]
    //public void Fnv1a_Simd() => _service.HashFnv1aText(Input);

    [Benchmark]
    public void GetHashCode_Substrings() => _service.HashGetHashCode(Input);

    [Benchmark]
    public void GetHashCode_Spans() => _service.HashGetHashCodeSpans(Input);
}