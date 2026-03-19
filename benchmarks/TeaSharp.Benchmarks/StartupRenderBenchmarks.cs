using BenchmarkDotNet.Attributes;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Benchmarks;

[MemoryDiagnoser]
public sealed class StartupRenderBenchmarks
{
    [Benchmark(Description = "startup-ish first-frame render baseline")]
    public int StartupLike_FirstFrame_Render()
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

        var canvas = new Canvas(120, 36);
        grid.Render(canvas, new Rect(0, 0, 120, 24));
        inspector.Render(canvas, new Rect(0, 24, 120, 10));
        status.Render(canvas, new Rect(0, 35, 120, 1));
        return canvas.Render().Length;
    }
}
