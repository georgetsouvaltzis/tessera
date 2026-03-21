using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class FormControlTests
{
    [Test]
    public void FormRenderShowsFieldsMarkersAndHelperText()
    {
        var control = new Form
        {
            Border = BorderStyle.None,
        };
        control.SetFields(
        [
            new FormField("email", "Email", "user@example.com", "Used for login", isRequired: true),
            new FormField("name", "Name", "User"),
        ]);

        var output = Render(control, 80, 6);
        Assert.That(output.Contains("> Email*", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("user@example.com", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void FormKeyboardAndPointerSelectionRaisesEvent()
    {
        var control = new Form
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        control.SetFields(
        [
            new FormField("a", "A", "1"),
            new FormField("b", "B", "2", isDisabled: true),
            new FormField("c", "C", "3"),
        ]);

        ListSelectionChangedEventArgs<FormField>? lastArgs = null;
        control.SelectionChanged += (_, args) => lastArgs = args;

        var keyChanged = control.Handle(new KeyPressed(Key.Down));
        var pointerChanged = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 0),
            new Rect(0, 0, 40, 3));

        Assert.That(keyChanged, Is.True);
        Assert.That(pointerChanged, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
        Assert.That(control.SelectedField?.Name, Is.EqualTo("a"));
        Assert.That(lastArgs, Is.Not.Null);
        Assert.That(lastArgs!.PreviousIndex, Is.EqualTo(2));
        Assert.That(lastArgs.SelectedIndex, Is.EqualTo(0));
    }

    [Test]
    public void FormDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new Form
        {
            Border = BorderStyle.None,
        };
        control.SetFields(
        [
            new FormField("host", "Host", "localhost"),
            new FormField("port", "Port", "5432"),
        ]);

        var first = Render(control, 64, 4);
        var second = Render(control, 64, 4);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(Form control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
