using BenchmarkDotNet.Running;

namespace Tessera.Benchmarks;

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
        typeof(InputDecodingBenchmarks)
    ];

    public static void Main(string[] args)
    {
        if (PerfGateRunner.TryRun(args, out var exitCode) || RuntimeEndToEndRunner.TryRun(args, out exitCode))
        {
            Environment.ExitCode = exitCode;
            return;
        }

        BenchmarkSwitcher.FromTypes(BenchmarkTypes).Run(args);
    }
}
