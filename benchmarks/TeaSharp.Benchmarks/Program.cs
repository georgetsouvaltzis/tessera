using BenchmarkDotNet.Running;

namespace TeaSharp.Benchmarks;

public static class Program
{
    private static readonly Type[] BenchmarkTypes =
    [
        typeof(StartupRenderBenchmarks),
        typeof(LargeTableBenchmarks),
        typeof(StyledHeavyOutputBenchmarks),
    ];

    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromTypes(BenchmarkTypes).Run(args);
    }
}
