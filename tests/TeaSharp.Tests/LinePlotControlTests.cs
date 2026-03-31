using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class LinePlotControlTests
{
    [Test]
    public void LinePlotRender_MultiSeriesLegendAndStatsRendered()
    {
        var cpu = new LineSeries("cpu", [12, 18, 24, 20, 16])
        {
            PointGlyph = '●',
        };
        var mem = new LineSeries("mem", [44, 42, 40, 38, 36])
        {
            PointGlyph = '◆',
        };
        var control = new LinePlot
        {
            Title = "Telemetry",
            Options = new LinePlotOptions(ShowLegend: true, ShowStats: true),
        };
        control.SetSeries([cpu, mem]);

        var output = Render(control, width: 48, height: 12);

        Assert.That(output.Contains("Telemetry", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("min:", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("cpu", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("mem", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('●') || output.Contains('◆'), Is.True);
    }

    [Test]
    public void LinePlotRender_FocusedTitleAndBorderStyleApplied()
    {
        var borderStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen);
        var focusedTitle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta);
        var control = new LinePlot
        {
            Title = "Focus",
            FocusMarker = "!",
            ShowFocusMarker = true,
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            FocusedTitleStyle = focusedTitle,
            FocusedBorderStyleText = borderStyle,
        };
        control.SetSeries([new LineSeries("s0", [1, 2, 3, 2, 1])]);

        var output = Render(control, width: 36, height: 10);

        Assert.That(output.Contains(focusedTitle.Render("Focus !"), StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(borderStyle.Render("┌"), StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void LinePlotApi_AppendRemoveAndClearSeriesBehaves()
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
    public void LinePlotZoomOffset_ShiftsVisibleStatsWindow()
    {
        var samples = Enumerable.Range(0, 20).Select(static value => (double)value).ToArray();
        var series = new LineSeries("s0", samples);
        var baseline = new LinePlot
        {
            Title = "Zoom",
            Options = new LinePlotOptions(ShowLegend: false, ShowStats: true, Zoom: 1.0, Offset: 0),
        };
        baseline.SetSeries([series]);

        var shifted = new LinePlot
        {
            Title = "Zoom",
            Options = new LinePlotOptions(ShowLegend: false, ShowStats: true, Zoom: 2.0, Offset: 8),
        };
        shifted.SetSeries([new LineSeries("s0", samples)]);

        var baselineOutput = Render(baseline, width: 34, height: 10);
        var shiftedOutput = Render(shifted, width: 34, height: 10);

        Assert.That(baselineOutput.Contains("min:0.0", StringComparison.Ordinal), Is.True);
        Assert.That(shiftedOutput.Contains("min:0.0", StringComparison.Ordinal), Is.False);
        Assert.That(shiftedOutput.Contains("max:", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void LineSeriesRetention_CapacityAndTrimToLast_KeepTrailingSamples()
    {
        var series = new LineSeries("req")
        {
            Capacity = 3,
        };
        series.SetSamples([1, 2, 3, 4, 5]);
        series.Append(6);
        series.TrimToLast(2);

        Assert.That(series.Samples, Is.EqualTo(new[] { 5d, 6d }));
    }

    [Test]
    public void LinePlotConfigureHelpers_UpdateAdvancedOptionsWithoutDirectReplacement()
    {
        var control = new LinePlot();

        control.ConfigureAxes(showAxes: true, xLabel: "time", sharedAxisLabel: "req/s", normalizedAxisLabel: "norm")
            .ConfigureGrid(showGrid: true)
            .ConfigureLegend(showLegend: false);

        Assert.That(control.Options.HasValue, Is.True);
        Assert.That(control.Options!.Value.ShowAxes, Is.True);
        Assert.That(control.Options!.Value.ShowGrid, Is.True);
        Assert.That(control.Options!.Value.ShowLegend, Is.False);
        Assert.That(control.Options!.Value.XLabel, Is.EqualTo("time"));
        Assert.That(control.Options!.Value.SharedAxisLabel, Is.EqualTo("req/s"));
        Assert.That(control.Options!.Value.NormalizedAxisLabel, Is.EqualTo("norm"));
    }

    [Test]
    public void LinePlotRender_NormalizedSeries_UsesPerSeriesScaleForMixedUnits()
    {
        var requests = new LineSeries("Req/s", [0, 10, 20]) { PointGlyph = '●' };
        var latency = new LineSeries("P95", [1000, 1001, 1002])
        {
            PointGlyph = '◆',
            ScaleMode = LineSeriesScaleMode.Normalized,
        };
        var control = new LinePlot
        {
            Border = BorderStyle.None,
            Options = new LinePlotOptions(ShowAxes: false, ShowLegend: false, ShowStats: false),
        };
        control.SetSeries([requests, latency]);

        var canvas = new Canvas(5, 5, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, 5, 5));

        Assert.That(canvas.Get(0, 4), Is.EqualTo('◆'));
        Assert.That(canvas.Get(2, 2), Is.EqualTo('◆'));
        Assert.That(canvas.Get(4, 0), Is.EqualTo('◆'));
    }

    [Test]
    public void LinePlotRender_NormalizedAxisLabel_IsRenderedWhenConfigured()
    {
        var control = new LinePlot
        {
            Border = BorderStyle.None,
        };
        control.ConfigureAxes(showAxes: true, xLabel: "t", sharedAxisLabel: "req/s", normalizedAxisLabel: "norm")
            .ConfigureGrid(showGrid: false)
            .ConfigureLegend(showLegend: false);
        control.SetSeries(
        [
            new LineSeries("Req/s", [10, 11, 12]),
            new LineSeries("P95", [100, 150, 200]) { ScaleMode = LineSeriesScaleMode.Normalized },
        ]);

        var output = Render(control, width: 24, height: 8);

        Assert.That(output.Contains("req/s", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("norm", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("t", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void LinePlotRender_CompactMode_UsesLineGlyphsForDenseSingleSeriesTelemetry()
    {
        var control = new LinePlot
        {
            Border = BorderStyle.None,
            Options = new LinePlotOptions(ShowAxes: false, ShowGrid: false, ShowLegend: false, ShowStats: false, RenderMode: LinePlotRenderMode.Compact),
        };
        control.SetSeries(
        [
            new LineSeries("cpu", [12, 26, 14, 38, 30, 46, 28, 62, 54, 70, 58, 74]),
        ]);

        var output = Render(control, width: 10, height: 4);

        Assert.That(output.Any(IsCompactLineCharacter), Is.True, "Compact mode should emit compact line glyphs for dense telemetry traces.");
        Assert.That(output.Any(IsBrailleCharacter), Is.False, "Default compact mode should avoid terminal-dependent braille output.");
    }

    [Test]
    public void LinePlotRender_CompactBrailleMode_UsesBrailleCellsForDenseSingleSeriesTelemetry()
    {
        var control = new LinePlot
        {
            Border = BorderStyle.None,
            Options = new LinePlotOptions(ShowAxes: false, ShowGrid: false, ShowLegend: false, ShowStats: false, RenderMode: LinePlotRenderMode.CompactBraille),
        };
        control.SetSeries(
        [
            new LineSeries("cpu", [12, 26, 14, 38, 30, 46, 28, 62, 54, 70, 58, 74]),
        ]);

        var output = Render(control, width: 10, height: 4);

        Assert.That(output.Any(IsBrailleCharacter), Is.True, "Braille compact mode should preserve subcell telemetry rendering.");
    }

    [Test]
    public void LinePlotRender_CompactMode_FallsBackToBlockMicroChartWhenHeightIsTiny()
    {
        var control = new LinePlot
        {
            Border = BorderStyle.None,
            Options = new LinePlotOptions(ShowAxes: false, ShowGrid: false, ShowLegend: false, ShowStats: false, RenderMode: LinePlotRenderMode.Compact),
        };
        control.SetSeries(
        [
            new LineSeries("cpu", [10, 30, 20, 50, 40, 70, 60, 90]),
        ]);

        var output = Render(control, width: 8, height: 1);

        Assert.That(output.Any(IsBlockSparkCharacter), Is.True, "Compact mode should fall back to block spark glyphs in one-row plots.");
    }

    private static bool IsBrailleCharacter(char value) => value is >= '\u2801' and <= '\u28FF';

    private static bool IsCompactLineCharacter(char value) => value is '─' or '│' or '╭' or '╮' or '╯' or '╰' or '┬' or '┴' or '├' or '┤' or '┼';

    private static bool IsBlockSparkCharacter(char value) => value is '▁' or '▂' or '▃' or '▄' or '▅' or '▆' or '▇' or '█';

    private static string Render(LinePlot control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
