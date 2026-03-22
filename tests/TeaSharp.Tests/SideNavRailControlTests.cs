using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SideNavRailControlTests
{
    [Test]
    public void SideNavRail_KeyboardNavigation_SkipsDisabledAndRaisesSelectionChanged()
    {
        var rail = CreateRail();
        rail.SetItems(
        [
            new NavItem("home", "Home"),
            new NavItem("ops", "Operations", isDisabled: true),
            new NavItem("logs", "Logs"),
        ]);

        SideNavRailSelectionChangedEventArgs? args = null;
        rail.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var handled = rail.Handle(new KeyPressed(Key.Down));

        Assert.That(handled, Is.True);
        Assert.That(rail.SelectedIndex, Is.EqualTo(2));
        Assert.That(rail.SelectedItem?.Id, Is.EqualTo("logs"));
        Assert.That(args, Is.Not.Null);
        Assert.That(args!.PreviousIndex, Is.EqualTo(0));
        Assert.That(args.SelectedIndex, Is.EqualTo(2));
        Assert.That(args.PreviousItem?.Id, Is.EqualTo("home"));
        Assert.That(args.SelectedItem?.Id, Is.EqualTo("logs"));
    }

    [Test]
    public void SideNavRail_Enter_RaisesActivatedForSelectedItem()
    {
        var rail = CreateRail();
        rail.SetItems(
        [
            new NavItem("home", "Home"),
            new NavItem("metrics", "Metrics"),
        ]);
        rail.SetSelectedIndex(1);

        SideNavRailActivatedEventArgs? activated = null;
        rail.Activated += (_, eventArgs) => activated = eventArgs;

        var handled = rail.Handle(new KeyPressed(Key.Enter));

        Assert.That(handled, Is.True);
        Assert.That(activated, Is.Not.Null);
        Assert.That(activated!.SelectedIndex, Is.EqualTo(1));
        Assert.That(activated.SelectedItem.Id, Is.EqualTo("metrics"));
    }

    [Test]
    public void SideNavRail_PointerClick_SelectsAndActivates()
    {
        var rail = CreateRail();
        rail.Border = BorderStyle.None;
        rail.SetItems(
        [
            new NavItem("home", "Home"),
            new NavItem("queue", "Queue"),
            new NavItem("jobs", "Jobs"),
        ]);

        SideNavRailActivatedEventArgs? activated = null;
        rail.Activated += (_, eventArgs) => activated = eventArgs;

        var handled = rail.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 2, Y: 2),
            new Rect(0, 0, 30, 8));

        Assert.That(handled, Is.True);
        Assert.That(rail.SelectedIndex, Is.EqualTo(1));
        Assert.That(rail.SelectedItem?.Id, Is.EqualTo("queue"));
        Assert.That(activated, Is.Not.Null);
        Assert.That(activated!.SelectedItem.Id, Is.EqualTo("queue"));
    }

    [Test]
    public void SideNavRail_StyleAndGlyphHooks_RenderExpectedStateStyling()
    {
        var rail = CreateRail();
        rail.Border = BorderStyle.SingleLine;
        rail.Title = "Rail";
        rail.FocusMarker = "!";
        rail.TitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(9, 8, 7));
        rail.FocusedTitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3));
        rail.BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 22, 33));
        rail.FocusedBorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(44, 55, 66));
        rail.SelectedItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(77, 88, 99));
        rail.FocusedSelectedItemStyle = TeaStyle.Empty.WithBold();
        rail.HoveredItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(12, 34, 56));
        rail.DisabledItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(98, 76, 54));
        rail.Glyphs = new SideNavRailGlyphSet("v", ">", ".", "~", "*", ":", "{", "}", "|");
        rail.SetItems(
        [
            new NavItem("home", "Home", icon: "H", badge: "1"),
            new NavItem("queue", "Queue", icon: "Q"),
            new NavItem("audit", "Audit", icon: "A", isDisabled: true),
        ]);
        rail.SetSelectedIndex(0);
        rail.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, X: 2, Y: 3), new Rect(0, 0, 36, 8));

        var output = Render(rail, width: 36, height: 8);

        Assert.That(output.Contains("Rail !", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("v", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("{1}", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;44;55;66", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;77;88;99", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;12;34;56", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;98;76;54", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("[1;", StringComparison.Ordinal) || output.Contains(";1;", StringComparison.Ordinal) || output.Contains("[1m", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void SideNavRail_DefaultRender_IsMonochromeAndDeterministic()
    {
        var rail = CreateRail();
        rail.Border = BorderStyle.None;
        rail.SetItems(
        [
            new NavItem("home", "Home"),
            new NavItem("metrics", "Metrics"),
        ]);

        var first = Render(rail, width: 28, height: 6);
        var second = Render(rail, width: 28, height: 6);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
        Assert.That(first.Contains("▼", StringComparison.Ordinal), Is.True);
        Assert.That(first.Contains("●", StringComparison.Ordinal), Is.True);
    }

    private static SideNavRail CreateRail()
    {
        return new SideNavRail
        {
            IsFocused = true,
        };
    }

    private static string Render(SideNavRail rail, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        rail.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
