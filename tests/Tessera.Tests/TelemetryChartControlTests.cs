using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TelemetryChartControlTests
{
    [Test]
    public void TelemetryChartRender_AutoMode_UsesBrailleCoverageForTinyCards()
    {
        var control = new TelemetryChart
        {
            Border = BorderStyle.None,
            Options = new TelemetryChartOptions(RenderMode: TelemetryChartRenderMode.Auto),
        };
        control.SetSamples([12, 26, 14, 38, 30, 46, 28, 62, 54, 70, 58, 74]);

        var output = Render(control, width: 14, height: 4);

        Assert.That(output.Any(IsBrailleCharacter), Is.True);
    }

    [Test]
    public void TelemetryChartRender_AreaMode_UsesFilledBlockCoverage()
    {
        var control = new TelemetryChart
        {
            Border = BorderStyle.None,
            Options = new TelemetryChartOptions(RenderMode: TelemetryChartRenderMode.Area),
        };
        control.SetSamples([10, 20, 35, 48, 44, 56, 72, 68, 76]);

        var output = Render(control, width: 12, height: 4);

        Assert.That(output.Any(IsBlockCharacter), Is.True);
        AssertNoEmptyColumns(output, IsBlockCharacter);
    }

    [Test]
    public void TelemetryChartRender_BlockMode_UsesThinnerRibbonThanAreaMode()
    {
        var area = new TelemetryChart
        {
            Border = BorderStyle.None,
            Options = new TelemetryChartOptions(RenderMode: TelemetryChartRenderMode.Area),
        };
        var block = new TelemetryChart
        {
            Border = BorderStyle.None,
            Options = new TelemetryChartOptions(RenderMode: TelemetryChartRenderMode.Block),
        };
        var samples = new[] { 18d, 32d, 54d, 60d, 46d, 40d, 68d, 74d, 58d, 36d };
        area.SetSamples(samples);
        block.SetSamples(samples);

        var areaOutput = Render(area, width: 12, height: 4);
        var blockOutput = Render(block, width: 12, height: 4);

        Assert.That(CountTraceGlyphs(blockOutput), Is.LessThan(CountTraceGlyphs(areaOutput)));
    }

    [Test]
    public void TelemetryChartRender_OneRowAutoMode_FallsBackToBlocks()
    {
        var control = new TelemetryChart
        {
            Border = BorderStyle.None,
            Options = new TelemetryChartOptions(RenderMode: TelemetryChartRenderMode.Auto),
        };
        control.SetSamples([8, 16, 14, 28, 22, 44, 40, 60]);

        var output = Render(control, width: 8, height: 1);

        Assert.That(output.Any(IsBlockCharacter), Is.True);
    }

    [Test]
    public void TelemetryChartFocusMarkerAndFocusedBorderStyleRenderWhenBorderEnabled()
    {
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightGreen);
        var focusedTitle = TesseraStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta);
        var control = new TelemetryChart
        {
            Border = BorderStyle.SingleLine,
            Title = "CPU",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = focusedTitle,
            FocusedBorderStyleText = borderStyle,
        };
        control.SetSamples([1, 2, 3, 4, 5]);

        var output = Render(control, width: 24, height: 6);

        Assert.That(output.Contains(focusedTitle.Render("CPU !"), StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(borderStyle.Render("┌"), StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void TelemetryChartAppendAndSetSamplesHonorCapacityAndClear()
    {
        var control = new TelemetryChart(capacity: 4);
        control.SetSamples([1, 2, 3, 4, 5, 6]);
        Assert.That(control.Samples, Is.EqualTo(new[] { 3d, 4d, 5d, 6d }));

        control.Append(7);
        Assert.That(control.Samples, Is.EqualTo(new[] { 4d, 5d, 6d, 7d }));
        control.TrimToLast(2);
        Assert.That(control.Samples, Is.EqualTo(new[] { 6d, 7d }));

        control.Clear();
        Assert.That(control.Samples.Count, Is.EqualTo(0));
    }

    [Test]
    public void TelemetryChartStatsRow_RendersNowMinMaxAndLegend()
    {
        var control = new TelemetryChart
        {
            Border = BorderStyle.None,
            Options = new TelemetryChartOptions(ShowStats: true, Legend: "cpu"),
        };
        control.SetSamples([10, 12, 16, 20]);

        var output = Render(control, width: 24, height: 4);

        Assert.That(output.Contains("now:20.0", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("min:10.0", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("max:20.0", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("cpu", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void TelemetryChartStatsRow_NarrowCard_FallsBackToTwoRows()
    {
        var control = new TelemetryChart
        {
            Border = BorderStyle.None,
            Options = new TelemetryChartOptions(ShowStats: true, Legend: "cpu"),
        };
        control.SetSamples([10, 12, 16, 20]);

        var output = Render(control, width: 24, height: 4);
        var lines = output.Split('\n', StringSplitOptions.None)
            .Select(static line => line.TrimEnd('\r'))
            .ToArray();

        Assert.That(lines.Length, Is.GreaterThanOrEqualTo(2));
        Assert.That(lines[0], Does.Contain("now:20.0"));
        Assert.That(lines[1], Does.Contain("min:10.0"));
        Assert.That(lines[1], Does.Contain("max:20.0"));
        Assert.That(lines[1], Does.Contain("cpu"));
    }

    private static string Render(TelemetryChart control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }

    private static int CountTraceGlyphs(string output) => output.Count(value => IsBrailleCharacter(value) || IsBlockCharacter(value));

    private static bool IsBrailleCharacter(char value) => value is >= '\u2801' and <= '\u28FF';

    private static bool IsBlockCharacter(char value) => value is '▁' or '▂' or '▃' or '▄' or '▅' or '▆' or '▇' or '█';

    private static void AssertNoEmptyColumns(string output, Func<char, bool> predicate)
    {
        var lines = output
            .Split('\n', StringSplitOptions.None)
            .Select(static line => line.TrimEnd('\r'))
            .ToArray();
        var width = lines.Max(static line => line.Length);
        var first = -1;
        var last = -1;

        for (var column = 0; column < width; column++)
        {
            if (!ColumnContains(lines, column, predicate))
            {
                continue;
            }

            if (first < 0)
            {
                first = column;
            }

            last = column;
        }

        Assert.That(first, Is.GreaterThanOrEqualTo(0));
        for (var column = first; column <= last; column++)
        {
            Assert.That(ColumnContains(lines, column, predicate), Is.True, $"Expected telemetry continuity at column {column}.");
        }
    }

    private static bool ColumnContains(string[] lines, int column, Func<char, bool> predicate)
    {
        for (var row = 0; row < lines.Length; row++)
        {
            if (column >= lines[row].Length)
            {
                continue;
            }

            if (predicate(lines[row][column]))
            {
                return true;
            }
        }

        return false;
    }
}
