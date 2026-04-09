using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class CommandOutputControlTests
{
    [Test]
    public void ControlsCommandOutputAppendsAndRendersChannelTags()
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
    public void ControlsCommandOutputKeyboardAndPointerSelectionRaisesSelectionChanged()
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
    public void ControlsCommandOutputStateStylesEmitAnsiAndDefaultRenderDeterministic()
    {
        var styled = new CommandOutput
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ShowTimestamp = true,
            TimestampFormat = "HH:mm:ss",
            StdOutStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            StdErrStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            SystemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            SelectedLineStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(41, 42, 43)),
            FocusedSelectedLineStyle = TesseraStyle.Empty.WithUnderline(),
            HoveredLineStyle = TesseraStyle.Empty.WithBold(),
            TimestampStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
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
