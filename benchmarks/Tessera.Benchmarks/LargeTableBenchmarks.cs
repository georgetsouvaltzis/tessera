using BenchmarkDotNet.Attributes;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Benchmarks;

[MemoryDiagnoser]
public class LargeTableBenchmarks
{
    private readonly Rect _bounds = new(0, 0, 180, 44);

    private readonly DataGrid _grid = new() { Border = BorderStyle.SingleLine, ShowHeader = true };

    [GlobalSetup]
    public void Setup()
    {
        _grid.SetColumns(BenchmarkDataFactory.CreateColumns(16, 10));
        _grid.SetRows(BenchmarkDataFactory.CreateRows(2_000, 16, 4242));
        _grid.SelectCell(500, 8);
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
