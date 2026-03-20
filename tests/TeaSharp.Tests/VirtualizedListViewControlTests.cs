using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class VirtualizedListViewControlTests
{
    [Test]
    public void VirtualizedListViewRenderUsesResolverForVisibleRowsOnly()
    {
        var resolveCount = 0;
        var control = new VirtualizedListView<string>
        {
            Border = BorderStyle.None,
            IsFocused = true,
        };
        control.SetDataSource(
            1000,
            index =>
            {
                resolveCount++;
                return $"item-{index}";
            });
        _ = control.SetSelectedIndex(500);

        var output = Render(control, width: 18, height: 4);

        Assert.That(output.Contains("> item-500", StringComparison.Ordinal), Is.True);
        Assert.That(resolveCount, Is.EqualTo(4), "Virtualized render should resolve only visible rows.");
    }

    [Test]
    public void VirtualizedListViewKeyboardNavigationAndSelectionEvent()
    {
        var control = new VirtualizedListView<string>
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        control.SetItems(["a", "b", "c", "d", "e"]);

        ListSelectionChangedEventArgs<string>? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var down = control.Handle(new KeyPressed(Key.Down));
        var pageDown = control.Handle(new KeyPressed(Key.PageDown));
        var end = control.Handle(new KeyPressed(Key.End));
        var clamped = control.Handle(new KeyPressed(Key.Down));
        var up = control.Handle(new KeyPressed(Key.Up));

        Assert.That(down, Is.True);
        Assert.That(pageDown, Is.True);
        Assert.That(end, Is.True);
        Assert.That(clamped, Is.False);
        Assert.That(up, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(3));
        Assert.That(control.SelectedItem, Is.EqualTo("d"));
        Assert.That(args, Is.Not.Null);
        Assert.That(args!.SelectedIndex, Is.EqualTo(3));
        Assert.That(args.SelectedItem, Is.EqualTo("d"));
    }

    [Test]
    public void VirtualizedListViewPointerHoverAndClickSelectsRow()
    {
        var control = new VirtualizedListView<string>
        {
            Border = BorderStyle.SingleLine,
        };
        control.SetItems(["alpha", "beta", "gamma", "delta"]);
        var bounds = new Rect(0, 0, 24, 6);

        var hover = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2), bounds);
        var click = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2), bounds);

        Assert.That(hover, Is.True, "Pointer move should update hovered row.");
        Assert.That(click, Is.True, "Pointer click should select clicked row.");
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem, Is.EqualTo("beta"));
    }

    [Test]
    public void VirtualizedListViewClearResetsSelectionAndCount()
    {
        var control = new VirtualizedListView<int>();
        control.SetItems([1, 2, 3]);
        _ = control.SetSelectedIndex(2);

        control.Clear();

        Assert.That(control.Count, Is.EqualTo(0));
        Assert.That(control.SelectedIndex, Is.EqualTo(-1));
        Assert.That(control.SelectedItem, Is.EqualTo(0));
    }

    private static string Render<T>(VirtualizedListView<T> control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
