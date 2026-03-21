using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class NotificationInboxControlTests
{
    [Test]
    public void NotificationInboxRenderShowsTitleTimestampAndUnreadMarker()
    {
        var control = new NotificationInbox();
        control.SetItems(
        [
            new InboxItem("n1", "Build finished", NotificationLevel.Success, new DateTimeOffset(2026, 3, 21, 10, 5, 0, TimeSpan.Zero), "CI"),
            new InboxItem("n2", "Disk warning", NotificationLevel.Warning, new DateTimeOffset(2026, 3, 21, 10, 6, 0, TimeSpan.Zero), "Host"),
        ]);

        var output = Render(control, width: 80, height: 8);

        Assert.That(output.Contains("Notification Inbox", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("10:05", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("•", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Build finished", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void NotificationInboxKeyboardNavigationAndReadMarkingWork()
    {
        var control = new NotificationInbox
        {
            IsFocused = true,
        };
        control.SetItems(
        [
            new InboxItem("n1", "One", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("n2", "Two", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
        ]);

        var moved = control.Handle(new KeyPressed(Key.Down));
        var marked = control.Handle(new KeyPressed(Key.Enter));

        Assert.That(moved, Is.True);
        Assert.That(marked, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.Items[1].IsRead, Is.True);
    }

    [Test]
    public void NotificationInboxPointerPressSelectsRowAndMarksRead()
    {
        var control = new NotificationInbox();
        control.SetItems(
        [
            new InboxItem("n1", "One", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("n2", "Two", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
        ]);

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 5, Y: 2),
            new Rect(0, 0, 60, 6));

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.Items[1].IsRead, Is.True);
    }

    [Test]
    public void NotificationInboxSelectedRowStyleEmitsAnsi()
    {
        var control = new NotificationInbox
        {
            SelectedItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(120, 80, 200)),
        };
        control.SetItems(
        [
            new InboxItem("n1", "One", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("n2", "Two", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
        ]);
        control.Select(1);

        var output = Render(control, width: 64, height: 6);

        Assert.That(output.Contains("38;2;120;80;200", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void NotificationInboxDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new NotificationInbox();
        control.SetItems(
        [
            new InboxItem("n1", "One", NotificationLevel.Info, DateTimeOffset.UnixEpoch),
            new InboxItem("n2", "Two", NotificationLevel.Warning, DateTimeOffset.UnixEpoch),
        ]);

        var bounds = new Rect(0, 0, 64, 8);
        var firstCanvas = new Canvas(64, 8);
        var secondCanvas = new Canvas(64, 8);
        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(NotificationInbox control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
