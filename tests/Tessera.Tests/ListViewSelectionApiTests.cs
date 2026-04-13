using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ListViewSelectionApiTests
{
    [Test]
    public void ListViewSelectionApiSetSelectedIndexAndSelectUseCanonicalClampingSemantics()
    {
        var control = new ListView<string>(static item => item);
        control.SetItems(["alpha", "beta", "gamma"]);

        Assert.That(control.SetSelectedIndex(99), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedItem, Is.EqualTo("gamma"));

        Assert.That(control.SetSelectedIndex(99), Is.False);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.Select(-10), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
        Assert.That(control.SelectedItem, Is.EqualTo("alpha"));
    }

    [Test]
    public void ListViewSelectionApiEmptyListSetSelectedIndexAndSelectReturnFalse()
    {
        var control = new ListView<string>(static item => item);

        Assert.That(control.SelectedIndex, Is.EqualTo(-1));
        Assert.That(control.SelectedItem, Is.Null);
        Assert.That(control.SetSelectedIndex(0), Is.False);
        Assert.That(control.Select(1), Is.False);
        Assert.That(control.SelectedIndex, Is.EqualTo(-1));
        Assert.That(control.SelectedItem, Is.Null);
    }

    [Test]
    public void ListViewSelectionApiSetSelectedIndexRaisesSelectionChangedOnSelectionTransitionsOnly()
    {
        var control = new ListView<string>(static item => item);
        control.SetItems(["alpha", "beta", "gamma"]);
        var events = new List<ListSelectionChangedEventArgs<string>>();
        control.SelectionChanged += (_, args) => events.Add(args);

        Assert.That(control.SetSelectedIndex(2), Is.True);
        Assert.That(control.SetSelectedIndex(2), Is.False);
        Assert.That(control.Select(1), Is.True);

        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events[0].PreviousIndex, Is.EqualTo(0));
        Assert.That(events[0].SelectedIndex, Is.EqualTo(2));
        Assert.That(events[0].PreviousItem, Is.EqualTo("alpha"));
        Assert.That(events[0].SelectedItem, Is.EqualTo("gamma"));
        Assert.That(events[1].PreviousIndex, Is.EqualTo(2));
        Assert.That(events[1].SelectedIndex, Is.EqualTo(1));
        Assert.That(events[1].PreviousItem, Is.EqualTo("gamma"));
        Assert.That(events[1].SelectedItem, Is.EqualTo("beta"));
    }

    [Test]
    public void ListViewSelectionApiPointerPressInsideRowLaneSelectsRowBeyondLabelGlyphWidth()
    {
        var control = new ListView<string>(static item => item) { Border = BorderStyle.None };
        control.SetItems(["a", "b", "c"]);

        var changed = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 19, 1),
            new Rect(0, 0, 20, 4));

        Assert.That(changed, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem, Is.EqualTo("b"));
    }

    [Test]
    public void ListViewSelectionApiKeyboardNavigationRemainsUnchangedAfterPointerHitAreaUpdate()
    {
        var control = new ListView<string>(static item => item);
        control.SetItems(["alpha", "beta", "gamma"]);
        control.IsFocused = true;

        Assert.That(control.SelectedIndex, Is.EqualTo(0));
        Assert.That(control.Handle(new KeyPressed(Key.Down)), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.Handle(new KeyPressed(Key.Up)), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
    }
}
