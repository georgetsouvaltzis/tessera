using BenchmarkDotNet.Attributes;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Benchmarks;

[MemoryDiagnoser]
public class StyledHeavyOutputBenchmarks
{
    private readonly DataGrid _grid = new()
    {
        Border = BorderStyle.SingleLine,
        ShowHeader = true,
        TitleStyle = TesseraStyle.Empty.WithBold().WithForeground(AnsiColor.BrightBlue),
        FocusedTitleStyle = TesseraStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
        HeaderStyle = TesseraStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow),
        RowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightWhite),
        SelectedRowStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(52, 58, 64)),
        SelectedCellStyle = TesseraStyle.Empty.WithBold().WithForeground(AnsiColor.BrightCyan),
        MutedStyle = TesseraStyle.Empty.WithDim().WithForeground(AnsiColor.BrightBlack),
        DisabledStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightBlack),
        MutedRowPredicate = static (rowIndex, _) => rowIndex % 3 == 0,
    };

    private readonly Rect _bounds = new(0, 0, 160, 40);

    [GlobalSetup]
    public void Setup()
    {
        _grid.SetColumns(BenchmarkDataFactory.CreateColumns(count: 12, width: 11));
        _grid.SetRows(BenchmarkDataFactory.CreateRows(rowCount: 1_200, columnCount: 12, seed: 2026));
        _grid.SelectCell(rowIndex: 320, columnIndex: 4);
    }

    [Benchmark(Description = "styled-heavy output render")]
    public int RenderStyledHeavyFrame()
    {
        var canvas = RenderStyledHeavyFrameCore();
        return canvas.Render().Length;
    }

    [Benchmark(Description = "styled-heavy output render-only (no materialization)")]
    public int RenderStyledHeavyFrameOnly()
    {
        var canvas = RenderStyledHeavyFrameCore();
        return canvas.Bounds.Width * canvas.Bounds.Height;
    }

    private Canvas RenderStyledHeavyFrameCore()
    {
        var canvas = new Canvas(_bounds.Width, _bounds.Height);
        _grid.Render(canvas, _bounds);
        return canvas;
    }
}
