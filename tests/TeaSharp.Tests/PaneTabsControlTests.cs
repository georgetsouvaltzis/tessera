using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class PaneTabsControlTests
{
    [Test]
    public void Controls_PaneTabs_KeyboardNavigationSkipsDisabledAndRaisesEvent()
    {
        var control = new PaneTabs
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        control.SetTabs(
        [
            new PaneTabItem("home", "Home"),
            new PaneTabItem("logs", "Logs", isDisabled: true),
            new PaneTabItem("diag", "Diagnostics"),
        ]);

        PaneTabSelectionChangedEventArgs? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var changed = control.Handle(new KeyPressed(Key.Right));

        Assert.That(changed, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedItem?.Id, Is.EqualTo("diag"));
        Assert.That(args, Is.Not.Null);
        Assert.That(args!.PreviousIndex, Is.EqualTo(0));
        Assert.That(args.SelectedIndex, Is.EqualTo(2));
        Assert.That(args.PreviousItem?.Id, Is.EqualTo("home"));
        Assert.That(args.SelectedItem?.Id, Is.EqualTo("diag"));
    }

    [Test]
    public void Controls_PaneTabs_PointerClickSelectsTab()
    {
        var control = new PaneTabs
        {
            Border = BorderStyle.None,
        };
        control.SetTabs(
        [
            new PaneTabItem("a", "A"),
            new PaneTabItem("b", "B"),
            new PaneTabItem("c", "C"),
        ]);
        var bounds = new Rect(0, 0, 30, 1);

        var changed = false;
        for (var x = bounds.X; x < bounds.Right; x++)
        {
            changed = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, x, 0), bounds);
            if (control.SelectedIndex == 1)
            {
                break;
            }
        }

        Assert.That(changed, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem?.Id, Is.EqualTo("b"));
    }

    [Test]
    public void Controls_PaneTabs_PointerMotionDoesNotSelectHoveredTab()
    {
        var control = new PaneTabs
        {
            Border = BorderStyle.None,
        };
        control.SetTabs(
        [
            new PaneTabItem("a", "A"),
            new PaneTabItem("b", "B"),
            new PaneTabItem("c", "C"),
        ]);
        var bounds = new Rect(0, 0, 30, 1);

        var handled = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 10, 0), bounds);

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
        Assert.That(control.SelectedItem?.Id, Is.EqualTo("a"));
    }

    [Test]
    public void Controls_PaneTabs_Render_EmitsFocusMarkerAndStyles()
    {
        var focusedBorder = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(66, 77, 88));
        var selectedStyle = TeaStyle.Empty.WithBold();
        var control = new PaneTabs
        {
            Title = "Pane",
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            FocusedBorderStyleText = focusedBorder,
            SelectedTabStyle = selectedStyle,
            FocusedSelectedTabStyle = TeaStyle.Empty.WithUnderline(),
        };
        control.SetTabs([new PaneTabItem("home", "Home")]);

        var output = Render(control, width: 40, height: 4);

        Assert.That(output.Contains("Pane *", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(focusedBorder.Render("┌"), StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("[Home]", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("[1;", StringComparison.Ordinal) || output.Contains(";1;", StringComparison.Ordinal) || output.Contains("[1m", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_PaneTabs_EmptyStateRendersText()
    {
        var control = new PaneTabs
        {
            Border = BorderStyle.None,
            EmptyText = "(empty)",
        };

        var output = Render(control, width: 24, height: 1);
        Assert.That(output.Contains("(empty)", StringComparison.Ordinal), Is.True);
    }

    private static string Render(PaneTabs control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
