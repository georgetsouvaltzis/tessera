using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Tessera.Controls;
using Tessera.Hosting;
using Tessera.Layout;

namespace Tessera.Benchmarks;

internal static class RuntimeEndToEndRunner
{
    private const string RunnerFlag = "--runtime-e2e";
    private const int WarmupCount = 2;
    private const int MeasurementCount = 10;
    private const int InputEventsPerRun = 24;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public static bool TryRun(string[] args, out int exitCode)
    {
        exitCode = 0;
        if (!ContainsFlag(args, RunnerFlag))
        {
            return false;
        }

        string? outputPath = null;
        for (var index = 0; index < args.Length; index++)
        {
            if (!string.Equals(args[index], "--output", StringComparison.Ordinal))
            {
                continue;
            }

            if (!TryReadValue(args, ref index, out outputPath))
            {
                Console.Error.WriteLine("Missing value for --output.");
                exitCode = 1;
                return true;
            }
        }

        var result = Execute();
        EmitResult(result, outputPath);
        exitCode = 0;
        return true;
    }

    private static RuntimeEndToEndResult Execute()
    {
        for (var warmup = 0; warmup < WarmupCount; warmup++)
        {
            _ = RunIteration();
        }

        Span<double> startupSamples = stackalloc double[MeasurementCount];
        Span<double> runSamples = stackalloc double[MeasurementCount];
        Span<double> flushSamples = stackalloc double[MeasurementCount];
        Span<double> outputByteSamples = stackalloc double[MeasurementCount];
        var decodedInputs = 0;

        for (var measurement = 0; measurement < MeasurementCount; measurement++)
        {
            var result = RunIteration();
            startupSamples[measurement] = result.StartupFirstFlushMs;
            runSamples[measurement] = result.TotalRunMs;
            flushSamples[measurement] = result.FlushCount;
            outputByteSamples[measurement] = result.OutputBytes;
            decodedInputs = result.DecodedInputs;
        }

        return new RuntimeEndToEndResult
        {
            Schema = "tessera-runtime-e2e-result-v1",
            Status = "measured",
            WarmupCount = WarmupCount,
            MeasurementCount = MeasurementCount,
            InputEventsPerRun = InputEventsPerRun,
            GeneratedAtUtc = DateTimeOffset.UtcNow,
            StartupFirstFlushMeanMs = ResolveMean(startupSamples),
            StartupFirstFlushP95Ms = ResolveP95(startupSamples),
            TotalRunMeanMs = ResolveMean(runSamples),
            TotalRunP95Ms = ResolveP95(runSamples),
            MeanFlushCount = ResolveMean(flushSamples),
            MeanOutputBytes = ResolveMean(outputByteSamples),
            DecodedInputsPerRun = decodedInputs,
        };
    }

