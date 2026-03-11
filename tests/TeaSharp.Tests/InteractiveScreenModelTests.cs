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

internal static class InteractiveScreenModelTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Application_InteractiveScreenModel_RoutesKeyWithoutManualEnsureScreen", InteractiveScreenModel_RoutesKeyWithoutManualEnsureScreen);
        yield return new TestCase("Application_InteractiveScreenModel_MasterDetailHelper_ComposesShell", InteractiveScreenModel_MasterDetailHelper_ComposesShell);
        yield return new TestCase("Application_InteractiveScreenModel_DashboardHelper_ComposesShell", InteractiveScreenModel_DashboardHelper_ComposesShell);
        yield return new TestCase("Application_InteractiveScreenModel_HandleTabNavigation_UsesFocusChainOrder", InteractiveScreenModel_HandleTabNavigation_UsesFocusChainOrder);
        yield return new TestCase("Application_InteractiveScreenModel_RestoreFocus_UsesCapturedSnapshot", InteractiveScreenModel_RestoreFocus_UsesCapturedSnapshot);
    }

    private static Task InteractiveScreenModel_RoutesKeyWithoutManualEnsureScreen()
    {
        var model = new ProbeScreenModel();

        var command = model.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(command is null, "Focused region activation should not require a command.");
        TestAssert.Equal(1, model.BuildCount, "Interactive screen base should lazily build the screen before key routing.");
        TestAssert.True(model.ButtonPresses == 1, "Focused region should receive the key through the base routing helper.");
        TestAssert.True(model.CurrentFocusedRegionKey == ProbeScreenModel.ButtonRegionId, "Preferred focus key should be applied through the shared shell.");
        return Task.CompletedTask;
    }

    private static Task InteractiveScreenModel_HandleTabNavigation_UsesFocusChainOrder()
    {
        var model = new ProbeScreenModel();

        model.Update(new KeyPressMsg(KeyCode.Tab));
        model.Update(new KeyPressMsg(KeyCode.Tab));

        TestAssert.True(model.CurrentFocusedRegionKey == ProbeScreenModel.ButtonRegionId, "Focus chain tab handling should cycle through the configured chain.");
        TestAssert.True(model.ButtonPresses == 0, "Tab navigation helper should not route the tab key into the focused component.");
        return Task.CompletedTask;
    }

    private static Task InteractiveScreenModel_MasterDetailHelper_ComposesShell()
    {
        var model = new ProbeMasterDetailScreenModel();

        _ = model.View();

        TestAssert.Equal(new Rect(0, 0, 40, 1), model.HeaderBounds, "Interactive model helper should expose header bounds through the master-detail scaffold.");
        TestAssert.Equal(new Rect(0, 1, 14, 10), model.MasterBounds, "Interactive model helper should expose master bounds through the master-detail scaffold.");
        TestAssert.Equal(new Rect(14, 1, 26, 10), model.DetailBounds, "Interactive model helper should expose detail bounds through the master-detail scaffold.");
        TestAssert.Equal(new Rect(0, 11, 40, 1), model.FooterBounds, "Interactive model helper should expose footer bounds through the master-detail scaffold.");
        return Task.CompletedTask;
    }

    private static Task InteractiveScreenModel_DashboardHelper_ComposesShell()
    {
        var model = new ProbeDashboardScreenModel();

        _ = model.View();

        TestAssert.Equal(new Rect(0, 0, 40, 1), model.HeaderBounds, "Interactive model helper should expose header bounds through the dashboard scaffold.");
        TestAssert.Equal(new Rect(0, 1, 12, 10), model.SidebarBounds, "Interactive model helper should expose sidebar bounds through the dashboard scaffold.");
        TestAssert.Equal(new Rect(12, 1, 28, 10), model.MainBounds, "Interactive model helper should expose main bounds through the dashboard scaffold.");
        TestAssert.Equal(new Rect(0, 11, 40, 1), model.FooterBounds, "Interactive model helper should expose footer bounds through the dashboard scaffold.");
        return Task.CompletedTask;
    }

    private static Task InteractiveScreenModel_RestoreFocus_UsesCapturedSnapshot()
    {
        var model = new ProbeScreenModel();

        model.FocusSecondaryRegion();
        model.FocusPrimaryRegion();
        var restored = model.RestoreSecondaryFocus();

        TestAssert.True(restored, "Captured focus snapshot should restore the prior focus target.");
        TestAssert.True(model.CurrentFocusedRegionKey == ProbeScreenModel.SecondaryRegionId, "Restored focus should target the captured region.");
        return Task.CompletedTask;
    }

    private sealed class ProbeScreenModel : InteractiveScreenModel
    {
        public static readonly ScreenRegionKey ButtonRegionId = new("probe.button");
        public static readonly ScreenRegionKey SecondaryRegionId = new("probe.secondary");

        private readonly ProbeButton _button = new();
        private readonly ProbeButton _secondary = new();
        private readonly ScreenFocusChain _focusChain;
        private ScreenFocusSnapshot _secondaryFocus;

        public int BuildCount { get; private set; }

        public int ButtonPresses => _button.PressCount;

        public ScreenRegionKey? CurrentFocusedRegionKey => FocusedRegionKey;

        public override Command? Init() => null;

        public override Command? Update(IMessage message)
        {
            return message is KeyPressMsg key
                ? RouteKey(key)
                : null;
        }

        public override View View()
        {
            var canvas = new Canvas(20, 4);
            RenderScreen(canvas);
            return TeaSharp.Core.Abstractions.View.From(canvas.Render());
        }

        protected override Rect GetBodyRect() => new(0, 0, 20, 4);

        protected override ScreenRegionKey? PreferredFocusRegionKey => ButtonRegionId;

        protected override void ComposeScreen(Rect bodyRect)
        {
            BuildCount++;
            var (left, right) = Layout.SplitVertical(bodyRect, Math.Max(10, bodyRect.Width / 2));
            Screen.AddComponent(ButtonRegionId, left, _button);
            Screen.AddComponent(SecondaryRegionId, right, _secondary);
        }

        public ProbeScreenModel()
        {
            _focusChain = CreateFocusChain(ButtonRegionId, SecondaryRegionId);
            InputRouter.AddScope(
                "probe.focused",
                InputScopeKind.FocusedRegion,
                () => FocusedRegionKey is not null,
                key => HandleTabNavigation(key, _focusChain)
                    ? InputRouteResult.HandledWithoutCommand
                    : Screen.Update(key)
                    ? InputRouteResult.HandledWithoutCommand
                    : InputRouteResult.NotHandled);
        }

        public void FocusPrimaryRegion()
        {
            SetFocus(ButtonRegionId);
        }

        public void FocusSecondaryRegion()
        {
            SetFocus(SecondaryRegionId);
            _secondaryFocus = CaptureFocus();
        }

        public bool RestoreSecondaryFocus()
        {
            return RestoreFocus(_secondaryFocus, _focusChain);
        }
    }

    private sealed class ProbeMasterDetailScreenModel : InteractiveScreenModel
    {
        private readonly ProbeButton _master = new();
        private readonly ProbeButton _detail = new();

        public Rect HeaderBounds { get; private set; }

        public Rect MasterBounds { get; private set; }

        public Rect DetailBounds { get; private set; }

        public Rect FooterBounds { get; private set; }

        public override Command? Init() => null;

        public override Command? Update(IMessage message) => null;

        public override View View()
        {
            var canvas = new Canvas(40, 12);
            RenderScreen(canvas);
            return TeaSharp.Core.Abstractions.View.From(canvas.Render());
        }

        protected override Rect GetBodyRect() => new(0, 0, 40, 12);

        protected override void ComposeScreen(Rect bodyRect)
        {
            var scaffold = MasterDetail(bodyRect, masterWidth: 14, headerHeight: 1, footerHeight: 1);
            HeaderBounds = scaffold.Header;
            MasterBounds = scaffold.Master;
            DetailBounds = scaffold.Detail;
            FooterBounds = scaffold.Footer;

            scaffold.AddHeader("probe.header", new StatusBarComponent(new StatusBarOptions(LeftText: "Header")));
            scaffold.AddMaster("probe.master", _master);
            scaffold.AddDetail("probe.detail", _detail);
            scaffold.AddFooter("probe.footer", new StatusBarComponent(new StatusBarOptions(LeftText: "Footer")));
        }
    }

    private sealed class ProbeDashboardScreenModel : InteractiveScreenModel
    {
        private readonly ProbeButton _sidebar = new();
        private readonly ProbeButton _main = new();

        public Rect HeaderBounds { get; private set; }

        public Rect SidebarBounds { get; private set; }

        public Rect MainBounds { get; private set; }

        public Rect FooterBounds { get; private set; }

        public override Command? Init() => null;

        public override Command? Update(IMessage message) => null;

        public override View View()
        {
            var canvas = new Canvas(40, 12);
            RenderScreen(canvas);
            return TeaSharp.Core.Abstractions.View.From(canvas.Render());
        }

        protected override Rect GetBodyRect() => new(0, 0, 40, 12);

        protected override void ComposeScreen(Rect bodyRect)
        {
            var scaffold = Dashboard(bodyRect, sidebarWidth: 12, headerHeight: 1, footerHeight: 1);
            HeaderBounds = scaffold.Header;
            SidebarBounds = scaffold.Sidebar;
            MainBounds = scaffold.Main;
            FooterBounds = scaffold.Footer;

            scaffold.AddHeader("probe.dashboard.header", new StatusBarComponent(new StatusBarOptions(LeftText: "Header")));
            scaffold.AddSidebar("probe.dashboard.sidebar", _sidebar);
            scaffold.AddMain("probe.dashboard.main", _main);
            scaffold.AddFooter("probe.dashboard.footer", new StatusBarComponent(new StatusBarOptions(LeftText: "Footer")));
        }
    }

    private sealed class ProbeButton : IStatefulComponent, IFocusableComponent
    {
        public bool Focused { get; set; }

        public int PressCount { get; private set; }

        public bool Update(IMessage message)
        {
            if (!Focused || message is not KeyPressMsg key || !key.Is(KeyCode.Enter))
            {
                return false;
            }

            PressCount++;
            return true;
        }

        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, Focused ? "focused" : "idle", rect.Width);
        }
    }
}
