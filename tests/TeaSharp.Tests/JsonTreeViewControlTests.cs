using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class JsonTreeViewControlTests
{
    [Test]
    public void Controls_JsonTreeView_SetJson_RendersHierarchy()
    {
        var control = new JsonTreeView
        {
            Border = BorderStyle.None,
        };
        control.SetJson("""{"user":{"name":"anna","role":"admin"},"ok":true}""");

        var output = Render(control, width: 64, height: 6);

        Assert.That(output.Contains("> ▼ user: {...}", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("• name: \"anna\"", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("• ok: true", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_JsonTreeView_KeyboardNavigationExpandCollapseAndSelectionEvents()
    {
        var control = new JsonTreeView
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        control.SetJson("""{"user":{"name":"anna","role":"admin"},"ok":true}""");
        JsonTreeSelectionChangedEventArgs? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var down = control.Handle(new KeyPressed(Key.Down));
        var up = control.Handle(new KeyPressed(Key.Up));
        var collapse = control.Handle(new KeyPressed(Key.Enter));
        var collapsedOutput = Render(control, width: 64, height: 6);
        var expand = control.Handle(new KeyPressed(Key.Enter));
        var expandedOutput = Render(control, width: 64, height: 6);

        Assert.That(down, Is.True);
        Assert.That(up, Is.True);
        Assert.That(collapse, Is.True);
        Assert.That(expand, Is.True);
        Assert.That(args, Is.Not.Null);
        Assert.That(args!.CurrentIndex, Is.EqualTo(control.SelectedIndex));
        Assert.That(collapsedOutput.Contains("▶ user: {...}", StringComparison.Ordinal), Is.True);
        Assert.That(collapsedOutput.Contains("name: \"anna\"", StringComparison.Ordinal), Is.False);
        Assert.That(expandedOutput.Contains("▼ user: {...}", StringComparison.Ordinal), Is.True);
        Assert.That(expandedOutput.Contains("name: \"anna\"", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_JsonTreeView_PointerHoverAndClick_SelectNode()
    {
        var control = new JsonTreeView
        {
            Border = BorderStyle.SingleLine,
        };
        control.SetJson("""{"user":{"name":"anna","role":"admin"},"ok":true}""");
        var bounds = new Rect(0, 0, 64, 8);

        var move = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 3, 2), bounds);
        var click = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 3, 2), bounds);

        Assert.That(move, Is.True);
        Assert.That(click, Is.True);
        Assert.That(control.SelectedNode?.Key, Is.EqualTo("name"));
    }

    [Test]
    public void Controls_JsonTreeView_TrySetJson_ReturnsFalseForInvalidJson()
    {
        var control = new JsonTreeView();

        var ok = control.TrySetJson("{invalid", out var error);

        Assert.That(ok, Is.False);
        Assert.That(string.IsNullOrWhiteSpace(error), Is.False);
    }

    [Test]
    public void Controls_JsonTreeView_StateStylesRenderAnsi_AndDefaultRenderIsDeterministic()
    {
        var control = new JsonTreeView
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ContainerStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
            ValueStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
            SelectedRowStyle = TeaStyle.Empty.WithBold(),
            FocusedSelectedRowStyle = TeaStyle.Empty.WithUnderline(),
            HoveredRowStyle = TeaStyle.Empty.WithItalic(),
        };
        control.SetJson("""{"user":{"name":"anna"},"ok":true}""");
        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 1), new Rect(0, 0, 64, 5));

        var first = Render(control, width: 64, height: 5);
        var second = Render(control, width: 64, height: 5);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("38;2;10;11;12", StringComparison.Ordinal), Is.True);
        Assert.That(first.Contains("38;2;20;21;22", StringComparison.Ordinal), Is.True);
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.True);

        var plain = new JsonTreeView
        {
            Border = BorderStyle.None,
        };
        plain.SetJson("""{"a":1}""");
        var plainOutput = Render(plain, width: 32, height: 3);
        Assert.That(plainOutput.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(JsonTreeView control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}

[TestFixture]
[NonParallelizable]
public sealed class CommandOutputControlTests
{
    [Test]
    public void Controls_CommandOutput_AppendsAndRendersChannelTags()
    {
        var control = new CommandOutput
        {
            Border = BorderStyle.None,
            ShowTimestamp = false,
        };
        control.AppendStdOut("build started", DateTimeOffset.UnixEpoch);
        control.AppendStdErr("compile failed", DateTimeOffset.UnixEpoch);
        control.AppendSystem("retrying", DateTimeOffset.UnixEpoch);

        var output = Render(control, width: 64, height: 4);

        Assert.That(output.Contains("OUT build started", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("ERR compile failed", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("SYS retrying", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_CommandOutput_KeyboardAndPointerSelection_RaisesSelectionChanged()
    {
        var control = new CommandOutput
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ShowTimestamp = false,
        };
        control.SetLines(
        [
            new CommandOutputLine("line-0", CommandOutputChannel.StdOut, DateTimeOffset.UnixEpoch),
            new CommandOutputLine("line-1", CommandOutputChannel.StdErr, DateTimeOffset.UnixEpoch),
            new CommandOutputLine("line-2", CommandOutputChannel.System, DateTimeOffset.UnixEpoch),
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
    public void Controls_CommandOutput_StateStylesEmitAnsi_AndDefaultRenderDeterministic()
    {
        var styled = new CommandOutput
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ShowTimestamp = true,
            TimestampFormat = "HH:mm:ss",
            StdOutStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            StdErrStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            SystemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            SelectedLineStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(41, 42, 43)),
            FocusedSelectedLineStyle = TeaStyle.Empty.WithUnderline(),
            HoveredLineStyle = TeaStyle.Empty.WithBold(),
            TimestampStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
        };
        styled.SetLines(
        [
            new CommandOutputLine("stdout", CommandOutputChannel.StdOut, DateTimeOffset.UnixEpoch),
            new CommandOutputLine("stderr", CommandOutputChannel.StdErr, DateTimeOffset.UnixEpoch),
            new CommandOutputLine("system", CommandOutputChannel.System, DateTimeOffset.UnixEpoch),
        ]);
        _ = styled.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), new Rect(0, 0, 80, 4));

        var styledOutput = Render(styled, width: 80, height: 4);
        Assert.That(styledOutput.Contains("38;2;11;12;13", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("38;2;21;22;23", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("38;2;31;32;33", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("48;2;41;42;43", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("38;2;51;52;53", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("\u001b[", StringComparison.Ordinal), Is.True);

        var plain = new CommandOutput
        {
            Border = BorderStyle.None,
            ShowTimestamp = false,
        };
        plain.SetLines([new CommandOutputLine("plain", CommandOutputChannel.StdOut, DateTimeOffset.UnixEpoch)]);
        var first = Render(plain, width: 24, height: 2);
        var second = Render(plain, width: 24, height: 2);
        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(CommandOutput control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}

[TestFixture]
[NonParallelizable]
public sealed class LogTailPanelControlTests
{
    [Test]
    public void Controls_LogTailPanel_RendersEntriesWithLevelAndSource()
    {
        var control = new LogTailPanel
        {
            Border = BorderStyle.None,
            ShowTimestamp = false,
            ShowLevel = true,
            ShowSource = true,
            AutoFollow = false,
        };
        control.Append("startup complete", LogLevel.Info, DateTimeOffset.UnixEpoch, "api");
        control.Append("failed to bind", LogLevel.Error, DateTimeOffset.UnixEpoch, "worker");

        var output = Render(control, width: 80, height: 4);

        Assert.That(output.Contains("INF api: startup complete", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("ERR worker: failed to bind", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_LogTailPanel_KeyboardAndPointerSelection_RaisesSelectionChanged()
    {
        var control = new LogTailPanel
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ShowTimestamp = false,
            AutoFollow = false,
        };
        control.SetEntries(
        [
            new LogEntry("line-0"),
            new LogEntry("line-1"),
            new LogEntry("line-2"),
        ]);

        var selectionEvents = 0;
        control.SelectionChanged += (_, _) => selectionEvents++;

        var homeHandled = control.Handle(new KeyPressed(Key.Home));
        var clickHandled = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), new Rect(0, 0, 64, 4));

        Assert.That(homeHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(selectionEvents, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void Controls_LogTailPanel_StateStylesEmitAnsi_AndDefaultRenderDeterministic()
    {
        var styled = new LogTailPanel
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ShowTimestamp = false,
            EntryStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            WarningEntryStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            SelectedEntryStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(31, 32, 33)),
            FocusedSelectedEntryStyle = TeaStyle.Empty.WithUnderline(),
            HoveredEntryStyle = TeaStyle.Empty.WithBold(),
        };
        styled.SetEntries(
        [
            new LogEntry("alpha", LogLevel.Warning),
            new LogEntry("beta", LogLevel.Info),
        ]);
        _ = styled.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), new Rect(0, 0, 64, 4));

        var styledOutput = Render(styled, width: 64, height: 4);
        Assert.That(styledOutput.Contains("38;2;11;12;13", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("38;2;21;22;23", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("48;2;31;32;33", StringComparison.Ordinal), Is.True);
        Assert.That(styledOutput.Contains("\u001b[", StringComparison.Ordinal), Is.True);

        var plain = new LogTailPanel
        {
            Border = BorderStyle.None,
            ShowTimestamp = false,
        };
        plain.Append("plain");
        var first = Render(plain, width: 24, height: 2);
        var second = Render(plain, width: 24, height: 2);
        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(LogTailPanel control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}

[TestFixture]
[NonParallelizable]
public sealed class ActivityFeedControlTests
{
    [Test]
    public void Controls_ActivityFeed_RendersTimelineRowsAndKinds()
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
    public void Controls_ActivityFeed_KeyboardAndPointerSelection_RaisesSelectionChanged()
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
    public void Controls_ActivityFeed_StateStylesEmitAnsi_AndDefaultRenderDeterministic()
    {
        var styled = new ActivityFeed
        {
            Border = BorderStyle.None,
            IsFocused = true,
            TimestampFormat = "HH:mm:ss",
            ShowTimestamp = true,
            InfoItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            SuccessItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            WarningItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            ErrorItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            UnreadItemStyle = TeaStyle.Empty.WithBold(),
            SelectedItemStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(51, 52, 53)),
            FocusedSelectedItemStyle = TeaStyle.Empty.WithUnderline(),
            HoveredItemStyle = TeaStyle.Empty.WithItalic(),
            TimestampStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
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
