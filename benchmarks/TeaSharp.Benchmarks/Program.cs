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
        typeof(ViewportRenderBenchmarks),
        typeof(SloLatencyBenchmarks),
    ];

    public static void Main(string[] args)
    {
        if (PerfGateRunner.TryRun(args, out var exitCode))
        {
            Environment.ExitCode = exitCode;
            return;
        }

        BenchmarkSwitcher.FromTypes(BenchmarkTypes).Run(args);
    }
}
