using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class JumpListControlTests
{
    [Test]
    public void ControlsJumpListKeyboardSelectionRaisesSelectionChanged()
    {
        var control = new JumpList { Border = BorderStyle.None, IsFocused = true };
        control.SetItems(
        [
            new JumpListItem("a", "Alpha"),
            new JumpListItem("b", "Beta"),
            new JumpListItem("c", "Gamma")
        ]);

        ListSelectionChangedEventArgs<JumpListItem>? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var handled = control.Handle(new KeyPressed(Key.Down));

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem?.Id, Is.EqualTo("b"));
        Assert.That(args, Is.Not.Null);
        Assert.That(args!.PreviousIndex, Is.EqualTo(0));
        Assert.That(args.SelectedIndex, Is.EqualTo(1));
    }

    [Test]
    public void ControlsJumpListActivationRaisesActivatedFromKeyboardAndPointer()
    {
        var control = new JumpList { Border = BorderStyle.None, IsFocused = true };
        control.SetItems(
        [
            new JumpListItem("a", "Alpha"),
            new JumpListItem("b", "Beta")
        ]);

        JumpListActivatedEventArgs? activated = null;
        var activationCount = 0;
        control.Activated += (_, eventArgs) =>
        {
            activated = eventArgs;
            activationCount++;
        };

        var keyboardHandled = control.Handle(new KeyPressed(Key.Enter));
        var pointerHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 1),
            new Rect(0, 0, 40, 8));

        Assert.That(keyboardHandled, Is.True);
        Assert.That(pointerHandled, Is.True);
        Assert.That(activationCount, Is.EqualTo(2));
        Assert.That(activated, Is.Not.Null);
        Assert.That(activated!.SelectedItem.Id, Is.EqualTo(control.SelectedItem?.Id));
    }

    [Test]
    public void ControlsJumpListPointerSelectionUpdatesSelectedItem()
    {
        var control = new JumpList { Border = BorderStyle.None };
        control.SetItems(
        [
            new JumpListItem("a", "Alpha"),
            new JumpListItem("b", "Beta"),
            new JumpListItem("c", "Gamma")
        ]);

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 3, 2),
            new Rect(0, 0, 40, 8));

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedItem?.Id, Is.EqualTo("b"));
    }

    [Test]
    public void ControlsJumpListStyleHooksEmitAnsi()
    {
        var control = new JumpList
        {
            IsFocused = true,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 22, 33)),
            FocusedBorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(33, 44, 55)),
            TitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(44, 55, 66)),
            FocusedTitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(77, 88, 99)),
            ItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30)),
            SelectedItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(50, 60, 70)),
            HoveredItemStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(70, 80, 90))
        };
        control.SetItems(
        [
            new JumpListItem("a", "Alpha", true),
            new JumpListItem("b", "Beta", isRecent: true)
        ]);
        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2), new Rect(0, 0, 48, 8));

        var output = Render(control, 48, 8);

        Assert.That(output.Contains("Jump List *", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;33;44;55", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;77;88;99", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("48;2;70;80;90", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsJumpListDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new JumpList { Border = BorderStyle.None };
        control.SetItems(
        [
            new JumpListItem("a", "Alpha", true),
            new JumpListItem("b", "Beta", isRecent: true)
        ]);

        var first = Render(control, 48, 8);
        var second = Render(control, 48, 8);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(JumpList control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
