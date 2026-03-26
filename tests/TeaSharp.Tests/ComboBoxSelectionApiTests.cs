using NUnit.Framework;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ComboBoxSelectionApiTests
{
    [Test]
    public void ComboBoxSelectionApi_SetSelectedIndex_UsesCanonicalClamping_AndUpdatesFilterText()
    {
        var control = new ComboBox();
        control.SetItems(["alpha", "beta", "gamma"]);
        control.SetFilterText("ga");
        var events = new List<SelectionChangedEventArgs>();
        control.SelectionChanged += (_, args) => events.Add(args);

        Assert.That(control.SetSelectedIndex(99), Is.True);
        Assert.That(control.SelectedItem, Is.EqualTo("gamma"));
        Assert.That(control.FilterText, Is.EqualTo("gamma"));

        Assert.That(control.SetSelectedIndex(99), Is.False);

        Assert.That(control.SetSelectedIndex(-10), Is.True);
        Assert.That(control.SelectedItem, Is.EqualTo("alpha"));
        Assert.That(control.FilterText, Is.EqualTo("alpha"));

        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events[0].PreviousIndex, Is.EqualTo(-1));
        Assert.That(events[0].SelectedIndex, Is.EqualTo(2));
        Assert.That(events[0].PreviousItem, Is.EqualTo(string.Empty));
        Assert.That(events[0].SelectedItem, Is.EqualTo("gamma"));
        Assert.That(events[1].PreviousIndex, Is.EqualTo(2));
        Assert.That(events[1].SelectedIndex, Is.EqualTo(0));
        Assert.That(events[1].PreviousItem, Is.EqualTo("gamma"));
        Assert.That(events[1].SelectedItem, Is.EqualTo("alpha"));
    }

    [Test]
    public void ComboBoxSelectionApi_TrySetSelectedItem_UsesOrdinalLookupAndSelectionChangeSemantics()
    {
        var control = new ComboBox();
        control.SetItems(["alpha", "Beta", "gamma"]);

        Assert.That(control.TrySetSelectedItem("Beta"), Is.True);
        Assert.That(control.SelectedItem, Is.EqualTo("Beta"));
        Assert.That(control.FilterText, Is.EqualTo("Beta"));

        Assert.That(control.TrySetSelectedItem("Beta"), Is.False, "Re-selecting the same item should be a no-op.");
        Assert.That(control.TrySetSelectedItem("beta"), Is.False, "Lookup should be ordinal and case-sensitive.");
        Assert.That(control.TrySetSelectedItem("missing"), Is.False);
    }

    [Test]
    public void ComboBoxSelectionApi_EmptyControl_SetSelectedApisReturnFalse()
    {
        var control = new ComboBox();

        Assert.That(control.SelectedItem, Is.EqualTo(string.Empty));
        Assert.That(control.FilterText, Is.EqualTo(string.Empty));
        Assert.That(control.SetSelectedIndex(0), Is.False);
        Assert.That(control.TrySetSelectedItem("anything"), Is.False);
        Assert.That(control.SelectedItem, Is.EqualTo(string.Empty));
    }
}
