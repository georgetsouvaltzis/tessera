using BenchmarkDotNet.Attributes;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Benchmarks;

[MemoryDiagnoser]
public class ResizeStormBenchmarks
{
    private const int ResizeIterations = 72;
    private readonly DataGrid _grid = new()
    {
        Border = BorderStyle.SingleLine,
        ShowHeader = true,
    };

    private readonly MarkdownView _markdownView = new()
    {
        Border = BorderStyle.SingleLine,
        ShowLineNumbers = true,
    };

    private readonly MiniLog _miniLog = new(capacity: 80);
    private readonly StatusBar _statusBar = new()
    {
        LeftText = "resize storm",
        RightText = "recompose",
    };

    private readonly (int Width, int Height)[] _sizes = BuildSizeSequence();

    [GlobalSetup]
    public void Setup()
    {
        _grid.SetColumns(BenchmarkDataFactory.CreateColumns(count: 10, width: 10));
        _grid.SetRows(BenchmarkDataFactory.CreateRows(rowCount: 900, columnCount: 10, seed: 6060));
        _grid.SelectCell(rowIndex: 220, columnIndex: 4);

        _markdownView.SetMarkdown(
            """
            # Resize Storm
            Deterministic layout recomposition benchmark.
            - fixed-size loops
            - repeated bounds changes
            - mixed controls
            """);

        _miniLog.Clear();
        for (var index = 0; index < 80; index++)
        {
            _miniLog.Append($"evt-{index:D3} size-check {(index * 37 + 5) % 997:D3}");
        }
    }

    [Benchmark(Description = "resize storm repeated recomposition")]
    public int RenderResizeStormFrames()
    {
        return RenderResizeStormFramesCore(materialize: true);
    }

    [Benchmark(Description = "resize storm repeated recomposition render-only")]
    public int RenderResizeStormFramesOnly()
    {
        return RenderResizeStormFramesCore(materialize: false);
    }

    private int RenderResizeStormFramesCore(bool materialize)
    {
        var totalLength = 0;
        for (var iteration = 0; iteration < ResizeIterations; iteration++)
        {
            var size = _sizes[iteration % _sizes.Length];
            var width = size.Width;
            var height = size.Height;

            _grid.Handle(new WindowResized(width, height));
            _markdownView.Handle(new WindowResized(width, height));
            _miniLog.Handle(new WindowResized(width, height));

            var canvas = new Canvas(width, height);
            var statusBounds = new Rect(0, height - 1, width, 1);
            var bodyHeight = Math.Max(1, height - 1);
            var topHeight = Math.Max(6, bodyHeight * 2 / 3);
            var bottomHeight = Math.Max(1, bodyHeight - topHeight);
            var leftWidth = Math.Max(20, width / 2);
            var rightWidth = Math.Max(1, width - leftWidth);

            _grid.Render(canvas, new Rect(0, 0, width, topHeight));
            _markdownView.Render(canvas, new Rect(0, topHeight, leftWidth, bottomHeight));
            _miniLog.Render(canvas, new Rect(leftWidth, topHeight, rightWidth, bottomHeight));
            _statusBar.Render(canvas, statusBounds);

            totalLength += materialize
                ? canvas.Render().Length
                : canvas.Bounds.Width * canvas.Bounds.Height;
        }

        return totalLength;
    }

    private static (int Width, int Height)[] BuildSizeSequence()
    {
        var sizes = new (int Width, int Height)[12];
        for (var index = 0; index < sizes.Length; index++)
        {
            var width = 90 + ((index * 17) % 70);
            var height = 24 + ((index * 11) % 20);
            sizes[index] = (width, height);
        }

        return sizes;
    }
}
