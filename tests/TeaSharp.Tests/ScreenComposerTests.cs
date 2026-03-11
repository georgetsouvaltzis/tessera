using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class ScreenComposerTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Components_ScreenComposer_MouseClickFocusesAndRoutesToRegisteredRegion", ScreenComposer_MouseClickFocusesAndRoutesToRegisteredRegion);
        yield return new TestCase("Components_ScreenComposer_FocusNextCyclesAcrossFocusableRegions", ScreenComposer_FocusNextCyclesAcrossFocusableRegions);
        yield return new TestCase("Components_ScreenComposer_CompleteFrameAppliesPreferredFocus", ScreenComposer_CompleteFrameAppliesPreferredFocus);
        yield return new TestCase("Components_ScreenComposer_PassiveToastOverlayDoesNotInterceptMouse", ScreenComposer_PassiveToastOverlayDoesNotInterceptMouse);
        yield return new TestCase("Components_ScreenComposer_ModalOverlayInterceptsUnderlyingMouse", ScreenComposer_ModalOverlayInterceptsUnderlyingMouse);
    }

    private static Task ScreenComposer_MouseClickFocusesAndRoutesToRegisteredRegion()
    {
        var composer = new ScreenComposer();
        var button = new MouseProbeComponent();
        var buttonKey = new ScreenRegionKey("button");
        composer.BeginFrame();
        composer.AddComponent(buttonKey, new Rect(0, 0, 12, 4), button);
        composer.CompleteFrame();

        var changed = composer.Update(new MouseClickMsg(MouseButton.Left, 2, 1));

        TestAssert.True(changed, "Mouse click should route to the registered region.");
        TestAssert.True(button.MouseEvents == 1, "Mouse region should receive the click.");
        TestAssert.True(button.Focused, "Clickable focusable region should become focused.");
        TestAssert.True(composer.FocusedRegionKey == buttonKey, "Composer should track focused region by typed key.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_FocusNextCyclesAcrossFocusableRegions()
    {
        var composer = new ScreenComposer();
        var first = new MouseProbeComponent();
        var second = new MouseProbeComponent();
        var firstKey = new ScreenRegionKey("first");
        var secondKey = new ScreenRegionKey("second");
        composer.BeginFrame();
        composer.AddRegion("static", new Rect(0, 0, 4, 1), static (_, _) => { });
        composer.AddComponent(firstKey, new Rect(0, 1, 12, 4), first);
        composer.AddComponent(secondKey, new Rect(12, 1, 12, 4), second);
        composer.CompleteFrame(firstKey);

        var changed = composer.FocusNext();

        TestAssert.True(changed, "FocusNext should advance focus across interactive regions.");
        TestAssert.True(!first.Focused, "Previous region should lose focus.");
        TestAssert.True(second.Focused, "Next focusable region should gain focus.");
        TestAssert.True(composer.FocusedRegionKey == secondKey, "Focused region key should advance.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_CompleteFrameAppliesPreferredFocus()
    {
        var composer = new ScreenComposer();
        var command = new MouseProbeComponent();
        var commandKey = new ScreenRegionKey("command");
        var focusCount = 0;
        composer.BeginFrame();
        composer.AddComponent("button", new Rect(0, 0, 12, 4), new MouseProbeComponent());
        composer.AddComponent(commandKey, new Rect(0, 5, 12, 4), command, onFocus: () => focusCount++);
        composer.CompleteFrame(commandKey);

        TestAssert.True(command.Focused, "Preferred region should be focused after frame completion.");
        TestAssert.True(composer.FocusedRegionKey == commandKey, "Preferred focus key should be preserved.");
        TestAssert.Equal(0, focusCount, "Frame completion should not fire focus callbacks just for snapshot rebuilds.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_PassiveToastOverlayDoesNotInterceptMouse()
    {
        var composer = new ScreenComposer();
        var button = new MouseProbeComponent();
        composer.BeginFrame();
        composer.AddComponent("button", new Rect(0, 0, 12, 4), button);
        composer.AddToastOverlay("toast", new Rect(0, 0, 12, 4), new RenderOnlyProbeComponent());
        composer.CompleteFrame("button");

        var changed = composer.Update(new MouseClickMsg(MouseButton.Left, 2, 1));

        TestAssert.True(changed, "Underlying interactive region should still receive mouse through passive overlay.");
        TestAssert.True(button.MouseEvents == 1, "Passive toast overlay should not steal the click.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_ModalOverlayInterceptsUnderlyingMouse()
    {
        var composer = new ScreenComposer();
        var button = new MouseProbeComponent();
        var modal = new MouseProbeComponent();
        composer.BeginFrame();
        composer.AddComponent("button", new Rect(0, 0, 12, 4), button);
        composer.AddModalComponent("modal", new Rect(0, 0, 12, 4), modal);
        composer.CompleteFrame("modal");

        var changed = composer.Update(new MouseClickMsg(MouseButton.Left, 2, 1));

        TestAssert.True(changed, "Modal overlay should handle clicks inside its region.");
        TestAssert.True(button.MouseEvents == 0, "Underlying region should not receive clicks under modal overlay.");
        TestAssert.True(modal.MouseEvents == 1, "Modal overlay should receive the click.");
        TestAssert.True(modal.Focused, "Modal overlay should keep focus.");
        return Task.CompletedTask;
    }

    private sealed class MouseProbeComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
    {
        public bool Focused { get; set; }

        public int MouseEvents { get; private set; }

        public bool Update(IMessage message)
        {
            return false;
        }

        public bool UpdateMouse(MouseMsg message, Rect bounds)
        {
            MouseEvents++;
            return true;
        }

        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, Focused ? "focused" : "idle", rect.Width);
        }
    }

    private sealed class RenderOnlyProbeComponent : ICanvasComponent
    {
        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, "toast", rect.Width);
        }
    }
}
