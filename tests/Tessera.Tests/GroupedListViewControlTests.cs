using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class GroupedListViewControlTests
{
    [Test]
    public void GroupedListViewRenderShowsHeadersItemsAndMarkers()
    {
        var control = CreateControl();
        var output = Render(control, 32, 8);

        Assert.That(output.Contains("▼ CPU", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("  user:31%", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("▼ Memory", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void GroupedListViewKeyboardNavigationCollapseExpandAndSelectionEvent()
    {
        var control = CreateControl();
        control.IsFocused = true;
        GroupedListSelectionChangedEventArgs<string, string>? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var down = control.Handle(new KeyPressed(Key.Down));
        var downAgain = control.Handle(new KeyPressed(Key.Down));
        var leftCollapse = control.Handle(new KeyPressed(Key.Left));
        var rightExpand = control.Handle(new KeyPressed(Key.Right));
        var end = control.Handle(new KeyPressed(Key.End));

        Assert.That(down, Is.True);
        Assert.That(downAgain, Is.True);
        Assert.That(leftCollapse, Is.True, "Left should collapse selected row's group.");
        Assert.That(rightExpand, Is.True, "Right should expand selected row's group.");
        Assert.That(end, Is.True);
        Assert.That(control.SelectedGroupIndex, Is.EqualTo(1));
        Assert.That(args, Is.Not.Null);
        Assert.That(TestAssert.NotNull(args).CurrentGroupIndex, Is.EqualTo(control.SelectedGroupIndex));
    }

    [Test]
    public void GroupedListViewPointerClickHeaderTogglesAndItemClickSelects()
    {
        var control = CreateControl();
        var bounds = new Rect(0, 0, 36, 8);

        var headerClick = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 1), bounds);
        var afterHeader = Render(control, 36, 8);
        var itemClick = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 4, 2), bounds);

        Assert.That(headerClick, Is.True, "Clicking header should toggle collapse state.");
        Assert.That(afterHeader.Contains("▶ CPU", StringComparison.Ordinal), Is.True,
            "Collapsed group should render collapsed marker.");
        Assert.That(itemClick, Is.True, "Clicking visible item row should select it.");
        Assert.That(control.SelectedItem, Is.EqualTo("used:6.1GB"));
    }

    [Test]
    public void GroupedListViewSetSelectedItemMovesSelection()
    {
        var control = CreateControl();

        var changed = control.SetSelectedItem(1, 0);

        Assert.That(changed, Is.True);
        Assert.That(control.SelectedGroupIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItemIndex, Is.EqualTo(0));
        Assert.That(control.SelectedItem, Is.EqualTo("used:6.1GB"));
    }

    private static GroupedListView<string, string> CreateControl()
    {
        var control = new GroupedListView<string, string> { Border = BorderStyle.SingleLine, Title = "System" };
        control.SetGroups(
        [
            new GroupedListViewGroup<string, string>("CPU", ["user:31%", "sys:9%"]),
            new GroupedListViewGroup<string, string>("Memory", ["used:6.1GB", "cache:2.4GB"])
        ]);
        return control;
    }

    private static string Render<TGroup, TItem>(GroupedListView<TGroup, TItem> control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
