using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class FieldSetControlTests
{
    [Test]
    public void FieldSetRenderShowsSectionMarkersAndSelectedRow()
    {
        var control = new FieldSet
        {
            Border = BorderStyle.None,
            SectionPrefix = "<",
            SectionSuffix = ">",
            Title = "Account"
        };
        control.SetItems(["Username", "Password"]);

        var output = Render(control, 48, 4);
        Assert.That(output.Contains("> Username", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Password", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void FieldSetKeyboardAndPointerSelectionRaisesEvent()
    {
        var control = new FieldSet { IsFocused = true, Border = BorderStyle.None };
        control.SetItems(["One", "Two", "Three"]);

        ListSelectionChangedEventArgs<string>? lastArgs = null;
        control.SelectionChanged += (_, args) => lastArgs = args;

        var keyChanged = control.Handle(new KeyPressed(Key.Down));
        var pointerChanged = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 2),
            new Rect(0, 0, 40, 3));

        Assert.That(keyChanged, Is.True);
        Assert.That(pointerChanged, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedItem, Is.EqualTo("Three"));
        Assert.That(lastArgs, Is.Not.Null);
        Assert.That(lastArgs!.PreviousIndex, Is.EqualTo(1));
        Assert.That(lastArgs.SelectedIndex, Is.EqualTo(2));
    }

    [Test]
    public void FieldSetDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new FieldSet { Border = BorderStyle.None };
        control.SetItems(["A", "B"]);

        var first = Render(control, 40, 3);
        var second = Render(control, 40, 3);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(FieldSet control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
