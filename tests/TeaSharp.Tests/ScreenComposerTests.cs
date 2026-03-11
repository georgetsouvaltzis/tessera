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
        yield return new TestCase("Components_ScreenComposer_FrameCreatesHeaderBodyFooterRegions", ScreenComposer_FrameCreatesHeaderBodyFooterRegions);
        yield return new TestCase("Components_ScreenComposer_MasterDetailCreatesExpectedRegions", ScreenComposer_MasterDetailCreatesExpectedRegions);
        yield return new TestCase("Components_MasterDetailScreen_CreateFocusChainTracksAddedRegions", MasterDetailScreen_CreateFocusChainTracksAddedRegions);
        yield return new TestCase("Components_ScreenComposer_DashboardCreatesExpectedRegions", ScreenComposer_DashboardCreatesExpectedRegions);
        yield return new TestCase("Components_DashboardScreen_CreateFocusChainTracksAddedRegions", DashboardScreen_CreateFocusChainTracksAddedRegions);
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

    private static Task ScreenComposer_MasterDetailCreatesExpectedRegions()
    {
        var composer = new ScreenComposer();
        var scaffold = composer.MasterDetail(new Rect(0, 0, 100, 30), masterWidth: 32, headerHeight: 2, footerHeight: 1);

        TestAssert.Equal(new Rect(0, 0, 100, 2), scaffold.Header, "Master-detail scaffold should expose header bounds.");
        TestAssert.Equal(new Rect(0, 2, 32, 27), scaffold.Master, "Master-detail scaffold should expose master-pane bounds.");
        TestAssert.Equal(new Rect(32, 2, 68, 27), scaffold.Detail, "Master-detail scaffold should expose detail-pane bounds.");
        TestAssert.Equal(new Rect(0, 29, 100, 1), scaffold.Footer, "Master-detail scaffold should expose footer bounds.");
        return Task.CompletedTask;
    }

    private static Task MasterDetailScreen_CreateFocusChainTracksAddedRegions()
    {
        var composer = new ScreenComposer();
        var scaffold = composer.MasterDetail(new Rect(0, 0, 100, 24), masterWidth: 28, headerHeight: 1, footerHeight: 1);
        var header = new MouseProbeComponent();
        var master = new MouseProbeComponent();
        var detail = new MouseProbeComponent();
        var footer = new MouseProbeComponent();
        var headerKey = new ScreenRegionKey("header");
        var masterKey = new ScreenRegionKey("master");
        var detailKey = new ScreenRegionKey("detail");
        var footerKey = new ScreenRegionKey("footer");

        composer.BeginFrame();
        scaffold.AddHeader(headerKey, header);
        scaffold.AddMaster(masterKey, master);
        scaffold.AddDetail(detailKey, detail);
        scaffold.AddFooter(footerKey, footer);
        composer.CompleteFrame();
        var focusChain = scaffold.CreateFocusChain();

        var firstChanged = composer.FocusFirst(focusChain);
        var nextChanged = composer.FocusNext(focusChain);

        TestAssert.True(firstChanged, "Scaffold focus chain should focus the first added focusable region.");
        TestAssert.True(nextChanged, "Scaffold focus chain should advance through tracked regions.");
        TestAssert.True(composer.FocusedRegionKey == masterKey, "Focus chain should preserve header-master-detail-footer order.");
        return Task.CompletedTask;
    }

    private static Task ScreenComposer_DashboardCreatesExpectedRegions()
    {
        var composer = new ScreenComposer();
        var scaffold = composer.Dashboard(new Rect(0, 0, 100, 30), sidebarWidth: 24, headerHeight: 2, footerHeight: 1);

        TestAssert.Equal(new Rect(0, 0, 100, 2), scaffold.Header, "Dashboard scaffold should expose header bounds.");
        TestAssert.Equal(new Rect(0, 2, 24, 27), scaffold.Sidebar, "Dashboard scaffold should expose sidebar bounds.");
        TestAssert.Equal(new Rect(24, 2, 76, 27), scaffold.Main, "Dashboard scaffold should expose main bounds.");
        TestAssert.Equal(new Rect(0, 29, 100, 1), scaffold.Footer, "Dashboard scaffold should expose footer bounds.");
        return Task.CompletedTask;
    }

    private static Task DashboardScreen_CreateFocusChainTracksAddedRegions()
    {
        var composer = new ScreenComposer();
        var scaffold = composer.Dashboard(new Rect(0, 0, 100, 24), sidebarWidth: 22, headerHeight: 1, footerHeight: 1);
        var header = new MouseProbeComponent();
        var sidebar = new MouseProbeComponent();
        var main = new MouseProbeComponent();
        var footer = new MouseProbeComponent();
        var headerKey = new ScreenRegionKey("header");
        var sidebarKey = new ScreenRegionKey("sidebar");
        var mainKey = new ScreenRegionKey("main");
        var footerKey = new ScreenRegionKey("footer");

        composer.BeginFrame();
        scaffold.AddHeader(headerKey, header);
        scaffold.AddSidebar(sidebarKey, sidebar);
        scaffold.AddMain(mainKey, main);
        scaffold.AddFooter(footerKey, footer);
        composer.CompleteFrame();
        var focusChain = scaffold.CreateFocusChain();

        var firstChanged = composer.FocusFirst(focusChain);
        var nextChanged = composer.FocusNext(focusChain);

        TestAssert.True(firstChanged, "Dashboard focus chain should focus the first added focusable region.");
        TestAssert.True(nextChanged, "Dashboard focus chain should advance through tracked regions.");
        TestAssert.True(composer.FocusedRegionKey == sidebarKey, "Focus chain should preserve header-sidebar-main-footer order.");
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
        TestAssert.True(first.Focused, "FocusFirst should focus the first focusable region.");
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
