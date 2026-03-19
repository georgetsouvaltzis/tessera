using BenchmarkDotNet.Attributes;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Benchmarks;

[MemoryDiagnoser]
public class LogTailStreamBenchmarks
{
    private const int AppendBatchSize = 2_048;
    private readonly string[] _seedLines = CreateSeedLines();
    private readonly LogView _logView = new()
    {
        Border = BorderStyle.SingleLine,
        AutoScroll = true,
        IsFocused = true,
    };

    private readonly Rect _bounds = new(0, 0, 160, 42);

    [Benchmark(Description = "log-tail stream append + scroll workload")]
    public int AppendAndScrollLogTail()
    {
        return AppendAndScrollLogTailCore(materialize: true);
    }

    [Benchmark(Description = "log-tail stream append + scroll render-only (no materialization)")]
    public int AppendAndScrollLogTailOnly()
    {
        return AppendAndScrollLogTailCore(materialize: false);
    }

    private int AppendAndScrollLogTailCore(bool materialize)
    {
        _logView.Clear();
        for (var index = 0; index < AppendBatchSize; index++)
        {
            _logView.Append(_seedLines[index % _seedLines.Length]);
            if ((index & 31) == 7)
            {
                _logView.Handle(new KeyPressed(Key.Up));
            }
            else if ((index & 31) == 23)
            {
                _logView.Handle(new KeyPressed(Key.Down));
            }
        }

        var canvas = new Canvas(_bounds.Width, _bounds.Height);
        _logView.Render(canvas, _bounds);
        return materialize
            ? canvas.Render().Length
            : canvas.Bounds.Width * canvas.Bounds.Height;
    }

    private static string[] CreateSeedLines()
    {
        const int count = 256;
        var lines = new string[count];
        for (var index = 0; index < count; index++)
        {
            var level = (index % 5) switch
            {
                0 => "TRACE",
                1 => "DEBUG",
                2 => "INFO",
                3 => "WARN",
                _ => "ERROR",
            };
            lines[index] = $"[{level}] stream={index % 9:D2} msg={index:D4} token={(index * 17 + 13) % 10_000:D4}";
        }

        return lines;
    }
}
