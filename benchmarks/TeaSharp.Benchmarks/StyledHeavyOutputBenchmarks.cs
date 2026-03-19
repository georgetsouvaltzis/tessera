using BenchmarkDotNet.Attributes;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Benchmarks;

[MemoryDiagnoser]
public sealed class StyledHeavyOutputBenchmarks
{
    private readonly DataGrid _grid = new()
    {
        Border = BorderStyle.SingleLine,
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
        var canvas = new Canvas(_bounds.Width, _bounds.Height);
        _grid.Render(canvas, _bounds);
        return canvas.Render().Length;
    }
}
