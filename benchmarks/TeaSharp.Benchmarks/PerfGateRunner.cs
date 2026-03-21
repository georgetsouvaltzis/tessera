using System.Text.Json;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace TeaSharp.Benchmarks;

internal static class PerfGateRunner
{
    private const string GateFlag = "--perf-gate";

    public static bool TryRun(string[] args, out int exitCode)
    {
        exitCode = 0;
        if (!ContainsFlag(args, GateFlag))
        {
            return false;
        }

        string? baselinePath = null;
        string? outputPath = null;
        var dryRun = false;

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            switch (arg)
            {
                case "--baseline":
                    if (!TryReadValue(args, ref index, out baselinePath))
                    {
                        Console.Error.WriteLine("Missing value for --baseline.");
                        exitCode = 1;
                        return true;
                    }

                    break;
                case "--output":
                    if (!TryReadValue(args, ref index, out outputPath))
                    {
                        Console.Error.WriteLine("Missing value for --output.");
                        exitCode = 1;
                        return true;
                    }

                    break;
                case "--dry-run":
                    dryRun = true;
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(baselinePath))
        {
            Console.Error.WriteLine("Usage: --perf-gate --baseline <path> [--output <path>] [--dry-run]");
            exitCode = 1;
            return true;
        }

        var baseline = LoadBaseline(baselinePath);
        var result = dryRun
            ? CreateDryRunResult(baselinePath, baseline)
            : ExecuteGate(baselinePath, baseline);

        EmitResult(result, outputPath);
        exitCode = result.Status == "pass" || result.Status == "dry-run" ? 0 : 2;
        return true;
    }

    private static PerfGateExecutionResult ExecuteGate(string baselinePath, PerfGateBaseline baseline)
    {
        var config = CreateGateConfig();
        var summary = BenchmarkRunner.Run<SloLatencyBenchmarks>(config);
        var measurements = CollectMeasurements(summary);
        return CompareAgainstBaseline(baselinePath, baseline, measurements);
    }

    private static IConfig CreateGateConfig()
    {
        var job = Job.Default
            .WithId("slo-gate")
            .WithLaunchCount(1)
            .WithWarmupCount(1)
            .WithIterationCount(8)
            .WithToolchain(InProcessEmitToolchain.Instance);

        return ManualConfig
            .Create(DefaultConfig.Instance)
            .AddLogger(ConsoleLogger.Default)
            .AddJob(job);
    }

    private static Dictionary<string, PerfGateMeasurement> CollectMeasurements(Summary summary)
    {
        var measurements = new Dictionary<string, PerfGateMeasurement>(StringComparer.Ordinal);
        foreach (var report in summary.Reports)
        {
            var statistics = report.ResultStatistics;
            if (statistics is null)
            {
                continue;
            }

            var descriptor = report.BenchmarkCase.Descriptor;
            var benchmarkId = string.Concat(descriptor.Type.Name, ".", descriptor.WorkloadMethod.Name);
            var meanMs = statistics.Mean / 1_000_000d;
            var allocatedBytes = ResolveAllocatedBytesPerOperation(report.GcStats);
            measurements[benchmarkId] = new PerfGateMeasurement(benchmarkId, meanMs, allocatedBytes);
        }

        return measurements;
    }

    private static PerfGateExecutionResult CompareAgainstBaseline(
        string baselinePath,
        PerfGateBaseline baseline,
        IReadOnlyDictionary<string, PerfGateMeasurement> measurements)
    {
        var scenarioResults = new List<PerfGateScenarioResult>(baseline.Scenarios.Count);
        var allPassed = true;

        for (var index = 0; index < baseline.Scenarios.Count; index++)
        {
            var scenario = baseline.Scenarios[index];
            if (!measurements.TryGetValue(scenario.BenchmarkId, out var measurement))
            {
                allPassed = false;
                scenarioResults.Add(new PerfGateScenarioResult
                {
                    BenchmarkId = scenario.BenchmarkId,
                    MaxMeanMs = scenario.MaxMeanMs,
                    MaxAllocatedBytes = scenario.MaxAllocatedBytes,
                    Pass = false,
                    FailureReason = "missing benchmark measurement",
                });
                continue;
            }

            var meanPass = measurement.MeanMs <= scenario.MaxMeanMs;
            var allocPass = !scenario.MaxAllocatedBytes.HasValue || measurement.AllocatedBytes <= scenario.MaxAllocatedBytes.Value;
            var pass = meanPass && allocPass;
            if (!pass)
            {
                allPassed = false;
            }

            scenarioResults.Add(new PerfGateScenarioResult
            {
                BenchmarkId = scenario.BenchmarkId,
                MeanMs = measurement.MeanMs,
                AllocatedBytes = measurement.AllocatedBytes,
                MaxMeanMs = scenario.MaxMeanMs,
                MaxAllocatedBytes = scenario.MaxAllocatedBytes,
                Pass = pass,
                FailureReason = pass ? null : ResolveFailureReason(meanPass, allocPass),
            });
        }

        return new PerfGateExecutionResult
        {
            Schema = "teasharp-perf-gate-result-v1",
            Status = allPassed ? "pass" : "fail",
            BaselinePath = baselinePath,
            GeneratedAtUtc = DateTimeOffset.UtcNow,
            ScenarioResults = scenarioResults,
        };
    }

