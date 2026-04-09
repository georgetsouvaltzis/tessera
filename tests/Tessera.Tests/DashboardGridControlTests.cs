using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class DashboardGridControlTests
{
    [Test]
    public void ControlsDashboardGridKeyboardSelectionRaisesSelectionChanged()
    {
        var control = new DashboardGrid
        {
            IsFocused = true,
        };
        control.SetTiles(
        [
            new DashboardTile("cpu", "CPU", 0, 0),
            new DashboardTile("mem", "Memory", 1, 0),
            new DashboardTile("lat", "Latency", 0, 1),
        ]);

        ListSelectionChangedEventArgs<DashboardTile>? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var handled = control.Handle(new KeyPressed(Key.Right));

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedTileId, Is.EqualTo("mem"));
        Assert.That(args, Is.Not.Null);
        Assert.That(args!.PreviousIndex, Is.EqualTo(0));
        Assert.That(args.SelectedIndex, Is.EqualTo(1));
        Assert.That(args.SelectedItem?.Id, Is.EqualTo("mem"));
    }

    [Test]
    public void ControlsDashboardGridPointerPressSelectsHitTile()
    {
        var control = new DashboardGrid();
        control.SetTiles(
        [
            new DashboardTile("cpu", "CPU", 0, 0),
            new DashboardTile("mem", "Memory", 1, 0),
        ]);

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 28, Y: 3),
            new Rect(0, 0, 48, 12));

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedTileId, Is.EqualTo("mem"));
    }

    [Test]
    public void ControlsDashboardGridMoveAndResizeTileUpdateState()
    {
        var control = new DashboardGrid();
        control.SetTiles(
        [
            new DashboardTile("cpu", "CPU", 0, 0),
            new DashboardTile("mem", "Memory", 1, 0),
        ]);

        Assert.That(control.MoveTile("cpu", 1, 1), Is.True);
        Assert.That(control.ResizeTile("cpu", 2, 2), Is.True);
        Assert.That(control.MoveTile("missing", 0, 0), Is.False);
        Assert.That(control.ResizeTile("missing", 1, 1), Is.False);

        var moved = control.Tiles.Single(tile => tile.Id == "cpu");
        Assert.That(moved.Column, Is.EqualTo(1));
        Assert.That(moved.Row, Is.EqualTo(1));
        Assert.That(moved.ColumnSpan, Is.EqualTo(2));
        Assert.That(moved.RowSpan, Is.EqualTo(2));

        Assert.Throws<ArgumentOutOfRangeException>(() => control.MoveTile("cpu", -1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => control.ResizeTile("cpu", 0, 1));
    }

    [Test]
    public void ControlsDashboardGridCanonicalTitleStyleAliasesStayInSync()
    {
        var control = new DashboardGrid();
        var titleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(12, 34, 56));
        var focusedTitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(65, 43, 21));

        control.TitleStyle = titleStyle;
        control.FocusedTitleStyle = focusedTitleStyle;

        Assert.That(control.TitleStyleText, Is.EqualTo(titleStyle));
        Assert.That(control.FocusedTitleStyleText, Is.EqualTo(focusedTitleStyle));
        Assert.That(control.TitleStyle, Is.EqualTo(titleStyle));
        Assert.That(control.FocusedTitleStyle, Is.EqualTo(focusedTitleStyle));
    }

    [Test]
    public void ControlsDashboardGridStyleHooksEmitAnsiAndFocusMarker()
    {
        var control = new DashboardGrid
        {
            IsFocused = true,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30)),
            FocusedBorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(30, 40, 50)),
            TitleStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(40, 50, 60)),
            FocusedTitleStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(60, 70, 80)),
            SelectedTileStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(80, 90, 100)),
        };
        control.SetTiles(
        [
            new DashboardTile("cpu", "CPU", 0, 0, subtitle: "18%"),
            new DashboardTile("mem", "Memory", 1, 0, subtitle: "2.8 GB"),
        ]);

        var output = Render(control, width: 52, height: 12);

        Assert.That(output.Contains("Dashboard Grid *", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;30;40;50", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;60;70;80", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsDashboardGridDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new DashboardGrid();
        control.SetTiles(
        [
            new DashboardTile("cpu", "CPU", 0, 0),
            new DashboardTile("mem", "Memory", 1, 0),
            new DashboardTile("lat", "Latency", 0, 1),
        ]);

        var first = Render(control, width: 52, height: 12);
        var second = Render(control, width: 52, height: 12);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(DashboardGrid control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
