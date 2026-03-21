using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Benchmarks;

[MemoryDiagnoser]
public sealed class SloLatencyBenchmarks
{
    private const int StartupSamples = 24;
    private const int InputSamples = 128;
    private static readonly KeyPressed UpKey = new(Key.Up);
    private static readonly KeyPressed DownKey = new(Key.Down);
    private readonly Rect _startupGridBounds = new(0, 0, 120, 24);
    private readonly Rect _startupInspectorBounds = new(0, 24, 120, 10);
    private readonly Rect _startupStatusBounds = new(0, 35, 120, 1);
    private readonly ListView<string> _normalList = new(item => item)
    {
        Border = BorderStyle.SingleLine,
        IsFocused = true,
    };

    private readonly Rect _normalBounds = new(0, 0, 120, 24);
    private readonly Canvas _normalCanvas = new(120, 24);
    private readonly DataGrid _heavyGrid = new()
    {
        Border = BorderStyle.SingleLine,
        IsFocused = true,
        ShowHeader = true,
        TitleStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightBlue),
        FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
        HeaderStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow),
        RowStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightWhite),
        SelectedRowStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(52, 58, 64)),
        SelectedCellStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightCyan),
        MutedStyle = TeaStyle.Empty.WithDim().WithForeground(AnsiColor.BrightBlack),
        DisabledStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
        MutedRowPredicate = static (rowIndex, _) => rowIndex % 3 == 0,
    };

    private readonly Rect _heavyBounds = new(0, 0, 160, 40);
    private readonly Canvas _heavyCanvas = new(160, 40);

    [GlobalSetup]
    public void Setup()
    {
        var normalItems = new string[256];
        for (var index = 0; index < normalItems.Length; index++)
        {
            normalItems[index] = $"row-{index:D3}";
        }

        _normalList.SetItems(normalItems);
        _ = _normalList.Handle(DownKey);
        _ = _normalList.Handle(UpKey);

        _heavyGrid.SetColumns(BenchmarkDataFactory.CreateColumns(count: 12, width: 11));
        _heavyGrid.SetRows(BenchmarkDataFactory.CreateRows(rowCount: 1_200, columnCount: 12, seed: 2026));
        _heavyGrid.SelectCell(rowIndex: 320, columnIndex: 4);
    }

    [Benchmark(Description = "startup first-frame p95 (ms)")]
    public double StartupFirstFrameP95Ms()
    {
        Span<long> samples = stackalloc long[StartupSamples];
        for (var index = 0; index < samples.Length; index++)
        {
            samples[index] = MeasureStartupFrameTicks();
        }

        return ResolveP95Milliseconds(samples);
    }

    [Benchmark(Description = "input-latency p95 normal-load (ms)")]
    public double InputLatencyNormalP95Ms()
    {
        Span<long> samples = stackalloc long[InputSamples];
        for (var index = 0; index < samples.Length; index++)
        {
            var key = (index & 1) == 0 ? DownKey : UpKey;
            var started = Stopwatch.GetTimestamp();
            _normalList.Handle(key);
            _normalCanvas.Clear();
            _normalList.Render(_normalCanvas, _normalBounds);
            _ = _normalCanvas.Render();
            samples[index] = Stopwatch.GetTimestamp() - started;
        }

        return ResolveP95Milliseconds(samples);
    }

    [Benchmark(Description = "input-latency p95 heavy-load (ms)")]
    public double InputLatencyHeavyP95Ms()
    {
        Span<long> samples = stackalloc long[InputSamples];
        for (var index = 0; index < samples.Length; index++)
        {
            var key = (index & 1) == 0 ? DownKey : UpKey;
            var started = Stopwatch.GetTimestamp();
            _heavyGrid.Handle(key);
            _heavyCanvas.Clear();
            _heavyGrid.Render(_heavyCanvas, _heavyBounds);
            _ = _heavyCanvas.Render();
            samples[index] = Stopwatch.GetTimestamp() - started;
        }

        return ResolveP95Milliseconds(samples);
    }

    private long MeasureStartupFrameTicks()
    {
        var grid = new DataGrid
        {
            Border = BorderStyle.SingleLine,
            ShowHeader = true,
        };

        grid.SetColumns(BenchmarkDataFactory.CreateColumns(count: 6, width: 14));
        grid.SetRows(BenchmarkDataFactory.CreateRows(rowCount: 28, columnCount: 6, seed: 1337));

        var inspector = new KeyValueList
        {
            Border = BorderStyle.SingleLine,
        };

        inspector.SetEntries(BenchmarkDataFactory.CreateInspectorEntries(count: 10));

        var status = new StatusBar
        {
            LeftText = "TeaSharp benchmark",
            RightText = "startup",
        };

        var canvas = new Canvas(_startupGridBounds.Width, _startupStatusBounds.Bottom + 1);
        var started = Stopwatch.GetTimestamp();
        grid.Render(canvas, _startupGridBounds);
        inspector.Render(canvas, _startupInspectorBounds);
        status.Render(canvas, _startupStatusBounds);
        _ = canvas.Render();
        return Stopwatch.GetTimestamp() - started;
    }

    private static double ResolveP95Milliseconds(Span<long> samples)
    {
        samples.Sort();
        var p95Index = (int)Math.Ceiling(samples.Length * 0.95) - 1;
        p95Index = Math.Clamp(p95Index, 0, samples.Length - 1);
        return (samples[p95Index] * 1000d) / Stopwatch.Frequency;
    }
}
