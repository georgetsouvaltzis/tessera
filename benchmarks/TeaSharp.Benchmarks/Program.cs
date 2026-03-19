using BenchmarkDotNet.Running;

namespace TeaSharp.Benchmarks;

public static class Program
{
    private static readonly Type[] BenchmarkTypes =
    [
        typeof(StartupRenderBenchmarks),
        typeof(LogTailStreamBenchmarks),
        typeof(LargeTableBenchmarks),
        typeof(OverlayStressBenchmarks),
        typeof(ResizeStormBenchmarks),
        typeof(StyledHeavyOutputBenchmarks),
    ];

    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromTypes(BenchmarkTypes).Run(args);
    }
}
