using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ScatterPlotControlTests
{
    [Test]
    public void Controls_ScatterPlot_RendersAxesLegendLabelsAndPoints()
    {
        var control = new ScatterPlot
        {
            Title = "Latency",
            Options = new ScatterPlotOptions(
                ShowAxes: true,
                ShowLabels: true,
                Legend: "p95",
                XLabel: "time",
                YLabel: "ms",
                PointGlyph: 'x'),
        };
        control.SetPoints(
        [
            new ScatterPlotPoint(5, 20, "a"),
            new ScatterPlotPoint(10, 40, "b"),
            new ScatterPlotPoint(15, 60, "c"),
        ]);
        var canvas = new Canvas(40, 12);

        control.Render(canvas, new Rect(0, 0, 40, 12));
        var output = canvas.Render();

        TestAssert.True(output.Contains(" Latency ", StringComparison.Ordinal), "Scatter plot should render title.");
        TestAssert.True(output.Contains('└'), "Scatter plot should render axis corner when axes are enabled.");
        TestAssert.True(output.Contains("p95", StringComparison.Ordinal), "Scatter plot should render legend text.");
        TestAssert.True(output.Contains("time", StringComparison.Ordinal), "Scatter plot should render x-axis label.");
        TestAssert.True(output.Contains("ms", StringComparison.Ordinal), "Scatter plot should render y-axis label.");
        TestAssert.True(output.Contains('x'), "Scatter plot should render configured point glyph.");
    }

    [Test]
    public void Controls_ScatterPlot_DefaultRender_IsMonochromeAndDeterministic()
    {
        var control = new ScatterPlot();
        control.SetPoints(
        [
            new ScatterPlotPoint(1, 1),
            new ScatterPlotPoint(2, 2),
            new ScatterPlotPoint(3, 1),
        ]);
        var bounds = new Rect(0, 0, 30, 10);
        var firstCanvas = new Canvas(30, 10);
        var secondCanvas = new Canvas(30, 10);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        TestAssert.Equal(first, second, "Scatter plot should render deterministically for identical state.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default scatter plot output should remain monochrome.");
    }

    [Test]
    public void Controls_ScatterPlot_StyledPointAndLabel_EmitAnsiSequences()
    {
        var pointStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(120, 40, 30));
        var labelStyle = TeaStyle.Empty.WithBold();
        var control = new ScatterPlot
        {
            PointStyle = pointStyle,
            LabelStyle = labelStyle,
            Options = new ScatterPlotOptions(ShowAxes: false, ShowLabels: true, PointGlyph: 'o'),
        };
        control.SetPoints([new ScatterPlotPoint(1, 1, "p1")]);
        var canvas = new Canvas(24, 8, CanvasTextMode.GraphemeAware);

        control.Render(canvas, new Rect(0, 0, 24, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("38;2;120;40;30", StringComparison.Ordinal), "Point style should render foreground ANSI sequence.");
        TestAssert.True(output.Contains("[1m", StringComparison.Ordinal), "Label style should render bold ANSI sequence.");
    }
}
