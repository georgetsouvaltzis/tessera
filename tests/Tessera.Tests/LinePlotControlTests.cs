using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class LinePlotControlTests
{
    [Test]
    public void LinePlotRenderMultiSeriesLegendAndStatsRendered()
    {
        var cpu = new LineSeries("cpu", [12, 18, 24, 20, 16]) { PointGlyph = '●' };
        var mem = new LineSeries("mem", [44, 42, 40, 38, 36]) { PointGlyph = '◆' };
        var control = new LinePlot
        {
            Title = "Telemetry",
            Options = new LinePlotOptions(ShowLegend: true, ShowStats: true)
        };
        control.SetSeries([cpu, mem]);

        var output = Render(control, 48, 12);

        Assert.That(output.Contains("Telemetry", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("min:", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("cpu", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("mem", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('●') || output.Contains('◆'), Is.True);
    }

    [Test]
    public void LinePlotRenderFocusedTitleAndBorderStyleApplied()
    {
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightGreen);
        var focusedTitle = TesseraStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta);
        var control = new LinePlot
        {
            Title = "Focus",
            FocusMarker = "!",
            ShowFocusMarker = true,
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            FocusedTitleStyle = focusedTitle,
            FocusedBorderStyleText = borderStyle
        };
        control.SetSeries([new LineSeries("s0", [1, 2, 3, 2, 1])]);

        var output = Render(control, 36, 10);

        Assert.That(output.Contains(focusedTitle.Render("Focus !"), StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(borderStyle.Render("┌"), StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void LinePlotApiAppendRemoveAndClearSeriesBehaves()
    {
        var control = new LinePlot();
        control.AddSeries(new LineSeries("a", [1, 2]));
        control.AddSeries(new LineSeries("b", [10]));

        var appended = control.AppendSample("a", 3);
        var removed = control.RemoveSeries("b");
        control.Clear();

        Assert.That(appended, Is.True);
        Assert.That(removed, Is.True);
        Assert.That(control.Series.Count, Is.EqualTo(0));
    }

    [Test]
    public void LinePlotZoomOffsetShiftsVisibleStatsWindow()
    {
        var samples = Enumerable.Range(0, 20).Select(static value => (double)value).ToArray();
        var series = new LineSeries("s0", samples);
        var baseline = new LinePlot
        {
            Title = "Zoom",
            Options = new LinePlotOptions(ShowLegend: false, ShowStats: true, Zoom: 1.0, Offset: 0)
        };
        baseline.SetSeries([series]);

        var shifted = new LinePlot
        {
            Title = "Zoom",
            Options = new LinePlotOptions(ShowLegend: false, ShowStats: true, Zoom: 2.0, Offset: 8)
        };
        shifted.SetSeries([new LineSeries("s0", samples)]);

        var baselineOutput = Render(baseline, 34, 10);
        var shiftedOutput = Render(shifted, 34, 10);

        Assert.That(baselineOutput.Contains("min:0.0", StringComparison.Ordinal), Is.True);
        Assert.That(shiftedOutput.Contains("min:0.0", StringComparison.Ordinal), Is.False);
        Assert.That(shiftedOutput.Contains("max:", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void LineSeriesRetentionCapacityAndTrimToLastKeepTrailingSamples()
    {
        var series = new LineSeries("req") { Capacity = 3 };
        series.SetSamples([1, 2, 3, 4, 5]);
        series.Append(6);
        series.TrimToLast(2);

        var expectedSamples = new[] { 5d, 6d };
        Assert.That(series.Samples, Is.EqualTo(expectedSamples));
    }

    [Test]
    public void LinePlotConfigureHelpersUpdateAdvancedOptionsWithoutDirectReplacement()
    {
        var control = new LinePlot();

        control.ConfigureAxes(true, "time", "req/s", "norm")
            .ConfigureGrid()
            .ConfigureLegend(false);

        Assert.That(control.Options.HasValue, Is.True);
        Assert.That(TestAssert.NotNull(control.Options).ShowAxes, Is.True);
        Assert.That(TestAssert.NotNull(control.Options).ShowGrid, Is.True);
        Assert.That(TestAssert.NotNull(control.Options).ShowLegend, Is.False);
        Assert.That(TestAssert.NotNull(control.Options).XLabel, Is.EqualTo("time"));
        Assert.That(TestAssert.NotNull(control.Options).SharedAxisLabel, Is.EqualTo("req/s"));
        Assert.That(TestAssert.NotNull(control.Options).NormalizedAxisLabel, Is.EqualTo("norm"));
    }

    [Test]
    public void LinePlotRenderNormalizedSeriesUsesPerSeriesScaleForMixedUnits()
    {
        var requests = new LineSeries("Req/s", [0, 10, 20]) { PointGlyph = '●' };
        var latency = new LineSeries("P95", [1000, 1001, 1002])
        {
            PointGlyph = '◆',
            ScaleMode = LineSeriesScaleMode.Normalized
        };
        var control = new LinePlot
        {
            Border = BorderStyle.None,
            Options = new LinePlotOptions(ShowLegend: false, ShowStats: false)
        };
        control.SetSeries([requests, latency]);

        var canvas = new Canvas(5, 5, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, 5, 5));

        Assert.That(canvas.Get(0, 4), Is.EqualTo('◆'));
        Assert.That(canvas.Get(2, 2), Is.EqualTo('◆'));
        Assert.That(canvas.Get(4, 0), Is.EqualTo('◆'));
    }

    [Test]
    public void LinePlotRenderNormalizedAxisLabelIsRenderedWhenConfigured()
    {
        var control = new LinePlot { Border = BorderStyle.None };
        control.ConfigureAxes(true, "t", "req/s", "norm")
            .ConfigureGrid(false)
            .ConfigureLegend(false);
        control.SetSeries(
        [
            new LineSeries("Req/s", [10, 11, 12]),
            new LineSeries("P95", [100, 150, 200]) { ScaleMode = LineSeriesScaleMode.Normalized }
        ]);

        var output = Render(control, 24, 8);

        Assert.That(output.Contains("req/s", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("norm", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('t'), Is.True);
    }

    [Test]
    public void LinePlotRenderCompactModeUsesLineGlyphsForDenseSingleSeriesTelemetry()
    {
        var output = RenderCompact([12, 26, 14, 38, 30, 46, 28, 62, 54, 70, 58, 74], 10, 4);

        Assert.That(output.Any(IsCompactLineCharacter), Is.True,
            "Compact mode should emit compact line glyphs for dense telemetry traces.");
        Assert.That(output.Any(IsBrailleCharacter), Is.False,
            "Default compact mode should avoid terminal-dependent braille output.");
        AssertNoEmptyColumns(output);
    }

    [Test]
    public void LinePlotRenderCompactBrailleModeUsesBrailleCellsForDenseSingleSeriesTelemetry()
    {
        var control = new LinePlot
        {
            Border = BorderStyle.None,
            Options = new LinePlotOptions(false, false, false, false, RenderMode: LinePlotRenderMode.CompactBraille)
        };
        control.SetSeries(
        [
            new LineSeries("cpu", [12, 26, 14, 38, 30, 46, 28, 62, 54, 70, 58, 74])
        ]);

        var output = Render(control, 10, 4);

        Assert.That(output.Any(IsBrailleCharacter), Is.True,
            "Braille compact mode should preserve subcell telemetry rendering.");
    }

    [Test]
    public void LinePlotRenderCompactModeFallsBackToBlockMicroChartWhenHeightIsTiny()
    {
        var output = RenderCompact([10, 30, 20, 50, 40, 70, 60, 90], 8, 1);

        Assert.That(output.Any(IsBlockSparkCharacter), Is.True,
            "Compact mode should fall back to block spark glyphs in one-row plots.");
    }

    [Test]
    public void LinePlotRenderCompactModeMonotoneRiseStaysContinuous()
    {
        var output = RenderCompact([10, 20, 30, 40, 50, 60, 70, 80], 12, 4);

        Assert.That(output.Any(static value => value is '╱' or '─'), Is.True);
        AssertNoEmptyColumns(output);
    }

    [Test]
    public void LinePlotRenderCompactModeMonotoneFallStaysContinuous()
    {
        var output = RenderCompact([80, 70, 60, 50, 40, 30, 20, 10], 12, 4);

        Assert.That(output.Any(static value => value is '╲' or '─'), Is.True);
        AssertNoEmptyColumns(output);
    }

    [Test]
    public void LinePlotRenderCompactModeShallowSlopeStaysContinuous()
    {
        var output = RenderCompact([40, 42, 44, 46, 47, 48, 50, 52], 12, 4);

        AssertNoEmptyColumns(output);
    }

    [Test]
    public void LinePlotRenderCompactModeValleyStaysContinuous()
    {
        var output = RenderCompact([70, 55, 40, 24, 36, 52, 68], 12, 4);

        AssertNoEmptyColumns(output);
    }

    [Test]
    public void LinePlotRenderCompactModePeakStaysContinuous()
    {
        var output = RenderCompact([24, 40, 58, 72, 56, 38, 20], 12, 4);

        AssertNoEmptyColumns(output);
    }

    private static bool IsBrailleCharacter(char value)
    {
        return value is >= '\u2801' and <= '\u28FF';
    }

    private static bool IsCompactLineCharacter(char value)
    {
        return value is '─' or '│' or '╱' or '╲' or '╭' or '╮' or '╯' or '╰' or '•';
    }

    private static bool IsBlockSparkCharacter(char value)
    {
        return value is '▁' or '▂' or '▃' or '▄' or '▅' or '▆' or '▇' or '█';
    }

    private static string RenderCompact(IEnumerable<double> samples, int width, int height)
    {
        var control = new LinePlot
        {
            Border = BorderStyle.None,
            Options = new LinePlotOptions(false, false, false, false, RenderMode: LinePlotRenderMode.Compact)
        };
        control.SetSeries([new LineSeries("cpu", samples)]);
        return Render(control, width, height);
    }

    private static void AssertNoEmptyColumns(string output)
    {
        var lines = output
            .Split('\n')
            .Select(static line => line.TrimEnd('\r'))
            .ToArray();
        var width = lines.Max(static line => line.Length);
        var first = -1;
        var last = -1;

        for (var column = 0; column < width; column++)
        {
            if (!ColumnContainsTrace(lines, column))
            {
                continue;
            }

            if (first < 0)
            {
                first = column;
            }

            last = column;
        }

        Assert.That(first, Is.GreaterThanOrEqualTo(0), "Expected compact render output to contain trace glyphs.");
        for (var column = first; column <= last; column++)
        {
            Assert.That(ColumnContainsTrace(lines, column), Is.True,
                $"Expected compact trace continuity at column {column}.");
        }
    }

    private static bool ColumnContainsTrace(string[] lines, int column)
    {
        for (var row = 0; row < lines.Length; row++)
        {
            if (column >= lines[row].Length)
            {
                continue;
            }

            var value = lines[row][column];
            if (IsCompactLineCharacter(value) || IsBlockSparkCharacter(value))
            {
                return true;
            }
        }

        return false;
    }

    private static string Render(LinePlot control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
