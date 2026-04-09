using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ActivityFeedControlTests
{
    [Test]
    public void ControlsActivityFeedRendersTimelineRowsAndKinds()
    {
        var control = new ActivityFeed
        {
            Border = BorderStyle.None,
            ShowTimestamp = false,
            AutoFollow = false,
        };
        control.SetItems(
        [
            new ActivityFeedItem("alice", "deployed", "api", "green", ActivityFeedItemKind.Success, DateTimeOffset.UnixEpoch)
            {
                IsUnread = false,
            },
            new ActivityFeedItem("bot", "alerted", "queue", "lag high", ActivityFeedItemKind.Warning, DateTimeOffset.UnixEpoch),
        ]);

        var output = Render(control, width: 96, height: 4);

        Assert.That(output.Contains("alice deployed api - green", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("bot alerted queue - lag high", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsActivityFeedKeyboardAndPointerSelectionRaisesSelectionChanged()
    {
        var control = new ActivityFeed
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ShowTimestamp = false,
            AutoFollow = false,
        };
        control.SetItems(
        [
            new ActivityFeedItem("a", "event"),
            new ActivityFeedItem("b", "event"),
            new ActivityFeedItem("c", "event"),
        ]);
        var selectionEvents = 0;
        control.SelectionChanged += (_, _) => selectionEvents++;

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var clickHandled = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 2), new Rect(0, 0, 64, 4));

        Assert.That(downHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(selectionEvents, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void ControlsActivityFeedStateStylesEmitAnsiAndDefaultRenderDeterministic()
    {
        var styled = new ActivityFeed
        {
            Border = BorderStyle.None,
            IsFocused = true,
            TimestampFormat = "HH:mm:ss",
            ShowTimestamp = true,
            InfoItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            SuccessItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            WarningItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            ErrorItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            UnreadItemStyle = TesseraStyle.Empty.WithBold(),
            SelectedItemStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(51, 52, 53)),
            FocusedSelectedItemStyle = TesseraStyle.Empty.WithUnderline(),
            HoveredItemStyle = TesseraStyle.Empty.WithItalic(),
            TimestampStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
        };
        styled.SetItems(
        [
            new ActivityFeedItem("svc", "ok", "api", kind: ActivityFeedItemKind.Success, timestamp: DateTimeOffset.UnixEpoch)
            {
                IsUnread = false,
            },
            new ActivityFeedItem("svc", "failed", "worker", kind: ActivityFeedItemKind.Error, timestamp: DateTimeOffset.UnixEpoch),
        ]);
        _ = styled.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), new Rect(0, 0, 96, 4));

        var styledOutput = Render(styled, width: 96, height: 4);
        Assert.That(styledOutput.Contains("38;2;21;22;23", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("38;2;41;42;43", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("38;2;61;62;63", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("48;2;51;52;53", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("\u001b[", StringComparison.Ordinal), Is.True);

        var plain = new ActivityFeed
        {
            Border = BorderStyle.None,
            ShowTimestamp = false,
        };
        plain.Append("bot", "noop", timestamp: DateTimeOffset.UnixEpoch);
        var first = Render(plain, width: 32, height: 2);
        var second = Render(plain, width: 32, height: 2);
        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(ActivityFeed control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