    private static RuntimeEndToEndIterationResult RunIteration()
    {
        var terminal = new ScriptedTerminalAdapter(BuildInputPayload(InputEventsPerRun));
        using var stopwatch = new RunStopwatch();
        var renderer = new MeasuringRenderer(stopwatch);

        try
        {
            var app = new RuntimeEndToEndProbeApp(InputEventsPerRun);
            var options = new TesseraRuntimeOptions
            {
                AdaptiveFramePacing = false,
                MaxFps = 1000,
                UseConsoleKeyEvents = false,
                EnableResizeSignals = false,
            };
            var hosting = new TesseraHostingOptions
            {
                EnableCapabilityProbe = false,
                Renderer = renderer,
                Terminal = terminal,
                EventDecoder = new EventDecoder(),
                TerminalCapabilities = TerminalCapabilityProfile.AllSupported,
                ColorProfile = TerminalColorProfile.TrueColor,
            };

            var application = TesseraHost.CreateApplication(app, options, hosting);
            stopwatch.Start();
            application.RunAsync().GetAwaiter().GetResult();
            stopwatch.Stop();

            return new RuntimeEndToEndIterationResult(
                renderer.FirstFlushMs,
                stopwatch.ElapsedMs,
                renderer.FlushCount,
                terminal.OutputBytes,
                app.ProcessedInputs);
        }
        finally
        {
            renderer.DisposeAsync().AsTask().GetAwaiter().GetResult();
            terminal.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    private static byte[] BuildInputPayload(int inputEvents)
    {
        var payload = new byte[inputEvents * 3];
        for (var index = 0; index < inputEvents; index++)
        {
            var offset = index * 3;
            payload[offset] = 0x1B;
            payload[offset + 1] = (byte)'[';
            payload[offset + 2] = (byte)'B';
        }

        return payload;
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

    private static double ResolveP95(Span<double> samples)
    {
        samples.Sort();
        var p95Index = (int)Math.Ceiling(samples.Length * 0.95d) - 1;
        p95Index = Math.Clamp(p95Index, 0, samples.Length - 1);
        return samples[p95Index];
    }

    private static void EmitResult(RuntimeEndToEndResult result, string? outputPath)
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

    private sealed class RuntimeEndToEndProbeApp(int targetInputs) : TesseraApp
    {
        private readonly Label _label = new()
        {
            Border = BorderStyle.Rounded,
            Padding = Thickness.All(1),
        };

        public int ProcessedInputs { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            if (message is KeyPressed key && key.Is(Key.Down))
            {
                ProcessedInputs++;
                _label.Text = $"runtime-e2e inputs={ProcessedInputs}/{targetInputs}";
                if (ProcessedInputs >= targetInputs)
                {
                    return TesseraEffects.Quit;
                }
            }

            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            _label.Text = $"runtime-e2e inputs={ProcessedInputs}/{targetInputs}";
            return Screen.Build(window =>
            {
                window.Padding(1);
                window.Body(new CenterLayout
                {
                    Content = _label,
                    Width = Math.Min(42, Math.Max(24, context.Width - 4)),
                    Height = 5,
                });
            });
        }
    }

    private sealed class ScriptedTerminalAdapter(byte[] payload) : ITerminalAdapter
    {
        private readonly MemoryStream _input = new(payload, writable: false);
        private readonly RetainedBufferStream _output = new();

        public Stream Input => _input;

        public Stream Output => _output;

        public bool IsInputInteractive => true;

        public bool IsOutputInteractive => true;

        public int OutputBytes => checked((int)_output.Length);

        public ValueTask PrepareAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask RestoreAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken) => ValueTask.FromResult(new TerminalSize(100, 24));

        public ValueTask DisposeAsync()
        {
            _input.Dispose();
            _output.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    private sealed class RetainedBufferStream : Stream
    {
        private readonly MemoryStream _inner = new();

        public override bool CanRead => _inner.CanRead;

        public override bool CanSeek => _inner.CanSeek;

        public override bool CanWrite => _inner.CanWrite;

        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() => _inner.Flush();

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);

        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
            _inner.WriteAsync(buffer, cancellationToken);

        public override Task FlushAsync(CancellationToken cancellationToken) => _inner.FlushAsync(cancellationToken);

        protected override void Dispose(bool disposing)
        {
            // Keep the buffer readable after runtime cleanup so the probe can report flushed bytes.
            base.Dispose(disposing);
        }
    }

    private sealed class MeasuringRenderer(RunStopwatch stopwatch) : IProgramRenderer
    {
        private readonly AnsiDiffRenderer _inner = new(TerminalCapabilityProfile.AllSupported);

        public double FirstFlushMs { get; private set; }

        public int FlushCount { get; private set; }

        public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken) =>
            _inner.InitializeAsync(output, cancellationToken);

        public void Resize(int width, int height) => _inner.Resize(width, height);

        public void UpdateCapabilities(TerminalCapabilityProfile capabilities) => _inner.UpdateCapabilities(capabilities);

        public void Render(RenderOutput output) => _inner.Render(output);

        public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken) =>
            _inner.WriteRawAsync(content, cancellationToken);

        public async ValueTask FlushAsync(CancellationToken cancellationToken)
        {
            await _inner.FlushAsync(cancellationToken).ConfigureAwait(false);
            FlushCount++;
            if (FirstFlushMs <= 0d)
            {
                FirstFlushMs = stopwatch.ElapsedMs;
            }
        }

        public ValueTask ResetAsync(CancellationToken cancellationToken) => _inner.ResetAsync(cancellationToken);

        public ValueTask DisposeAsync() => _inner.DisposeAsync();
    }

    private sealed class RunStopwatch : IDisposable
    {
        private readonly Stopwatch _stopwatch = new();

        public double ElapsedMs => _stopwatch.Elapsed.TotalMilliseconds;

        public void Start() => _stopwatch.Start();

        public void Stop() => _stopwatch.Stop();

        public void Dispose() => _stopwatch.Stop();
    }
}

internal readonly record struct RuntimeEndToEndIterationResult(
    double StartupFirstFlushMs,
    double TotalRunMs,
    int FlushCount,
    int OutputBytes,
    int DecodedInputs);

internal sealed class RuntimeEndToEndResult
{
    public string Schema { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public DateTimeOffset GeneratedAtUtc { get; init; }

    public int WarmupCount { get; init; }

    public int MeasurementCount { get; init; }

    public int InputEventsPerRun { get; init; }

    public double StartupFirstFlushMeanMs { get; init; }

    public double StartupFirstFlushP95Ms { get; init; }

    public double TotalRunMeanMs { get; init; }

    public double TotalRunP95Ms { get; init; }

    public double MeanFlushCount { get; init; }

    public double MeanOutputBytes { get; init; }

    public int DecodedInputsPerRun { get; init; }
}
