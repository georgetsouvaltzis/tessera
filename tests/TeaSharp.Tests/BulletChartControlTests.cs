using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class BulletChartControlTests
{
    [Test]
    public void BulletChart_Api_SetRangesNormalizesAndSettersUpdateValues()
    {
        var control = new BulletChart();
        control.SetRanges(
        [
            new BulletRange(80, 40, BulletRangeKind.Warning, "warn"),
            new BulletRange(20, 30, BulletRangeKind.Normal, "ok"),
        ]);

        control.SetValue(64.5);
        control.SetTarget(72);

        Assert.That(control.Ranges.Count, Is.EqualTo(2));
        Assert.That(control.Ranges[0].Start, Is.EqualTo(20d));
        Assert.That(control.Ranges[0].End, Is.EqualTo(30d));
        Assert.That(control.Ranges[1].Start, Is.EqualTo(40d));
        Assert.That(control.Ranges[1].End, Is.EqualTo(80d));
        Assert.That(control.Value, Is.EqualTo(64.5d));
        Assert.That(control.Target, Is.EqualTo(72d));
    }

    [Test]
    public void BulletChart_DefaultRender_IsMonochromeAndDeterministic()
    {
        var control = new BulletChart
        {
            Border = BorderStyle.None,
        };
        control.SetRanges(
        [
            new BulletRange(0, 60, BulletRangeKind.Normal),
            new BulletRange(60, 80, BulletRangeKind.Warning),
            new BulletRange(80, 100, BulletRangeKind.Critical),
        ]);
        control.SetValue(65);
        control.SetTarget(82);

        var firstCanvas = new Canvas(44, 4);
        var secondCanvas = new Canvas(44, 4);
        var bounds = new Rect(0, 0, 44, 4);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
        Assert.That(first.Contains('█'), Is.True);
        Assert.That(first.Contains('│'), Is.True);
    }

    [Test]
    public void BulletChart_StyleHooks_EmitAnsiForSegmentsValueAndTarget()
    {
        var control = new BulletChart
        {
            Border = BorderStyle.None,
            RangeStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30)),
            WarningRangeStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(40, 50, 60)),
            CriticalRangeStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(70, 80, 90)),
            ValueBarStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(100, 110, 120)),
            TargetMarkerStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(130, 140, 150)),
        };
        control.SetRanges(
        [
            new BulletRange(0, 50, BulletRangeKind.Normal),
            new BulletRange(50, 80, BulletRangeKind.Warning),
            new BulletRange(80, 100, BulletRangeKind.Critical),
        ]);
        control.SetValue(84);
        control.SetTarget(91);
        var canvas = new Canvas(50, 4, CanvasTextMode.GraphemeAware);

        control.Render(canvas, new Rect(0, 0, 50, 4));
        var output = canvas.Render();

        Assert.That(output.Contains("38;2;10;20;30", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;40;50;60", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;70;80;90", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;100;110;120", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;130;140;150", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void BulletChart_FocusedTitleAndBorderHooks_RenderStyledOutput()
    {
        var focusedTitle = TeaStyle.Empty.WithForeground(AnsiColor.BrightMagenta).WithUnderline();
        var focusedBorder = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen);
        var control = new BulletChart
        {
            Title = "SLO",
            FocusMarker = "!",
            ShowFocusMarker = true,
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            FocusedTitleStyle = focusedTitle,
            FocusedBorderStyleText = focusedBorder,
        };
        control.SetRanges([new BulletRange(0, 100)]);
        control.SetValue(42);
        control.SetTarget(70);

        var canvas = new Canvas(36, 6, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, 36, 6));
        var output = canvas.Render();

        Assert.That(output.Contains(focusedTitle.Render("SLO !"), StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(focusedBorder.Render("┌"), StringComparison.Ordinal), Is.True);
    }
}
