using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class LogTailPanelControlTests
{
    [Test]
    public void ControlsLogTailPanelRendersEntriesWithLevelAndSource()
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
    public void ControlsLogTailPanelKeyboardAndPointerSelectionRaisesSelectionChanged()
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
    public void ControlsLogTailPanelStateStylesEmitAnsiAndDefaultRenderDeterministic()
    {
        var styled = new LogTailPanel
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ShowTimestamp = false,
            EntryStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            WarningEntryStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            SelectedEntryStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(31, 32, 33)),
            FocusedSelectedEntryStyle = TesseraStyle.Empty.WithUnderline(),
            HoveredEntryStyle = TesseraStyle.Empty.WithBold(),
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

    [Test]
    public void ControlsLogTailPanelDisplayToggleRebuildsCachedBodies()
    {
        var control = new LogTailPanel
        {
            Border = BorderStyle.None,
            ShowTimestamp = false,
            ShowLevel = false,
            ShowSource = false,
        };
        control.Append("startup complete", LogLevel.Info, DateTimeOffset.UnixEpoch, "api");

        var plain = Render(control, width: 80, height: 2);

        control.ShowLevel = true;
        control.ShowSource = true;
        var enriched = Render(control, width: 80, height: 2);

        Assert.That(plain.Contains("startup complete", StringComparison.Ordinal), Is.True);
        Assert.That(plain.Contains("INF api:", StringComparison.Ordinal), Is.False);
        Assert.That(enriched.Contains("INF api: startup complete", StringComparison.Ordinal), Is.True);
    }

    private static string Render(LogTailPanel control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
