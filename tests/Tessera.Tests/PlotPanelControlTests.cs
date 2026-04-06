using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class PlotPanelControlTests
{
    [Test]
    public void PlotPanelRender_ArrangesChildrenAcrossConfiguredColumns()
    {
        var first = new LinePlot
        {
            Border = BorderStyle.SingleLine,
            Title = "P1",
            Options = new LinePlotOptions(ShowLegend: false, ShowStats: false),
        };
        first.SetSeries([new LineSeries("a", [1, 2, 3])]);

        var second = new LinePlot
        {
            Border = BorderStyle.SingleLine,
            Title = "P2",
            Options = new LinePlotOptions(ShowLegend: false, ShowStats: false),
        };
        second.SetSeries([new LineSeries("b", [3, 2, 1])]);

        var panel = new PlotPanel
        {
            Title = "Dashboard",
            Border = BorderStyle.SingleLine,
            Options = new PlotPanelOptions(Columns: 2, Spacing: 1),
        };
        panel.SetPlots([first, second]);

        var output = Render(panel, width: 72, height: 16);

        Assert.That(output.Contains("Dashboard", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("P1", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("P2", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void PlotPanelRender_EmptyTextShownWhenNoPlotsConfigured()
    {
        var panel = new PlotPanel
        {
            EmptyText = "nothing",
            Border = BorderStyle.SingleLine,
        };

        var output = Render(panel, width: 30, height: 6);

        Assert.That(output.Contains("nothing", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void PlotPanelHandle_PointerIsForwardedToContainingCellOnly()
    {
        var left = new SpyControl();
        var right = new SpyControl();
        var panel = new PlotPanel
        {
            Border = BorderStyle.None,
            Options = new PlotPanelOptions(Columns: 2, Spacing: 0),
        };
        panel.SetPlots([left, right]);

        var handled = panel.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 15, Y: 1),
            new Rect(0, 0, 20, 4));

        Assert.That(handled, Is.True);
        Assert.That(left.PointerCalls, Is.EqualTo(0));
        Assert.That(right.PointerCalls, Is.EqualTo(1));
    }

    private static string Render(PlotPanel control, int width, int height)
    {
        var canvas = new Canvas(width, height);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }

    private sealed class SpyControl : Control
    {
        public int PointerCalls { get; private set; }

        public override void Render(Canvas canvas, Rect rect)
        {
        }

        public override bool Handle(Message message, Rect bounds)
        {
            if (message is PointerInput)
            {
                PointerCalls++;
                return true;
            }

            return false;
        }
    }
}
