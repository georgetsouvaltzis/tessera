using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class NotificationInboxSelectionChangedEventTests
{
    [Test]
    public void NotificationInboxSelectionChangedEventSelectRaisesExpectedPayload()
    {
        var control = new NotificationInbox();
        control.SetItems(
        [
            new InboxItem("a", "alpha", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("b", "beta", NotificationLevel.Warning, DateTimeOffset.UnixEpoch),
            new InboxItem("c", "gamma", NotificationLevel.Error, DateTimeOffset.UnixEpoch),
        ]);

        ListSelectionChangedEventArgs<InboxItem>? observed = null;
        control.SelectionChanged += (_, args) => observed = args;

        var changed = control.Select(2);

        Assert.That(changed, Is.True);
        Assert.That(observed, Is.Not.Null);
        Assert.That(observed!.PreviousIndex, Is.EqualTo(0));
        Assert.That(observed.SelectedIndex, Is.EqualTo(2));
        Assert.That(observed.PreviousItem?.Id, Is.EqualTo("a"));
        Assert.That(observed.SelectedItem?.Id, Is.EqualTo("c"));
    }

    [Test]
    public void NotificationInboxSelectionChangedEventClearRaisesTransitionToNoSelection()
    {
        var control = new NotificationInbox();
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
    public void NotificationInboxSelectionChangedEventRemoveSelectedRaisesTransitionToRemainingItem()
    {
        var control = new NotificationInbox
        {
            IsFocused = true,
        };
        control.SetItems(
        [
            new InboxItem("a", "alpha", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("b", "beta", NotificationLevel.Warning, DateTimeOffset.UnixEpoch),
        ]);
        control.Select(1);

        ListSelectionChangedEventArgs<InboxItem>? observed = null;
        control.SelectionChanged += (_, args) => observed = args;

        var removed = control.Handle(new KeyPressed(Key.Delete));

        Assert.That(removed, Is.True);
        Assert.That(observed, Is.Not.Null);
        Assert.That(observed!.PreviousIndex, Is.EqualTo(1));
        Assert.That(observed.SelectedIndex, Is.EqualTo(0));
        Assert.That(observed.PreviousItem?.Id, Is.EqualTo("b"));
        Assert.That(observed.SelectedItem?.Id, Is.EqualTo("a"));
    }
}
