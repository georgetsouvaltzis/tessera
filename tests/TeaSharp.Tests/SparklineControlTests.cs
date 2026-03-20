using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SparklineControlTests
{
    [Test]
    public void SparklineRenderMapsAscendingSamplesToExpectedBlocks()
    {
        var control = new Sparkline
        {
            MinValue = 0,
            MaxValue = 100,
            Border = BorderStyle.None,
        };
        control.SetSamples([0, 14, 28, 42, 57, 71, 85, 100]);

        var output = Render(control, width: 8, height: 1);

        Assert.That(output, Is.EqualTo("▁▂▃▄▅▆▇█"));
    }

    [Test]
    public void SparklineAppendAndSetSamplesHonorCapacityAndClear()
    {
        var control = new Sparkline(capacity: 4);
        control.SetSamples([1, 2, 3, 4, 5, 6]);
        Assert.That(control.Samples, Is.EqualTo(new[] { 3d, 4d, 5d, 6d }));

        control.Append(7);
        Assert.That(control.Samples, Is.EqualTo(new[] { 4d, 5d, 6d, 7d }));

        control.Clear();
        Assert.That(control.Samples.Count, Is.EqualTo(0));
    }

    [Test]
    public void SparklineFocusMarkerAndFocusedTitleStyleRenderWhenBorderEnabled()
    {
        var focusedTitle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta);
        var control = new Sparkline
        {
            Border = BorderStyle.SingleLine,
            Title = "CPU",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = focusedTitle,
        };
        control.SetSamples([1, 2, 3, 4]);

        var output = Render(control, width: 24, height: 4);

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
            DataStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            DisabledStyle = TeaStyle.Empty.WithDim(),
            IsDisabled = true,
        };
        control.SetSamples([0, 100]);

        var output = Render(control, width: 2, height: 1);
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
            Options = new SparklineOptions(Steps: ".oO"),
        };
        control.SetSamples([0, 50, 100]);

        var output = Render(control, width: 3, height: 1);

        Assert.That(output, Is.EqualTo(".oO"));
    }

    private static string Render(Sparkline control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
