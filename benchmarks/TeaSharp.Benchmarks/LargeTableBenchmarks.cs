using BenchmarkDotNet.Attributes;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Benchmarks;

[MemoryDiagnoser]
public class LargeTableBenchmarks
{
    private readonly DataGrid _grid = new()
    {
        Border = BorderStyle.SingleLine,
        ShowHeader = true,
    };

    private readonly Rect _bounds = new(0, 0, 180, 44);

    [GlobalSetup]
    public void Setup()
    {
        _grid.SetColumns(BenchmarkDataFactory.CreateColumns(count: 16, width: 10));
        _grid.SetRows(BenchmarkDataFactory.CreateRows(rowCount: 2_000, columnCount: 16, seed: 4242));
        _grid.SelectCell(rowIndex: 500, columnIndex: 8);
    }

    [Benchmark(Description = "large-table render (2k x 16)")]
    public int RenderLargeTableFrame()
    {
        var canvas = RenderLargeTableFrameCore();
        return canvas.Render().Length;
    }

    [Benchmark(Description = "large-table render-only (2k x 16, no materialization)")]
    public int RenderLargeTableFrameOnly()
    {
        var canvas = RenderLargeTableFrameCore();
        return canvas.Bounds.Width * canvas.Bounds.Height;
    }

    private Canvas RenderLargeTableFrameCore()
    {
        var canvas = new Canvas(_bounds.Width, _bounds.Height);
        _grid.Render(canvas, _bounds);
        return canvas;
    }
}
