using NUnit.Framework;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class NotificationsSelectionChangedEventTests
{
    [Test]
    public void NotificationsSelectionChangedEventSetSelectedIndexRaisesExpectedPayload()
    {
        var control = new Notifications();
        control.SetItems(
        [
            new InboxItem("a", "alpha", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("b", "beta", NotificationLevel.Warning, DateTimeOffset.UnixEpoch),
            new InboxItem("c", "gamma", NotificationLevel.Error, DateTimeOffset.UnixEpoch),
        ]);

        ListSelectionChangedEventArgs<InboxItem>? observed = null;
        control.SelectionChanged += (_, args) => observed = args;

        var changed = control.SetSelectedIndex(2);

        Assert.That(changed, Is.True);
        Assert.That(observed, Is.Not.Null);
        Assert.That(observed!.PreviousIndex, Is.EqualTo(0));
        Assert.That(observed.SelectedIndex, Is.EqualTo(2));
        Assert.That(observed.PreviousItem?.Id, Is.EqualTo("a"));
        Assert.That(observed.SelectedItem?.Id, Is.EqualTo("c"));
    }

    [Test]
    public void NotificationsSelectionChangedEventClearRaisesTransitionToNoSelection()
    {
        var control = new Notifications();
        control.SetItems(
        [
            new InboxItem("a", "alpha", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("b", "beta", NotificationLevel.Warning, DateTimeOffset.UnixEpoch),
        ]);

        ListSelectionChangedEventArgs<InboxItem>? observed = null;
        control.SelectionChanged += (_, args) => observed = args;

        control.Clear();

        Assert.That(observed, Is.Not.Null);
        Assert.That(observed!.PreviousIndex, Is.EqualTo(0));
        Assert.That(observed.SelectedIndex, Is.EqualTo(-1));
        Assert.That(observed.PreviousItem?.Id, Is.EqualTo("a"));
        Assert.That(observed.SelectedItem, Is.Null);
    }

    [Test]
    public void NotificationsSelectionChangedEventRemoveSelectedRaisesTransitionToRemainingItem()
    {
        var control = new Notifications();
        control.SetItems(
        [
            new InboxItem("a", "alpha", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("b", "beta", NotificationLevel.Warning, DateTimeOffset.UnixEpoch),
        ]);
        control.SetSelectedIndex(1);

        ListSelectionChangedEventArgs<InboxItem>? observed = null;
        control.SelectionChanged += (_, args) => observed = args;

        var removed = control.RemoveSelected();

        Assert.That(removed, Is.True);
        Assert.That(observed, Is.Not.Null);
        Assert.That(observed!.PreviousIndex, Is.EqualTo(1));
        Assert.That(observed.SelectedIndex, Is.EqualTo(0));
        Assert.That(observed.PreviousItem?.Id, Is.EqualTo("b"));
        Assert.That(observed.SelectedItem?.Id, Is.EqualTo("a"));
    }
}
