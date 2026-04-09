using NUnit.Framework;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ChoiceSelectionApiTests
{
    [Test]
    public void ChoiceSelectionApiSetSelectedIndexUsesCanonicalClampingAndRaisesEventsOnTransitions()
    {
        var control = new Choice();
        control.SetItems(["dev", "stage", "prod"]);
        var events = new List<SelectionChangedEventArgs>();
        control.SelectionChanged += (_, args) => events.Add(args);

        Assert.That(control.SetSelectedIndex(99), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedItem, Is.EqualTo("prod"));

        Assert.That(control.SetSelectedIndex(99), Is.False);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.SetSelectedIndex(-10), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
        Assert.That(control.SelectedItem, Is.EqualTo("dev"));

        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events[0].PreviousIndex, Is.EqualTo(0));
        Assert.That(events[0].SelectedIndex, Is.EqualTo(2));
        Assert.That(events[0].PreviousItem, Is.EqualTo("dev"));
        Assert.That(events[0].SelectedItem, Is.EqualTo("prod"));
        Assert.That(events[1].PreviousIndex, Is.EqualTo(2));
        Assert.That(events[1].SelectedIndex, Is.EqualTo(0));
        Assert.That(events[1].PreviousItem, Is.EqualTo("prod"));
        Assert.That(events[1].SelectedItem, Is.EqualTo("dev"));
    }

    [Test]
    public void ChoiceSelectionApiTrySetSelectedItemUsesOrdinalLookupAndSelectionChangeSemantics()
    {
        var control = new Choice();
        control.SetItems(["alpha", "Beta", "gamma"]);

        Assert.That(control.TrySetSelectedItem("Beta"), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem, Is.EqualTo("Beta"));

        Assert.That(control.TrySetSelectedItem("Beta"), Is.False, "Re-selecting the same item should be a no-op.");
        Assert.That(control.TrySetSelectedItem("beta"), Is.False, "Lookup should be ordinal and case-sensitive.");
        Assert.That(control.TrySetSelectedItem("missing"), Is.False);
    }

    [Test]
    public void ChoiceSelectionApiEmptyControlSetSelectedApisReturnFalse()
    {
        var control = new Choice();

        Assert.That(control.SelectedIndex, Is.EqualTo(-1));
        Assert.That(control.SelectedItem, Is.EqualTo(string.Empty));
        Assert.That(control.SetSelectedIndex(0), Is.False);
        Assert.That(control.TrySetSelectedItem("anything"), Is.False);
        Assert.That(control.SelectedIndex, Is.EqualTo(-1));
        Assert.That(control.SelectedItem, Is.EqualTo(string.Empty));
    }
}
