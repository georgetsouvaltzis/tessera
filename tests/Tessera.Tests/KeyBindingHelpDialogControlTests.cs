using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class KeyBindingHelpDialogControlTests
{
    [Test]
    public void KeyBindingHelpDialogRenderShowsGroupsAndRows()
    {
        var control = new KeyBindingHelpDialog
        {
            IsVisible = true,
            ShowGroups = true,
        };
        control.SetItems(
        [
            new KeyBindingItem("Ctrl+P", "Open palette", "Global", isGlobal: true),
            new KeyBindingItem("Ctrl+S", "Save file", "File"),
        ]);

        var output = Render(control, width: 80, height: 8);

        Assert.That(output.Contains("Keyboard Shortcuts", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("[Global]", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Ctrl+P", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Open palette", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void KeyBindingHelpDialogKeyboardNavigationAndEscapeWork()
    {
        var control = new KeyBindingHelpDialog
        {
            IsVisible = true,
            IsFocused = true,
        };
        control.SetItems(
        [
            new KeyBindingItem("Ctrl+P", "Open palette"),
            new KeyBindingItem("Ctrl+S", "Save file"),
        ]);

        var moved = control.Handle(new KeyPressed(Key.Down));
        var escaped = control.Handle(new KeyPressed(Key.Escape));

        Assert.That(moved, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(escaped, Is.True);
        Assert.That(control.IsVisible, Is.False);
    }

    [Test]
    public void KeyBindingHelpDialogPointerSelectsRowAndOutsideClickHides()
    {
        var control = new KeyBindingHelpDialog
        {
            IsVisible = true,
        };
        control.SetItems(
        [
            new KeyBindingItem("Ctrl+P", "Open palette"),
            new KeyBindingItem("Ctrl+S", "Save file"),
        ]);

        var selectHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 5, Y: 2),
            new Rect(0, 0, 72, 8));
        var hideHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 200, Y: 200),
            new Rect(0, 0, 72, 8));

        Assert.That(selectHandled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(hideHandled, Is.True);
        Assert.That(control.IsVisible, Is.False);
    }

    [Test]
    public void KeyBindingHelpDialogSelectedRowStyleEmitsAnsi()
    {
        var control = new KeyBindingHelpDialog
        {
            IsVisible = true,
            SelectedRowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(24, 133, 240)),
        };
        control.SetItems(
        [
            new KeyBindingItem("Ctrl+P", "Open palette"),
            new KeyBindingItem("Ctrl+S", "Save file"),
        ]);
        control.Select(1);

        var output = Render(control, width: 80, height: 8);

        Assert.That(output.Contains("38;2;24;133;240", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void KeyBindingHelpDialogDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new KeyBindingHelpDialog
        {
            IsVisible = true,
        };
        control.SetItems(
        [
            new KeyBindingItem("Ctrl+P", "Open palette", "Global"),
            new KeyBindingItem("Ctrl+S", "Save file", "File"),
        ]);
        var bounds = new Rect(0, 0, 72, 8);
        var firstCanvas = new Canvas(72, 8);
        var secondCanvas = new Canvas(72, 8);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(KeyBindingHelpDialog control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
