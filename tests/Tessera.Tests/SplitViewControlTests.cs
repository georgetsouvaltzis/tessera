using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SplitViewControlTests
{
    [Test]
    public void Controls_SplitView_KeyboardFocusHandoff_ForwardsToActivePane()
    {
        var first = new SpyControl("left");
        var second = new SpyControl("right");
        var control = new SplitView
        {
            Border = BorderStyle.None,
            IsFocused = true,
            First = first,
            Second = second,
        };

        var firstHandled = control.Handle(new KeyPressed(Key.Down));
        var tabHandled = control.Handle(new KeyPressed(Key.Tab));
        var secondHandled = control.Handle(new KeyPressed(Key.Up));

        TestAssert.True(firstHandled, "First pane should handle key when active.");
        TestAssert.True(tabHandled, "Tab should switch active pane.");
        TestAssert.True(secondHandled, "Second pane should handle key after focus handoff.");
        TestAssert.Equal(1, control.ActivePaneIndex, "Second pane should become active.");
        TestAssert.Equal(1, first.KeyHandleCount, "First pane should receive one key message.");
        TestAssert.Equal(1, second.KeyHandleCount, "Second pane should receive one key message.");
        TestAssert.True(!first.IsFocused && second.IsFocused, "Focus should be applied to active pane only.");
    }

    [Test]
    public void Controls_SplitView_PointerDividerDrag_UpdatesRatio()
    {
        var control = new SplitView
        {
            Border = BorderStyle.None,
            ShowDivider = true,
            DividerThickness = 1,
            Ratio = 0.5d,
            Orientation = SplitViewOrientation.Horizontal,
            First = new Label { Text = "first", Border = BorderStyle.None },
            Second = new Label { Text = "second", Border = BorderStyle.None },
        };
        var bounds = new Rect(0, 0, 40, 8);

        var press = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 20, Y: 1), bounds);
        var move = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, X: 30, Y: 1), bounds);
        var release = control.Handle(new PointerInput(PointerEventKind.Release, PointerButton.Left, X: 30, Y: 1), bounds);

        TestAssert.True(press, "Press on divider should start drag.");
        TestAssert.True(move, "Drag motion should update ratio.");
        TestAssert.True(release, "Release should complete drag.");
        TestAssert.True(control.Ratio > 0.5d, "Dragging divider to the right should increase first pane ratio.");
    }

    [Test]
    public void Controls_SplitView_PointerPress_SelectsPaneAndForwardsBounds()
    {
        var first = new SpyControl("first");
        var second = new SpyControl("second");
        var control = new SplitView
        {
            Border = BorderStyle.None,
            Orientation = SplitViewOrientation.Horizontal,
            Ratio = 0.5d,
            First = first,
            Second = second,
        };
        var bounds = new Rect(0, 0, 40, 8);

        var changed = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 30, Y: 2), bounds);

        TestAssert.True(changed, "Pointer press in second pane should be handled.");
        TestAssert.Equal(1, control.ActivePaneIndex, "Second pane should become active.");
        TestAssert.Equal(0, first.PointerHandleCount, "First pane should not receive second-pane pointer event.");
        TestAssert.Equal(1, second.PointerHandleCount, "Second pane should receive pointer event.");
        TestAssert.True(second.LastBounds.X > bounds.X, "Forwarded bounds should match second pane layout.");
    }

    [Test]
    public void Controls_SplitView_FocusedStyles_EmitAnsi()
    {
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(91, 81, 71));
        var dividerStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 21, 31));
        var control = new SplitView
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            First = new Label { Text = "alpha", Border = BorderStyle.None },
            Second = new Label { Text = "beta", Border = BorderStyle.None },
            FocusedBorderStyleText = borderStyle,
            FocusedDividerStyle = dividerStyle,
        };

        var output = Render(control, 40, 8, CanvasTextMode.GraphemeAware);

        TestAssert.True(output.Contains(borderStyle.Render("┌"), StringComparison.Ordinal), "Focused border style should apply to border glyphs.");
        TestAssert.True(output.Contains(dividerStyle.Render("│"), StringComparison.Ordinal), "Focused divider style should apply to divider glyphs.");
    }

    [Test]
    public void Controls_SplitView_DefaultRender_IsDeterministicAndMonochrome()
    {
        var control = new SplitView
        {
            Border = BorderStyle.None,
            First = new Label { Text = "left pane", Border = BorderStyle.None },
            Second = new Label { Text = "right pane", Border = BorderStyle.None },
        };

        var first = Render(control, 44, 9);
        var second = Render(control, 44, 9);

        TestAssert.Equal(first, second, "SplitView should render deterministically for identical state.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default SplitView output should remain monochrome.");
    }

    private static string Render(SplitView control, int width, int height, CanvasTextMode mode = CanvasTextMode.Fast)
    {
        var canvas = new Canvas(width, height, mode);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }

    private sealed class SpyControl(string text) : Control
    {
        public int KeyHandleCount { get; private set; }
        public int PointerHandleCount { get; private set; }
        public Rect LastBounds { get; private set; } = new Rect(0, 0, 0, 0);

        public override bool Handle(Message message)
        {
            KeyHandleCount++;
            return true;
        }

        public override bool Handle(Message message, Rect bounds)
        {
            PointerHandleCount++;
            LastBounds = bounds;
            return true;
        }

        public override void Render(Canvas canvas, Rect rect)
        {
            if (rect.IsEmpty)
            {
                return;
            }

            canvas.WriteText(rect.X, rect.Y, text, rect.Width);
        }
    }
}
