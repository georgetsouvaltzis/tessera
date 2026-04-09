using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class HistogramControlTests
{
    [Test]
    public void ControlsHistogramRendersBarsAxesLegendAndScale()
    {
        var control = new Histogram
        {
            Title = "Errors",
            Options = new HistogramOptions(
                ShowAxes: true,
                ShowBucketLabels: true,
                ShowScale: false,
                Legend: "req/s",
                XLabel: "bucket",
                YLabel: "count",
                BarGlyph: '#'),
        };
        control.SetBuckets(
        [
            new HistogramBucket("p50", 30),
            new HistogramBucket("p95", 70),
            new HistogramBucket("p99", 90),
        ]);
        var canvas = new Canvas(44, 12);

        control.Render(canvas, new Rect(0, 0, 44, 12));
        var output = canvas.Render();

        TestAssert.True(output.Contains(" Errors ", StringComparison.Ordinal), "Histogram should render title.");
        TestAssert.True(output.Contains('└'), "Histogram should render axis corner when axes are enabled.");
        TestAssert.True(output.Contains("req/s", StringComparison.Ordinal), "Histogram should render legend.");
        TestAssert.True(output.Contains("bucket", StringComparison.Ordinal), "Histogram should render x-axis label.");
        TestAssert.True(output.Contains("coun", StringComparison.Ordinal), "Histogram should render clipped y-axis label text.");
        TestAssert.True(output.Contains('#'), "Histogram should render configured bar glyph.");
    }

    [Test]
    public void ControlsHistogramWithScaleRendersMaxText()
    {
        var control = new Histogram
        {
            Options = new HistogramOptions(ShowScale: true, ShowBucketLabels: false),
        };
        control.SetBuckets(
        [
            new HistogramBucket("ok", 10),
            new HistogramBucket("warn", 25),
        ]);
        var canvas = new Canvas(28, 10);

        control.Render(canvas, new Rect(0, 0, 28, 10));
        var output = canvas.Render();

        TestAssert.True(output.Contains("max:", StringComparison.Ordinal), "Histogram should render max scale text when enabled.");
    }

    [Test]
    public void ControlsHistogramSetValueUpdatesExistingBucket()
    {
        var control = new Histogram();
        control.SetBuckets(
        [
            new HistogramBucket("ok", 10),
            new HistogramBucket("warn", 20),
        ]);

        control.SetValue("warn", 60);

        TestAssert.Equal(2, control.Buckets.Count, "Histogram should keep existing bucket count when updating.");
        TestAssert.Equal(60d, control.Buckets[1].Value, "Histogram should update matching bucket value.");
    }

    [Test]
    public void ControlsHistogramDefaultRenderIsMonochromeAndDeterministic()
    {
        var control = new Histogram();
        control.SetBuckets(
        [
            new HistogramBucket("a", 1),
            new HistogramBucket("b", 2),
            new HistogramBucket("c", 3),
        ]);
        var bounds = new Rect(0, 0, 30, 10);
        var firstCanvas = new Canvas(30, 10);
        var secondCanvas = new Canvas(30, 10);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        TestAssert.Equal(first, second, "Histogram should render deterministically for identical state.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default histogram output should remain monochrome.");
    }

    [Test]
    public void ControlsHistogramBarStyleEmitsAnsiSequences()
    {
        var barStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(66, 77, 88));
        var control = new Histogram
        {
            BarStyle = barStyle,
            Options = new HistogramOptions(BarGlyph: '@'),
        };
        control.SetBuckets(
        [
            new HistogramBucket("x", 2),
            new HistogramBucket("y", 4),
        ]);
        var canvas = new Canvas(28, 10, CanvasTextMode.GraphemeAware);

        control.Render(canvas, new Rect(0, 0, 28, 10));
        var output = canvas.Render();

        TestAssert.True(output.Contains("38;2;66;77;88", StringComparison.Ordinal), "Histogram bar style should render foreground ANSI sequence.");
    }
}
