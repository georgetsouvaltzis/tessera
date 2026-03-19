using BenchmarkDotNet.Attributes;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Benchmarks;

[MemoryDiagnoser]
public class StartupRenderBenchmarks
{
    private readonly Rect _gridBounds = new(0, 0, 120, 24);
    private readonly Rect _inspectorBounds = new(0, 24, 120, 10);
    private readonly Rect _statusBounds = new(0, 35, 120, 1);

    [Benchmark(Description = "startup-ish first-frame render baseline")]
    public int StartupLikeFirstFrameRender()
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

        var canvas = new Canvas(_gridBounds.Width, _statusBounds.Bottom + 1);
        grid.Render(canvas, _gridBounds);
        inspector.Render(canvas, _inspectorBounds);
        status.Render(canvas, _statusBounds);
        return canvas.Render().Length;
    }
}
