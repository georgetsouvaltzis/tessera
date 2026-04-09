using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class HeatmapControlTests
{
    [Test]
    public void ControlsHeatmapRendersTitleHeadersLegendAndCells()
    {
        var control = new Heatmap
        {
            Title = "Utilization",
            ShowLegend = true,
            ShowRowLabels = true,
            ShowColumnLabels = true,
        };
        control.SetMatrix(new double[,] { { 1, 2, 3 }, { 4, 5, 9 } });
        control.SetRowLabels(["api", "db"]);
        control.SetColumnLabels(["n1", "n2", "n3"]);

        var output = Render(control, 56, 12);

        TestAssert.True(output.Contains(" Utilization ", StringComparison.Ordinal), "Heatmap should render title.");
        TestAssert.True(output.Contains("api", StringComparison.Ordinal), "Heatmap should render row labels.");
        TestAssert.True(output.Contains("n", StringComparison.Ordinal), "Heatmap should render column label glyphs.");
        TestAssert.True(output.Contains("low", StringComparison.Ordinal), "Heatmap should render implicit legend text.");
        TestAssert.True(output.Contains('█') || output.Contains('▓') || output.Contains('▒') || output.Contains('░'), "Heatmap should render heat glyphs.");
    }

    [Test]
    public void ControlsHeatmapKeyboardAndPointerSelectionRaisesSelectionChanged()
    {
        var control = new Heatmap
        {
            Border = BorderStyle.None,
            ShowLegend = false,
            ShowRowLabels = false,
            ShowColumnLabels = false,
            IsFocused = true,
        };
        control.SetMatrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });

        var changes = 0;
        control.SelectionChanged += (_, _) => changes++;

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var rightHandled = control.Handle(new KeyPressed(Key.Right));
        var pointerHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 2, Y: 1),
            new Rect(0, 0, 8, 6));

        TestAssert.True(downHandled, "Down key should change selected row.");
        TestAssert.True(rightHandled, "Right key should change selected column.");
        TestAssert.True(pointerHandled, "Pointer press should select clicked cell.");
        TestAssert.Equal(1, control.SelectedRow, "Pointer press should select row under pointer.");
        TestAssert.Equal(2, control.SelectedColumn, "Pointer press should select column under pointer.");
        TestAssert.True(changes >= 3, "Selection changed event should fire for keyboard and pointer updates.");
    }

    [Test]
    public void ControlsHeatmapStyledCellAndFocusedBorderEmitAnsi()
    {
        var highStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 22, 33));
        var selectedStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(44, 55, 66));
        var focusedBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(77, 88, 99));
        var control = new Heatmap
        {
            Border = BorderStyle.SingleLine,
            ShowLegend = false,
            ShowRowLabels = false,
            ShowColumnLabels = false,
            IsFocused = true,
            HighCellStyle = highStyle,
            PeakCellStyle = highStyle,
            SelectedCellStyle = selectedStyle,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
            FocusedBorderStyleText = focusedBorderStyle,
        };
        control.SetMatrix(new double[,] { { 1, 9 } });
        _ = control.SetSelectedCell(0, 1);

        var output = Render(control, 24, 8, CanvasTextMode.GraphemeAware);

        TestAssert.True(output.Contains("38;2;11;22;33", StringComparison.Ordinal), "Heatmap high cell style should render foreground ANSI sequence.");
        TestAssert.True(output.Contains("48;2;44;55;66", StringComparison.Ordinal), "Heatmap selected style should render background ANSI sequence.");
        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "Heatmap focused border style should render on frame glyphs.");
    }

    [Test]
    public void ControlsHeatmapDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new Heatmap
        {
            ShowLegend = false,
            ShowRowLabels = false,
            ShowColumnLabels = false,
        };
        control.SetMatrix(new double[,] { { 1, 4, 9 }, { 2, 5, 8 } });

        var first = Render(control, 20, 8);
        var second = Render(control, 20, 8);

        TestAssert.Equal(first, second, "Heatmap should render deterministically for identical state.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default heatmap output should remain monochrome.");
    }

    [Test]
    public void ControlsHeatmapCustomLegendAppliesGlyphsAndStyles()
    {
        var coldStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 60, 110));
        var hotStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(200, 90, 30));
        var control = new Heatmap
        {
            Border = BorderStyle.None,
            ShowLegend = true,
            ShowRowLabels = false,
            ShowColumnLabels = false,
            LegendStyle = TesseraStyle.Empty.WithUnderline(),
        };
        control.SetLegend(
        [
            new HeatmapLegend("cold", 0, 4.99, 'c', coldStyle),
            new HeatmapLegend("hot", 5, 10, 'h', hotStyle),
        ]);
        control.SetMatrix(new double[,] { { 1, 9 } });

        var output = Render(control, 28, 6, CanvasTextMode.GraphemeAware);

        TestAssert.True(output.Contains("cold", StringComparison.Ordinal), "Heatmap should render custom legend labels.");
        TestAssert.True(output.Contains('h'), "Heatmap should render custom high-band glyph.");
        TestAssert.True(output.Contains("38;2;200;90;30", StringComparison.Ordinal), "Heatmap should apply custom legend/cell style.");
    }

    private static string Render(Heatmap control, int width, int height, CanvasTextMode mode = CanvasTextMode.Fast)
    {
        var canvas = new Canvas(width, height, mode);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
