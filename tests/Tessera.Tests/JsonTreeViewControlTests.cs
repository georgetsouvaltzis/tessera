using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class JsonTreeViewControlTests
{
    [Test]
    public void ControlsJsonTreeViewSetJsonRendersHierarchy()
    {
        var control = new JsonTreeView { Border = BorderStyle.None };
        control.SetJson("""{"user":{"name":"anna","role":"admin"},"ok":true}""");

        var output = Render(control, 64, 6);

        Assert.That(output.Contains("> ▼ user: {...}", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("• name: \"anna\"", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("• ok: true", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsJsonTreeViewKeyboardNavigationExpandCollapseAndSelectionEvents()
    {
        var control = new JsonTreeView { IsFocused = true, Border = BorderStyle.None };
        control.SetJson("""{"user":{"name":"anna","role":"admin"},"ok":true}""");
        JsonTreeSelectionChangedEventArgs? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var down = control.Handle(new KeyPressed(Key.Down));
        var up = control.Handle(new KeyPressed(Key.Up));
        var collapse = control.Handle(new KeyPressed(Key.Enter));
        var collapsedOutput = Render(control, 64, 6);
        var expand = control.Handle(new KeyPressed(Key.Enter));
        var expandedOutput = Render(control, 64, 6);

        Assert.That(down, Is.True);
        Assert.That(up, Is.True);
        Assert.That(collapse, Is.True);
        Assert.That(expand, Is.True);
        Assert.That(args, Is.Not.Null);
        Assert.That(TestAssert.NotNull(args).CurrentIndex, Is.EqualTo(control.SelectedIndex));
        Assert.That(collapsedOutput.Contains("▶ user: {...}", StringComparison.Ordinal), Is.True);
        Assert.That(collapsedOutput.Contains("name: \"anna\"", StringComparison.Ordinal), Is.False);
        Assert.That(expandedOutput.Contains("▼ user: {...}", StringComparison.Ordinal), Is.True);
        Assert.That(expandedOutput.Contains("name: \"anna\"", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsJsonTreeViewPointerHoverAndClickSelectNode()
    {
        var control = new JsonTreeView { Border = BorderStyle.SingleLine };
        control.SetJson("""{"user":{"name":"anna","role":"admin"},"ok":true}""");
        var bounds = new Rect(0, 0, 64, 8);

        var move = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 3, 2), bounds);
        var click = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 3, 2), bounds);

        Assert.That(move, Is.True);
        Assert.That(click, Is.True);
        Assert.That(control.SelectedNode?.Key, Is.EqualTo("name"));
    }

    [Test]
    public void ControlsJsonTreeViewTrySetJsonReturnsFalseForInvalidJson()
    {
        var control = new JsonTreeView();

        var ok = control.TrySetJson("{invalid", out var error);

        Assert.That(ok, Is.False);
        Assert.That(string.IsNullOrWhiteSpace(error), Is.False);
    }

    [Test]
    public void ControlsJsonTreeViewStateStylesRenderAnsiAndDefaultRenderIsDeterministic()
    {
        var control = new JsonTreeView
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ContainerStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
            ValueStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
            SelectedRowStyle = TesseraStyle.Empty.WithBold(),
            FocusedSelectedRowStyle = TesseraStyle.Empty.WithUnderline(),
            HoveredRowStyle = TesseraStyle.Empty.WithItalic()
        };
        control.SetJson("""{"user":{"name":"anna"},"ok":true}""");
        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 1), new Rect(0, 0, 64, 5));

        var first = Render(control, 64, 5);
        var second = Render(control, 64, 5);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("38;2;10;11;12", StringComparison.Ordinal), Is.True);
        Assert.That(first.Contains("38;2;20;21;22", StringComparison.Ordinal), Is.True);
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.True);

        var plain = new JsonTreeView { Border = BorderStyle.None };
        plain.SetJson("""{"a":1}""");
        var plainOutput = Render(plain, 32, 3);
        Assert.That(plainOutput.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(JsonTreeView control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
