using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SparklineControlTests
{
    [Test]
    public void SparklineRenderMapsAscendingSamplesToExpectedBlocks()
    {
        var control = new Sparkline { MinValue = 0, MaxValue = 100, Border = BorderStyle.None };
        control.SetSamples([0, 14, 28, 42, 57, 71, 85, 100]);

        var output = Render(control, 8, 1);

        Assert.That(output, Is.EqualTo("▁▂▃▄▅▆▇█"));
    }

    [Test]
    public void SparklineAppendAndSetSamplesHonorCapacityAndClear()
    {
        var control = new Sparkline(4);
        control.SetSamples([1, 2, 3, 4, 5, 6]);
        var expectedAfterSet = new[] { 3d, 4d, 5d, 6d };
        Assert.That(control.Samples, Is.EqualTo(expectedAfterSet));

        control.Append(7);
        var expectedAfterAppend = new[] { 4d, 5d, 6d, 7d };
        Assert.That(control.Samples, Is.EqualTo(expectedAfterAppend));
        control.TrimToLast(2);
        var expectedAfterTrim = new[] { 6d, 7d };
        Assert.That(control.Samples, Is.EqualTo(expectedAfterTrim));

        control.Clear();
        Assert.That(control.Samples.Count, Is.EqualTo(0));
    }

    [Test]
    public void SparklineFocusMarkerAndFocusedTitleStyleRenderWhenBorderEnabled()
    {
        var focusedTitle = TesseraStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta);
        var control = new Sparkline
        {
            Border = BorderStyle.SingleLine,
            Title = "CPU",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = focusedTitle
        };
        control.SetSamples([1, 2, 3, 4]);

        var output = Render(control, 24, 4);

        Assert.That(output.Contains("CPU !", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(focusedTitle.Render("CPU !"), StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void SparklineDisabledStyleIsMergedIntoDataStyle()
    {
        var control = new Sparkline
        {
            Border = BorderStyle.None,
            MinValue = 0,
            MaxValue = 100,
            DataStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            DisabledStyle = TesseraStyle.Empty.WithDim(),
            IsDisabled = true
        };
        control.SetSamples([0, 100]);

        var output = Render(control, 2, 1);
        var expected = control.DataStyle.Merge(control.DisabledStyle).Render("▁█");

        Assert.That(output.Contains(expected, StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void SparklineUsesCustomStepsFromOptions()
    {
        var control = new Sparkline
        {
            Border = BorderStyle.None,
            MinValue = 0,
            MaxValue = 100,
            Options = new SparklineOptions(".oO")
        };
        control.SetSamples([0, 50, 100]);

        var output = Render(control, 3, 1);

        Assert.That(output, Is.EqualTo(".oO"));
    }

    private static string Render(Sparkline control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
