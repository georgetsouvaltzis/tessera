using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ResizablePaneGroupControlTests
{
    [Test]
    public void ControlsResizablePaneGroupKeyboardSelectionRaisesSelectionChanged()
    {
        var control = new ResizablePaneGroup
        {
            Border = BorderStyle.None,
            IsFocused = true,
        };
        control.SetPanes(
        [
            new PaneSpec("left", title: "Left"),
            new PaneSpec("center", title: "Center"),
            new PaneSpec("right", title: "Right"),
        ]);

        ListSelectionChangedEventArgs<PaneSpec>? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var handled = control.Handle(new KeyPressed(Key.Right));

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedPaneIndex, Is.EqualTo(1));
        Assert.That(args, Is.Not.Null);
        Assert.That(args!.PreviousIndex, Is.EqualTo(0));
        Assert.That(args.SelectedIndex, Is.EqualTo(1));
        Assert.That(args.SelectedItem?.Id, Is.EqualTo("center"));
    }

    [Test]
    public void ControlsResizablePaneGroupCanonicalSelectionAliasesStayInSync()
    {
        var control = new ResizablePaneGroup();
        control.SetPanes(
        [
            new PaneSpec("left", title: "Left"),
            new PaneSpec("center", title: "Center"),
            new PaneSpec("right", title: "Right"),
        ]);

        Assert.That(control.SelectedIndex, Is.EqualTo(control.SelectedPaneIndex));
        Assert.That(control.SelectedItem, Is.SameAs(control.SelectedPane));

        var changed = control.SetSelectedIndex(2);

        Assert.That(changed, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedPaneIndex, Is.EqualTo(2));
        Assert.That(control.SelectedItem?.Id, Is.EqualTo("right"));
        Assert.That(control.SelectedPane?.Id, Is.EqualTo("right"));
    }

    [Test]
    public void ControlsResizablePaneGroupCanonicalTitleStyleAliasesStayInSync()
    {
        var control = new ResizablePaneGroup();
        var titleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(17, 27, 37));
        var focusedTitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(73, 63, 53));

        control.TitleStyle = titleStyle;
        control.FocusedTitleStyle = focusedTitleStyle;

        Assert.That(control.TitleStyleText, Is.EqualTo(titleStyle));
        Assert.That(control.FocusedTitleStyleText, Is.EqualTo(focusedTitleStyle));
        Assert.That(control.TitleStyle, Is.EqualTo(titleStyle));
        Assert.That(control.FocusedTitleStyle, Is.EqualTo(focusedTitleStyle));
    }

    [Test]
    public void ControlsResizablePaneGroupKeyboardResizeUpdatesRenderedPaneWidths()
    {
        var first = new SpyPaneControl("first");
        var second = new SpyPaneControl("second");
        var control = new ResizablePaneGroup
        {
            Border = BorderStyle.None,
            IsFocused = true,
        };
        control.SetPanes(
        [
            new PaneSpec("left", first),
            new PaneSpec("right", second),
        ]);

        RenderControl(control, width: 64, height: 8);
        var before = first.LastRenderBounds.Width;

        var handled = control.Handle(new KeyPressed(Key.Right, Modifiers: ModifierKeys.Ctrl));
        RenderControl(control, width: 64, height: 8);
        var after = first.LastRenderBounds.Width;

        Assert.That(handled, Is.True);
        Assert.That(after, Is.GreaterThan(before));
    }

    [Test]
    public void ControlsResizablePaneGroupPointerResizeAndSelectionWork()
    {
        var first = new SpyPaneControl("first");
        var second = new SpyPaneControl("second");
        var control = new ResizablePaneGroup
        {
            Border = BorderStyle.None,
        };
        control.SetPanes(
        [
            new PaneSpec("left", first),
            new PaneSpec("right", second),
        ]);

        var bounds = new Rect(0, 0, 60, 8);
        RenderControl(control, 60, 8);
        var before = first.LastRenderBounds.Width;

        var pressDivider = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 30, 2), bounds);
        var dragDivider = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 42, 2), bounds);
        var releaseDivider = control.Handle(new PointerInput(PointerEventKind.Release, PointerButton.Left, 42, 2), bounds);

        RenderControl(control, 60, 8);
        var after = first.LastRenderBounds.Width;

        var selectSecond = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 52, 2), bounds);

        Assert.That(pressDivider, Is.True);
        Assert.That(dragDivider, Is.True);
        Assert.That(releaseDivider, Is.True);
        Assert.That(after, Is.GreaterThan(before));
        Assert.That(selectSecond, Is.True);
        Assert.That(control.SelectedPaneIndex, Is.EqualTo(1));
    }

    [Test]
    public void ControlsResizablePaneGroupStyleHooksEmitAnsiAndFocusMarker()
    {
        var control = new ResizablePaneGroup
        {
            IsFocused = true,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(22, 33, 44)),
            FocusedBorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(44, 55, 66)),
            TitleStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(12, 23, 34)),
            FocusedTitleStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(90, 80, 70)),
            DividerStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30)),
        };
        control.SetPanes(
        [
            new PaneSpec("left", title: "Left"),
            new PaneSpec("right", title: "Right"),
        ]);

        var output = RenderControl(control, width: 56, height: 8);

        Assert.That(output.Contains("Resizable Pane Group *", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;44;55;66", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;90;80;70", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsResizablePaneGroupDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new ResizablePaneGroup
        {
            Border = BorderStyle.None,
        };
        control.SetPanes(
        [
            new PaneSpec("left", title: "Left"),
            new PaneSpec("right", title: "Right"),
            new PaneSpec("tail", title: "Tail"),
        ]);

        var first = RenderControl(control, width: 56, height: 8);
        var second = RenderControl(control, width: 56, height: 8);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string RenderControl(ResizablePaneGroup control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }

    private sealed class SpyPaneControl(string label) : Control
    {
        public Rect LastRenderBounds { get; private set; } = new Rect(0, 0, 0, 0);

        public override void Render(Canvas canvas, Rect rect)
        {
            LastRenderBounds = rect;
            if (rect.IsEmpty)
            {
                return;
            }

            canvas.WriteText(rect.X, rect.Y, label, rect.Width);
        }
    }
}
