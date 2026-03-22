using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class BoxPlotControlTests
{
    [Test]
    public void Controls_BoxPlot_RendersSeriesAndDistributionGlyphs()
    {
        var control = new BoxPlot
        {
            Border = BorderStyle.None,
            Title = string.Empty,
        };
        control.SetSeries(
        [
            new BoxPlotSeries("api", 10, 20, 30, 40, 50),
            new BoxPlotSeries("db", 5, 15, 18, 25, 35),
        ]);

        var output = Render(control, width: 72, height: 4);

        Assert.That(output.Contains("api", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("db", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('─') || output.Contains('═') || output.Contains('│'), Is.True);
    }

    [Test]
    public void Controls_BoxPlot_KeyboardAndPointerSelection_RaisesSelectionChanged()
    {
        var control = new BoxPlot
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            IsFocused = true,
        };
        control.SetSeries(
        [
            new BoxPlotSeries("a", 1, 2, 3, 4, 5),
            new BoxPlotSeries("b", 1, 2, 3, 4, 5),
            new BoxPlotSeries("c", 1, 2, 3, 4, 5),
        ]);

        var changes = 0;
        ListSelectionChangedEventArgs<BoxPlotSeries>? latest = null;
        control.SelectionChanged += (_, args) =>
        {
            changes++;
            latest = args;
        };

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var clickHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 1, Y: 2),
            new Rect(0, 0, 64, 4));

        Assert.That(downHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(control.SelectedSeriesIndex, Is.EqualTo(2));
        Assert.That(latest?.SelectedItem?.Name, Is.EqualTo("c"));
        Assert.That(changes, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void Controls_BoxPlot_StyleHooksAndFocusedBorder_EmitAnsi()
    {
        var control = new BoxPlot
        {
            IsFocused = true,
            FocusMarker = "!",
            Border = BorderStyle.SingleLine,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30)),
            FocusedBorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(40, 50, 60)),
            FocusedTitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(70, 80, 90)),
            WhiskerStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(100, 110, 120)),
            QuartileStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(130, 140, 150)),
            MedianStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(160, 170, 180)),
            SelectedSeriesStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(190, 200, 210)),
        };
        control.SetSeries(
        [
            new BoxPlotSeries("api", 10, 20, 25, 30, 40),
            new BoxPlotSeries("db", 5, 8, 12, 16, 20),
        ]);
        _ = control.SetSelectedSeries(1);

        var output = Render(control, width: 72, height: 8);

        Assert.That(output.Contains("Box Plot !", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;40;50;60", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;70;80;90", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;100;110;120", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;130;140;150", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;160;170;180", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("48;2;190;200;210", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_BoxPlot_DefaultRender_IsDeterministicAndMonochrome()
    {
        var control = new BoxPlot
        {
            Border = BorderStyle.None,
            Title = string.Empty,
        };
        control.SetSeries(
        [
            new BoxPlotSeries("api", 1, 2, 3, 4, 5),
            new BoxPlotSeries("db", 2, 3, 4, 5, 6),
        ]);

        var first = Render(control, width: 64, height: 4);
        var second = Render(control, width: 64, height: 4);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(BoxPlot control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
