using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class AreaPlotControlTests
{
    [Test]
    public void AreaPlotRenderDrawsLineAndFillGlyphs()
    {
        var control = new AreaPlot
        {
            Border = BorderStyle.None,
            MinValue = 0,
            MaxValue = 100,
            Options = new AreaPlotOptions(
                FillGlyph: '.',
                LineGlyph: '*',
                ShowBaseline: false),
        };
        control.SetSamples([0, 50, 100, 50]);

        var output = Render(control, width: 4, height: 4);
        var rows = output.Split('\n');

        Assert.That(rows.Length, Is.EqualTo(4));
        Assert.That(output.Contains('*', StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('.', StringComparison.Ordinal), Is.True);
        Assert.That(rows[^1], Is.EqualTo("*..."));
    }

    [Test]
    public void AreaPlotAppendAndSetSamplesHonorCapacityAndClear()
    {
        var control = new AreaPlot(capacity: 3);
        control.SetSamples([1, 2, 3, 4, 5]);
        var expectedAfterSet = new[] { 3d, 4d, 5d };
        Assert.That(control.Samples, Is.EqualTo(expectedAfterSet));

        control.Append(6);
        var expectedAfterAppend = new[] { 4d, 5d, 6d };
        Assert.That(control.Samples, Is.EqualTo(expectedAfterAppend));

        control.Clear();
        Assert.That(control.Samples.Count, Is.EqualTo(0));
    }

    [Test]
    public void AreaPlotRendersStatsLegendAndBaselineWhenEnabled()
    {
        var control = new AreaPlot
        {
            Border = BorderStyle.SingleLine,
            Options = new AreaPlotOptions(
                ShowBaseline: true,
                BaselineGlyph: '=',
                ShowStats: true,
                Legend: "mem"),
        };
        control.SetSamples([10, 20, 30, 40]);

        var output = Render(control, width: 28, height: 8);

        Assert.That(output.Contains("min:10.0 max:40.0", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("mem", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('='), Is.True);
    }

    [Test]
    public void AreaPlotFocusMarkerAndFocusedTitleStyleRenderWhenBorderEnabled()
    {
        var focusedTitle = TesseraStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightCyan);
        var control = new AreaPlot
        {
            Border = BorderStyle.SingleLine,
            Title = "Memory",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = focusedTitle,
        };
        control.SetSamples([1, 2, 3, 4, 5]);

        var output = Render(control, width: 28, height: 8);

        Assert.That(output.Contains("Memory !", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(focusedTitle.Render("Memory !"), StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void AreaPlotDisabledStyleIsMergedIntoFillAndLineStyles()
    {
        var control = new AreaPlot
        {
            Border = BorderStyle.None,
            MinValue = 0,
            MaxValue = 100,
            FillStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            LineStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            DisabledStyle = TesseraStyle.Empty.WithDim(),
            IsDisabled = true,
            Options = new AreaPlotOptions(ShowBaseline: false),
        };
        control.SetSamples([100]);

        var output = Render(control, width: 1, height: 2);
        var expectedLine = control.LineStyle.Merge(control.DisabledStyle).Render("▀");

        Assert.That(output.Contains(expectedLine, StringComparison.Ordinal), Is.True);
    }

    private static string Render(AreaPlot control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
