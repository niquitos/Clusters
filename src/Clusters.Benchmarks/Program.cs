using BenchmarkDotNet.Running;

namespace Clusters.Benchmarks;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Application).Assembly).Run();
    }
}
