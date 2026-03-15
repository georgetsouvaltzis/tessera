using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Styles;
using TLayout = TeaSharp.Layout;

namespace TeaSharp.Tests;

internal static class ScreenComposerTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Components_ScreenComposer_FrameCreatesHeaderBodyFooterRegions", ScreenComposer_FrameCreatesHeaderBodyFooterRegions);
        yield return new TestCase("Components_ScreenComposer_ComposeSplitColumns_UsesDeterministicSlotBounds", ScreenComposer_ComposeSplitColumns_UsesDeterministicSlotBounds);
        yield return new TestCase("Components_ScreenComposer_ComposePanelRow_UsesGroupPaddingMarginAndBorder", ScreenComposer_ComposePanelRow_UsesGroupPaddingMarginAndBorder);
        yield return new TestCase("Components_ScreenComposer_ComposeCenterText_RendersCenteredStyledText", ScreenComposer_ComposeCenterText_RendersCenteredStyledText);
        yield return new TestCase("Components_ScreenComposer_MouseClickFocusesAndRoutesToRegisteredRegion", ScreenComposer_MouseClickFocusesAndRoutesToRegisteredRegion);
        yield return new TestCase("Components_ScreenComposer_FocusNextCyclesAcrossFocusableRegions", ScreenComposer_FocusNextCyclesAcrossFocusableRegions);
        yield return new TestCase("Components_ScreenComposer_FocusFirstTargetsFirstFocusableRegion", ScreenComposer_FocusFirstTargetsFirstFocusableRegion);
        yield return new TestCase("Components_ScreenComposer_FocusChainControlsNavigationOrder", ScreenComposer_FocusChainControlsNavigationOrder);
        yield return new TestCase("Components_ScreenComposer_RestoreFocusFallsBackToChain", ScreenComposer_RestoreFocusFallsBackToChain);
        yield return new TestCase("Components_ScreenComposer_CompleteFrameAppliesPreferredFocus", ScreenComposer_CompleteFrameAppliesPreferredFocus);
        yield return new TestCase("Components_ScreenComposer_PassiveToastOverlayDoesNotInterceptMouse", ScreenComposer_PassiveToastOverlayDoesNotInterceptMouse);
        yield return new TestCase("Components_ScreenComposer_ModalOverlayInterceptsUnderlyingMouse", ScreenComposer_ModalOverlayInterceptsUnderlyingMouse);
    }

    private static Task ScreenComposer_FrameCreatesHeaderBodyFooterRegions()
    {
        var composer = new ScreenComposer();
        var frame = composer.Frame(new Rect(0, 0, 80, 24), headerHeight: 2, footerHeight: 1);
        var (left, right) = frame.SplitBodyColumns(24);
        var (top, bottom) = frame.SplitBodyRows(10);

        TestAssert.Equal(new Rect(0, 0, 80, 2), frame.Header, "Frame should reserve requested header height.");
        TestAssert.Equal(new Rect(0, 2, 80, 21), frame.Body, "Frame should allocate body between header and footer.");
        TestAssert.Equal(new Rect(0, 23, 80, 1), frame.Footer, "Frame should reserve requested footer height.");
        TestAssert.Equal(24, left.Width, "Body column split should preserve requested left width when possible.");
        TestAssert.Equal(56, right.Width, "Body column split should preserve remaining width.");
        TestAssert.Equal(10, top.Height, "Body row split should preserve requested top height when possible.");
        TestAssert.Equal(11, bottom.Height, "Body row split should preserve remaining height.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_ComposeSplitColumns_UsesDeterministicSlotBounds()
    {
        var composer = new ScreenComposer();
        var leftComponent = new RenderLabelComponent("X");
        var rightComponent = new RenderLabelComponent("Y");

        composer.BeginFrame();
        composer.Compose(
            TLayout.Split.Columns(
                left: TLayout.Slot.Fixed(8, leftComponent, regionKey: "left"),
                right: TLayout.Slot.Fill(rightComponent, regionKey: "right")),
            new Rect(0, 0, 30, 5));
        composer.CompleteFrame();

        var left = composer.Regions.Single(region => region.Id == new ScreenRegionKey("left"));
        var right = composer.Regions.Single(region => region.Id == new ScreenRegionKey("right"));

        TestAssert.Equal(new Rect(0, 0, 8, 5), left.Bounds, "Split.Columns should keep the left slot at the fixed width.");
        TestAssert.Equal(new Rect(8, 0, 22, 5), right.Bounds, "Split.Columns should assign the remaining width to the fill slot.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_ComposePanelRow_UsesGroupPaddingMarginAndBorder()
    {
        var composer = new ScreenComposer();

        composer.BeginFrame();
        composer.Compose(
            TLayout.Split.Columns(
                left: TLayout.Slot.Auto(new RenderLabelComponent("X"), regionKey: "x", preferredWidth: 1, preferredHeight: 1),
                right: TLayout.Slot.Fill(
                    TLayout.Panel.Row(
                        [
                            TLayout.Slot.Auto(new RenderLabelComponent("Y"), regionKey: "y", preferredWidth: 1, preferredHeight: 1),
                            TLayout.Slot.Auto(new RenderLabelComponent("Z"), regionKey: "z", preferredWidth: 1, preferredHeight: 1),
                            TLayout.Slot.Auto(new RenderLabelComponent("J"), regionKey: "j", preferredWidth: 1, preferredHeight: 1),
                        ],
                        gap: 1,
                        title: "Actions",
                        border: BorderStyle.Rounded,
                        padding: Thickness.All(1),
                        margin: Thickness.Symmetric(horizontal: 2)))),
            new Rect(0, 0, 30, 5));
        composer.CompleteFrame();

        var x = composer.Regions.Single(region => region.Id == new ScreenRegionKey("x"));
        var y = composer.Regions.Single(region => region.Id == new ScreenRegionKey("y"));
        var z = composer.Regions.Single(region => region.Id == new ScreenRegionKey("z"));
        var j = composer.Regions.Single(region => region.Id == new ScreenRegionKey("j"));

        TestAssert.Equal(new Rect(0, 0, 1, 5), x.Bounds, "Auto slot should use the preferred width while stretching across the split height.");
        TestAssert.Equal(new Rect(5, 2, 1, 1), y.Bounds, "Panel row should account for outer margin, border, and padding before placing the first child.");
        TestAssert.Equal(new Rect(7, 2, 1, 1), z.Bounds, "Panel row should respect the requested gap between children.");
        TestAssert.Equal(new Rect(9, 2, 1, 1), j.Bounds, "Panel row should preserve stable item order inside the group.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_ComposeCenterText_RendersCenteredStyledText()
    {
        var composer = new ScreenComposer();
        var style = TeaStyle.Empty.WithBold();
        var expected = $"{new string(' ', 4)}{style.Render("Hello World")}";

        composer.BeginFrame();
        composer.Compose(TLayout.Center.Text("Hello World", style: style), new Rect(0, 0, 20, 5));
        composer.CompleteFrame();

        var canvas = new Canvas(20, 5, CanvasTextMode.GraphemeAware);
        composer.Render(canvas);
        var lines = canvas.Render().Split('\n');

        TestAssert.Equal(5, lines.Length, "Centered text should render within the provided canvas height.");
        TestAssert.True(lines[2].StartsWith(expected, StringComparison.Ordinal), "Center.Text should place styled text in the middle row without manual geometry math.");
        return Task.CompletedTask;
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
        TestAssert.True(button.IsFocused, "Clickable focusable region should become focused.");
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
        TestAssert.True(!first.IsFocused, "Previous region should lose focus.");
        TestAssert.True(second.IsFocused, "Next focusable region should gain focus.");
        TestAssert.True(composer.FocusedRegionKey == secondKey, "IsFocused region key should advance.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_FocusFirstTargetsFirstFocusableRegion()
    {
        var composer = new ScreenComposer();
        var first = new MouseProbeComponent();
        composer.BeginFrame();
        composer.AddRegion("static", new Rect(0, 0, 4, 1), static (_, _) => { });
        composer.AddComponent("first", new Rect(0, 1, 12, 4), first);
        composer.AddComponent("second", new Rect(12, 1, 12, 4), new MouseProbeComponent());
        composer.CompleteFrame();

        var changed = composer.FocusFirst();

        TestAssert.True(changed, "FocusFirst should focus the first available interactive region.");
        TestAssert.True(first.IsFocused, "FocusFirst should focus the first focusable region.");
        TestAssert.True(composer.FocusedRegionKey == new ScreenRegionKey("first"), "Composer should track the first focused region.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_FocusChainControlsNavigationOrder()
    {
        var composer = new ScreenComposer();
        var firstKey = new ScreenRegionKey("first");
        var secondKey = new ScreenRegionKey("second");
        var thirdKey = new ScreenRegionKey("third");
        var chain = new ScreenFocusChain([thirdKey, firstKey]);
        composer.BeginFrame();
        composer.AddComponent(firstKey, new Rect(0, 0, 8, 3), new MouseProbeComponent());
        composer.AddComponent(secondKey, new Rect(8, 0, 8, 3), new MouseProbeComponent());
        composer.AddComponent(thirdKey, new Rect(16, 0, 8, 3), new MouseProbeComponent());
        composer.CompleteFrame();

        var firstChanged = composer.FocusFirst(chain);
        var nextChanged = composer.FocusNext(chain);

        TestAssert.True(firstChanged, "FocusFirst(chain) should focus the first region in the provided chain.");
        TestAssert.True(nextChanged, "FocusNext(chain) should advance within the provided chain.");
        TestAssert.True(composer.FocusedRegionKey == firstKey, "Focus chain should skip regions not listed in the chain.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_RestoreFocusFallsBackToChain()
    {
        var composer = new ScreenComposer();
        var firstKey = new ScreenRegionKey("first");
        var secondKey = new ScreenRegionKey("second");
        var fallbackChain = new ScreenFocusChain([secondKey, firstKey]);
        composer.BeginFrame();
        composer.AddComponent(firstKey, new Rect(0, 0, 8, 3), new MouseProbeComponent());
        composer.AddComponent(secondKey, new Rect(8, 0, 8, 3), new MouseProbeComponent());
        composer.CompleteFrame(firstKey);
        var snapshot = composer.CaptureFocus();

        composer.BeginFrame();
        composer.AddComponent(secondKey, new Rect(8, 0, 8, 3), new MouseProbeComponent());
        composer.CompleteFrame();

        var changed = composer.RestoreFocus(snapshot, fallbackChain);

        TestAssert.True(changed, "RestoreFocus should fall back when the captured region no longer exists.");
        TestAssert.True(composer.FocusedRegionKey == secondKey, "Fallback focus chain should restore focus to the first available fallback region.");
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

        TestAssert.True(command.IsFocused, "Preferred region should be focused after frame completion.");
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
        TestAssert.True(modal.IsFocused, "Modal overlay should keep focus.");
        return Task.CompletedTask;
    }

    private sealed class MouseProbeComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
    {
        public bool IsFocused { get; set; }

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
            canvas.WriteText(rect.X, rect.Y, IsFocused ? "focused" : "idle", rect.Width);
        }
    }

    private sealed class RenderOnlyProbeComponent : ICanvasComponent
    {
        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, "toast", rect.Width);
        }
    }

    private sealed class RenderLabelComponent(string text) : ICanvasComponent
    {
        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, text, rect.Width);
        }
    }
}