    private static PerfGateExecutionResult CreateDryRunResult(string baselinePath, PerfGateBaseline baseline)
    {
        var scenarioResults = new List<PerfGateScenarioResult>(baseline.Scenarios.Count);
        for (var index = 0; index < baseline.Scenarios.Count; index++)
        {
            var scenario = baseline.Scenarios[index];
            scenarioResults.Add(new PerfGateScenarioResult
            {
                BenchmarkId = scenario.BenchmarkId,
                MaxMeanMs = scenario.MaxMeanMs,
                MaxAllocatedBytes = scenario.MaxAllocatedBytes,
                Pass = true,
            });
        }

        return new PerfGateExecutionResult
        {
            Schema = "teasharp-perf-gate-result-v1",
            Status = "dry-run",
            BaselinePath = baselinePath,
            GeneratedAtUtc = DateTimeOffset.UtcNow,
            ScenarioResults = scenarioResults,
        };
    }

    private static string ResolveFailureReason(bool meanPass, bool allocPass)
    {
        if (!meanPass && !allocPass)
        {
            return "mean and allocation limits exceeded";
        }

        if (!meanPass)
        {
            return "mean limit exceeded";
        }

        return "allocation limit exceeded";
    }

    private static void EmitResult(PerfGateExecutionResult result, string? outputPath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        var json = JsonSerializer.Serialize(result, options);
        Console.WriteLine(json);

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return;
        }

        var parent = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(parent))
        {
            Directory.CreateDirectory(parent);
        }

        File.WriteAllText(outputPath, json);
    }

    private static double ResolveAllocatedBytesPerOperation(object gcStats)
    {
        var type = gcStats.GetType();
        var bytes = TryReadDoubleProperty(type, gcStats, "BytesAllocatedPerOperation");
        if (bytes.HasValue)
        {
            return bytes.Value;
        }

        bytes = TryReadDoubleProperty(type, gcStats, "AllocatedBytes");
        if (bytes.HasValue)
        {
            return bytes.Value;
        }

        var totalBytes = TryReadDoubleProperty(type, gcStats, "TotalOperations");
        var operations = TryReadDoubleProperty(type, gcStats, "Operations");
        if (totalBytes.HasValue && operations.HasValue && operations.Value > 0d)
        {
            return totalBytes.Value / operations.Value;
        }

        return 0d;
    }

    private static double? TryReadDoubleProperty(Type type, object instance, string propertyName)
    {
        var property = type.GetProperty(propertyName);
        if (property is null)
        {
            return null;
        }

        var value = property.GetValue(instance);
        return value switch
        {
            null => null,
            byte numeric => numeric,
            sbyte numeric => numeric,
            short numeric => numeric,
            ushort numeric => numeric,
            int numeric => numeric,
            uint numeric => numeric,
            long numeric => numeric,
            ulong numeric => numeric,
            float numeric => numeric,
            double numeric => numeric,
            decimal numeric => (double)numeric,
            _ => null,
        };
    }

    private static PerfGateBaseline LoadBaseline(string baselinePath)
    {
        var json = File.ReadAllText(baselinePath);
        var baseline = JsonSerializer.Deserialize<PerfGateBaseline>(json);
        if (baseline is null || baseline.Scenarios is null || baseline.Scenarios.Count == 0)
        {
            throw new InvalidOperationException($"Invalid perf baseline file: {baselinePath}");
        }

        return baseline;
    }

    private static bool ContainsFlag(IReadOnlyList<string> args, string flag)
    {
        for (var index = 0; index < args.Count; index++)
        {
            if (string.Equals(args[index], flag, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReadValue(IReadOnlyList<string> args, ref int index, out string? value)
    {
        var valueIndex = index + 1;
        if (valueIndex >= args.Count)
        {
            value = null;
            return false;
        }

        value = args[valueIndex];
        index = valueIndex;
        return true;
    }
}

internal sealed class PerfGateBaseline
{
    public string Schema { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public List<PerfGateBaselineScenario> Scenarios { get; init; } = [];
}

internal sealed class PerfGateBaselineScenario
{
    public string BenchmarkId { get; init; } = string.Empty;

    public double MaxMeanMs { get; init; }

    public double? MaxAllocatedBytes { get; init; }
}

internal readonly record struct PerfGateMeasurement(
    string BenchmarkId,
    double MeanMs,
    double AllocatedBytes);

internal sealed class PerfGateExecutionResult
{
    public string Schema { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string BaselinePath { get; init; } = string.Empty;

    public DateTimeOffset GeneratedAtUtc { get; init; }

    public List<PerfGateScenarioResult> ScenarioResults { get; init; } = [];
}

internal sealed class PerfGateScenarioResult
{
    public string BenchmarkId { get; init; } = string.Empty;

    public double? MeanMs { get; init; }

    public double? AllocatedBytes { get; init; }

    public double MaxMeanMs { get; init; }

    public double? MaxAllocatedBytes { get; init; }

    public bool Pass { get; init; }

    public string? FailureReason { get; init; }
}
