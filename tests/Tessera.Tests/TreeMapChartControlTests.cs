using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TreeMapChartControlTests
{
    [Test]
    public void ControlsTreeMapChartRendersTitleCellsAndLabels()
    {
        var control = new TreeMapChart
        {
            Title = "Capacity",
            ShowLabels = true,
            ShowLegend = true,
        };
        control.SetNodes(
        [
            new TreeMapNode("services",
            [
                new TreeMapNode("api", 50),
                new TreeMapNode("worker", 30),
            ]),
            new TreeMapNode("db", 20),
        ]);

        var output = Render(control, 56, 14);

        TestAssert.True(output.Contains(" Capacity ", StringComparison.Ordinal), "TreeMap should render title.");
        TestAssert.True(output.Contains("api", StringComparison.Ordinal), "TreeMap should render leaf labels.");
        TestAssert.True(output.Contains("db", StringComparison.Ordinal), "TreeMap should render all leaf nodes.");
        TestAssert.True(output.Contains("low", StringComparison.Ordinal), "TreeMap should render implicit legend.");
        TestAssert.True(output.Contains('█') || output.Contains('▓') || output.Contains('▒') || output.Contains('░'), "TreeMap should render weighted fill glyphs.");
    }

    [Test]
    public void ControlsTreeMapChartKeyboardAndPointerSelectionRaisesSelectionChanged()
    {
        var control = new TreeMapChart
        {
            Border = BorderStyle.None,
            ShowLabels = false,
            ShowLegend = false,
            IsFocused = true,
        };
        control.SetNodes(
        [
            new TreeMapNode("a", 10),
            new TreeMapNode("b", 10),
            new TreeMapNode("c", 10),
        ]);
        _ = Render(control, 30, 8);

        var events = 0;
        control.SelectionChanged += (_, _) => events++;

        var keyHandled = control.Handle(new KeyPressed(Key.Right));
        var pointerHandled = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 24, 2), new Rect(0, 0, 30, 8));

        TestAssert.True(keyHandled, "Keyboard navigation should move selection.");
        TestAssert.True(pointerHandled, "Pointer press should select hit node.");
        TestAssert.Equal(2, control.SelectedIndex, "Pointer should select right-side leaf.");
        TestAssert.True(events >= 2, "Selection changed event should fire for both input paths.");
    }

    [Test]
    public void ControlsTreeMapChartStyledNodeAndFocusedBorderEmitAnsi()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(90, 80, 70));
        var peakStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 22, 33));
        var selectedStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(44, 55, 66));
        var control = new TreeMapChart
        {
            Border = BorderStyle.SingleLine,
            ShowLabels = false,
            ShowLegend = false,
            IsFocused = true,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 10, 10)),
            FocusedBorderStyleText = focusedBorderStyle,
            PeakNodeStyle = peakStyle,
            SelectedNodeStyle = selectedStyle,
        };
        control.SetNodes([new TreeMapNode("hot", 100), new TreeMapNode("cold", 10)]);
        _ = control.SetSelectedIndex(0);

        var output = Render(control, 26, 8, CanvasTextMode.GraphemeAware);

        TestAssert.True(output.Contains("38;2;11;22;33", StringComparison.Ordinal), "Node style should emit ANSI foreground sequence.");
        TestAssert.True(output.Contains("48;2;44;55;66", StringComparison.Ordinal), "Selected style should emit ANSI background sequence.");
        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "Focused border style should apply to frame glyphs.");
    }

    [Test]
    public void ControlsTreeMapChartDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new TreeMapChart
        {
            ShowLabels = false,
            ShowLegend = false,
        };
        control.SetNodes(
        [
            new TreeMapNode("one", 1),
            new TreeMapNode("two", 2),
            new TreeMapNode("three", 3),
        ]);

        var first = Render(control, 24, 8);
        var second = Render(control, 24, 8);

        TestAssert.Equal(first, second, "TreeMap should render deterministically for identical state.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default TreeMap output should remain monochrome.");
    }

    private static string Render(TreeMapChart control, int width, int height, CanvasTextMode mode = CanvasTextMode.Fast)
    {
        var canvas = new Canvas(width, height, mode);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
