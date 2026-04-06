using NUnit.Framework;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class NotificationsPrimaryApiTests
{
    [Test]
    public void NotificationsPrimaryApiSetAddSelectRemoveAndMarkAllReadWork()
    {
        var control = new Notifications();
        control.SetItems(
        [
            new InboxItem("a", "alpha", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("b", "beta", NotificationLevel.Warning, DateTimeOffset.UnixEpoch),
        ]);

        var moved = control.SetSelectedIndex(1);
        var selectedAgain = control.Select(1);
        control.MarkAllRead();
        var removed = control.RemoveSelected();
        control.Add(new InboxItem("c", "gamma", NotificationLevel.Error, DateTimeOffset.UnixEpoch));

        Assert.That(moved, Is.True);
        Assert.That(selectedAgain, Is.False);
        Assert.That(removed, Is.True);
        Assert.That(control.Count, Is.EqualTo(2));
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem?.Id, Is.EqualTo("c"));
        Assert.That(control.Items[0].IsRead, Is.True);
        Assert.That(control.Items[1].IsRead, Is.False);
    }

    [Test]
    public void NotificationsPrimaryApiSelectedAccessorsStayConsistentAcrossEmptyAndClampedSelection()
    {
        var control = new Notifications();

        Assert.That(control.SelectedIndex, Is.EqualTo(-1));
        Assert.That(control.SelectedItem, Is.Null);
        Assert.That(control.SetSelectedIndex(0), Is.False);
        Assert.That(control.RemoveSelected(), Is.False);

        control.SetItems(
        [
            new InboxItem("one", "one", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("two", "two", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
        ]);

        var clamped = control.SetSelectedIndex(999);

        Assert.That(clamped, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem?.Id, Is.EqualTo("two"));
        control.Clear();
        Assert.That(control.SelectedIndex, Is.EqualTo(-1));
        Assert.That(control.SelectedItem, Is.Null);
    }

    [Test]
    public void NotificationsPrimaryApiSetItemsClonesInboundItems()
    {
        var original = new InboxItem("item-1", "original", NotificationLevel.Info, DateTimeOffset.UnixEpoch, "ops", isRead: false);
        var control = new Notifications();

        control.SetItems([original]);
        original.Message = "mutated outside";
        original.IsRead = true;
        original.Source = "external";

        Assert.That(control.Items[0].Message, Is.EqualTo("original"));
        Assert.That(control.Items[0].IsRead, Is.False);
        Assert.That(control.Items[0].Source, Is.EqualTo("ops"));
    }

    [Test]
    public void NotificationsPrimaryApiAddClonesInboundItem()
    {
        var original = new InboxItem("item-2", "before", NotificationLevel.Warning, DateTimeOffset.UnixEpoch, "ci", isRead: false);
        var control = new Notifications();

        control.Add(original);
        original.Message = "after";
        original.IsRead = true;
        original.Source = "mutated";

        Assert.That(control.Items[0].Message, Is.EqualTo("before"));
        Assert.That(control.Items[0].IsRead, Is.False);
        Assert.That(control.Items[0].Source, Is.EqualTo("ci"));
    }

    [Test]
    public void NotificationsPrimaryApiPushRemainsCompatibleAndTrimsToMaxItems()
    {
        var control = new Notifications
        {
            MaxItems = 2,
        };

        control.Push("first", NotificationLevel.Info, "n1");
        control.Push("second", NotificationLevel.Warning, "n2");
        control.Push("third", NotificationLevel.Error, "n3");

        Assert.That(control.Count, Is.EqualTo(2));
        Assert.That(control.Items[0].Id, Is.EqualTo("n2"));
        Assert.That(control.Items[1].Id, Is.EqualTo("n3"));
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem?.Message, Is.EqualTo("third"));
    }
}
