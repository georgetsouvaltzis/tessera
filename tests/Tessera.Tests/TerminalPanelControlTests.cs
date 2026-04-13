using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TerminalPanelControlTests
{
    [Test]
    public void TerminalPanelAppendAndRenderUsesMarkersAndBoundedBuffer()
    {
        var control = new TerminalPanel { MaxLines = 3, FollowTail = true };

        control.Append("one");
        control.Append("two", TerminalPanelChannel.StandardError);
        control.Append("three", TerminalPanelChannel.Command);
        control.Append("four", TerminalPanelChannel.System);

        Assert.That(control.Lines.Count, Is.EqualTo(3));
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedLine?.Text, Is.EqualTo("four"));

        var output = Render(control, 64, 4);
        Assert.That(output.Contains("one", StringComparison.Ordinal), Is.False);
        Assert.That(output.Contains("0001 ERR two", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("0002 CMD three", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("0003 SYS four", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void TerminalPanelKeyboardAndPointerSelectionRaisesSelectionChanged()
    {
        var control = new TerminalPanel { IsFocused = true, FollowTail = false };
        control.SetLines(
        [
            new TerminalPanelLine("first"),
            new TerminalPanelLine("second"),
            new TerminalPanelLine("third")
        ]);

        ListSelectionChangedEventArgs<TerminalPanelLine>? lastArgs = null;
        control.SelectionChanged += (_, args) => lastArgs = args;

        var keyChanged = control.Handle(new KeyPressed(Key.Down));
        var pointerChanged = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2),
            new Rect(0, 0, 40, 3));

        Assert.That(keyChanged, Is.True);
        Assert.That(pointerChanged, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedLine?.Text, Is.EqualTo("third"));
        Assert.That(lastArgs, Is.Not.Null);
        Assert.That(TestAssert.NotNull(lastArgs).PreviousIndex, Is.EqualTo(1));
        Assert.That(lastArgs.SelectedIndex, Is.EqualTo(2));
        Assert.That(lastArgs.PreviousItem?.Text, Is.EqualTo("second"));
        Assert.That(lastArgs.SelectedItem?.Text, Is.EqualTo("third"));
    }

    [Test]
    public void TerminalPanelDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new TerminalPanel();
        control.AppendRange(
        [
            new TerminalPanelLine("alpha"),
            new TerminalPanelLine("beta", TerminalPanelChannel.StandardError)
        ]);

        var first = Render(control, 40, 4);
        var second = Render(control, 40, 4);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(TerminalPanel control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
