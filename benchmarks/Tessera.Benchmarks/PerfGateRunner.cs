using System.Text.Json;

using System.Diagnostics.CodeAnalysis;

namespace Tessera.Benchmarks;

internal static class PerfGateRunner
{
    private const string GateFlag = "--perf-gate";
    private const string BaselineSchema = "tessera-perf-gate-baseline-v1";
    private const int WarmupCount = 2;
    private const int MeasurementCount = 10;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

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
        var measurements = CollectMeasurements();
        return CompareAgainstBaseline(baselinePath, baseline, measurements);
    }

    private static Dictionary<string, PerfGateMeasurement> CollectMeasurements()
    {
        var scenarios = new PerfGateScenario[]
        {
            new(
                "SloLatencyBenchmarks.StartupFirstFrameP95Ms",
                static () =>
                {
                    var benchmarks = new SloLatencyBenchmarks();
                    benchmarks.Setup();
                    return benchmarks.StartupFirstFrameP95Ms;
                }),
            new(
                "SloLatencyBenchmarks.InputLatencyNormalP95Ms",
                static () =>
                {
                    var benchmarks = new SloLatencyBenchmarks();
                    benchmarks.Setup();
                    return benchmarks.InputLatencyNormalP95Ms;
                }),
            new(
                "SloLatencyBenchmarks.InputLatencyHeavyP95Ms",
                static () =>
                {
                    var benchmarks = new SloLatencyBenchmarks();
                    benchmarks.Setup();
                    return benchmarks.InputLatencyHeavyP95Ms;
                }),
        };

        var measurements = new Dictionary<string, PerfGateMeasurement>(scenarios.Length, StringComparer.Ordinal);
        for (var index = 0; index < scenarios.Length; index++)
        {
            var scenario = scenarios[index];
            measurements[scenario.BenchmarkId] = MeasureScenario(scenario);
        }

        return measurements;
    }

    private static PerfGateMeasurement MeasureScenario(PerfGateScenario scenario)
    {
        var execute = scenario.CreateWorkload();
        for (var warmup = 0; warmup < WarmupCount; warmup++)
        {
            _ = execute();
        }

        Span<double> samples = stackalloc double[MeasurementCount];
        Span<double> allocationSamples = stackalloc double[MeasurementCount];
        for (var measurement = 0; measurement < MeasurementCount; measurement++)
        {
            var beforeAlloc = GC.GetAllocatedBytesForCurrentThread();
            samples[measurement] = execute();
            var afterAlloc = GC.GetAllocatedBytesForCurrentThread();
            allocationSamples[measurement] = afterAlloc - beforeAlloc;
        }

        return new PerfGateMeasurement(
            scenario.BenchmarkId,
            ResolveMean(samples),
            ResolveMean(allocationSamples));
    }

    private static double ResolveMean(ReadOnlySpan<double> samples)
    {
        if (samples.Length == 0)
        {
            return 0d;
        }

        var total = 0d;
        for (var index = 0; index < samples.Length; index++)
        {
            total += samples[index];
        }

        return total / samples.Length;
    }

    private static PerfGateExecutionResult CompareAgainstBaseline(
        string baselinePath,
        PerfGateBaseline baseline,
        Dictionary<string, PerfGateMeasurement> measurements)
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
            Schema = "tessera-perf-gate-result-v1",
            Status = allPassed ? "pass" : "fail",
            BaselinePath = baselinePath,
            GeneratedAtUtc = DateTimeOffset.UtcNow,
            Runner = "direct-slo-runner",
            WarmupCount = WarmupCount,
            MeasurementCount = MeasurementCount,
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
            Schema = "tessera-perf-gate-result-v1",
            Status = "dry-run",
            BaselinePath = baselinePath,
            GeneratedAtUtc = DateTimeOffset.UtcNow,
            Runner = "direct-slo-runner",
            WarmupCount = WarmupCount,
            MeasurementCount = MeasurementCount,
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
        var json = JsonSerializer.Serialize(result, JsonOptions);
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

    private static PerfGateBaseline LoadBaseline(string baselinePath)
    {
        var json = File.ReadAllText(baselinePath);
        var baseline = JsonSerializer.Deserialize<PerfGateBaseline>(json, JsonOptions);
        if (baseline is null || baseline.Scenarios is null || baseline.Scenarios.Count == 0)
        {
            throw new InvalidOperationException($"Invalid perf baseline file: {baselinePath}");
        }

        if (!string.Equals(baseline.Schema, BaselineSchema, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Invalid perf baseline schema '{baseline.Schema}'. Expected '{BaselineSchema}'.");
        }

        for (var index = 0; index < baseline.Scenarios.Count; index++)
        {
            var scenario = baseline.Scenarios[index];
            if (string.IsNullOrWhiteSpace(scenario.BenchmarkId))
            {
                throw new InvalidOperationException($"Baseline scenario[{index}] missing benchmarkId.");
            }

            if (scenario.MaxMeanMs <= 0d || double.IsNaN(scenario.MaxMeanMs))
            {
                throw new InvalidOperationException(
                    $"Baseline scenario[{index}] has invalid maxMeanMs '{scenario.MaxMeanMs}'.");
            }
        }

        return baseline;
    }

    private static bool ContainsFlag(string[] args, string flag)
    {
        for (var index = 0; index < args.Length; index++)
        {
            if (string.Equals(args[index], flag, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReadValue(string[] args, ref int index, out string? value)
    {
        var valueIndex = index + 1;
        if (valueIndex >= args.Length)
        {
            value = null;
            return false;
        }

        value = args[valueIndex];
        index = valueIndex;
        return true;
    }
}

internal readonly record struct PerfGateScenario(
    string BenchmarkId,
    Func<Func<double>> CreateWorkload);

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

    public string Runner { get; init; } = string.Empty;

    public int WarmupCount { get; init; }

    public int MeasurementCount { get; init; }

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
