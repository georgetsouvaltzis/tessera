using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class DockWorkspaceControlTests
{
    [Test]
    public void Controls_DockWorkspace_KeyboardNavigationSkipsDisabledAndRaisesEvent()
    {
        var control = new DockWorkspace
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        control.SetPanes(
        [
            new DockPane("explorer", "Explorer", DockPanePosition.Left, size: 16),
            new DockPane("debug", "Debug", DockPanePosition.Right, size: 20) { IsDisabled = true },
            new DockPane("logs", "Logs", DockPanePosition.Bottom, size: 6),
        ]);

        ListSelectionChangedEventArgs<DockPane>? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var changed = control.Handle(new KeyPressed(Key.Right));

        Assert.That(changed, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedPane?.Id, Is.EqualTo("logs"));
        Assert.That(args, Is.Not.Null);
        Assert.That(args!.PreviousIndex, Is.EqualTo(0));
        Assert.That(args.SelectedIndex, Is.EqualTo(2));
        Assert.That(args.PreviousItem?.Id, Is.EqualTo("explorer"));
        Assert.That(args.SelectedItem?.Id, Is.EqualTo("logs"));
    }

    [Test]
    public void Controls_DockWorkspace_PointerClickSelectsPane()
    {
        var control = new DockWorkspace
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        control.SetPanes(
        [
            new DockPane("top", "Top", DockPanePosition.Top, size: 3) { Lines = ["top"] },
            new DockPane("center", "Center", DockPanePosition.Center) { Lines = ["center"] },
        ]);
        _ = control.Handle(new KeyPressed(Key.Down));

        var bounds = new Rect(0, 0, 40, 12);
        var changed = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);

        Assert.That(changed, Is.True);
        Assert.That(control.IsFocused, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
        Assert.That(control.SelectedPane?.Id, Is.EqualTo("top"));
    }

    [Test]
    public void Controls_DockWorkspace_Render_ShowsPaneTitlesAndSelectionMarker()
    {
        var control = new DockWorkspace
        {
            Border = BorderStyle.None,
            PaneBorder = BorderStyle.SingleLine,
            SelectedPaneMarker = ">",
        };
        control.SetPanes(
        [
            new DockPane("left", "Explorer", DockPanePosition.Left, size: 16) { Lines = ["files"] },
            new DockPane("center", "Editor", DockPanePosition.Center) { Lines = ["main.cs"] },
        ]);

        var output = Render(control, width: 60, height: 14);

        Assert.That(output.Contains("> Explorer", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Editor", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_DockWorkspace_FocusAndBorderStyles_EmitAnsi()
    {
        var focusedBorder = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(120, 70, 33));
        var focusedPaneBorder = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(41, 91, 131));
        var selectedBody = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(9, 19, 29));
        var control = new DockWorkspace
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            PaneBorder = BorderStyle.SingleLine,
            FocusedBorderStyleText = focusedBorder,
            FocusedPaneBorderStyleText = focusedPaneBorder,
            SelectedPaneBodyStyle = selectedBody,
        };
        control.SetPanes(
        [
            new DockPane("console", "Console", DockPanePosition.Bottom, size: 5) { Lines = ["line1"] },
            new DockPane("editor", "Editor", DockPanePosition.Center) { Lines = ["code"] },
        ]);

        var output = Render(control, width: 70, height: 16);

        Assert.That(output.Contains(focusedBorder.Render("┌"), StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(focusedPaneBorder.Render("┌"), StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("48;2;9;19;29", StringComparison.Ordinal), Is.True);
    }

    private static string Render(DockWorkspace control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
