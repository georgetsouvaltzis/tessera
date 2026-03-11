using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using System.Diagnostics.CodeAnalysis;
using TeaSharp.Layout;

namespace TeaSharp.Components.Composition;

public abstract class InteractiveScreenModel : IScreen
{
    protected ScreenComposer Screen { get; } = new();

    protected InputRouter InputRouter { get; } = new();

    protected ScreenRegionKey? FocusedRegionKey => Screen.FocusedRegionKey;

    protected bool HasScreen => Screen.Regions.Count > 0;

    protected abstract Rect GetBodyRect();

    protected abstract void ComposeScreen(Rect bodyRect);

    protected virtual ScreenRegionKey? PreferredFocusRegionKey => null;

    protected virtual bool CanBuildScreen => true;

    protected void EnsureScreen()
    {
        if (Screen.Regions.Count == 0 && CanBuildScreen)
        {
            RebuildScreen();
        }
    }

    protected void RebuildScreen()
    {
        if (!CanBuildScreen)
        {
            return;
        }

        Screen.BeginFrame();
        ComposeScreen(GetBodyRect());
        Screen.CompleteFrame(PreferredFocusRegionKey);
    }

    protected Effect? RouteKey(KeyPressMsg key)
    {
        EnsureScreen();
        var routed = InputRouter.Route(key);
        return routed.Handled ? routed.Effect : null;
    }

    protected bool RouteMouse(MouseMsg mouse)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.Update(mouse);
    }

    protected bool RouteFocusedMessage(IMessage message)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.Update(message);
    }

    protected void RenderScreen(Canvas canvas)
    {
        RebuildScreen();
        Screen.Render(canvas);
    }

    protected ScreenFrameLayout Frame(Rect bounds, int headerHeight = 0, int footerHeight = 0) =>
        Screen.Frame(bounds, headerHeight, footerHeight);

    /// <summary>
    /// Composes a public layout node into screen regions within the provided bounds.
    /// </summary>
    /// <param name="layout">The layout tree to compose.</param>
    /// <param name="bounds">The layout bounds.</param>
    protected void Compose(LayoutNode layout, Rect bounds) =>
        Screen.Compose(layout, bounds);

    /// <summary>
    /// Creates a master-detail screen scaffold with optional header and footer regions.
    /// </summary>
    /// <param name="bounds">The full screen bounds to partition.</param>
    /// <param name="masterWidth">Requested width for the master pane.</param>
    /// <param name="headerHeight">Header height in rows.</param>
    /// <param name="footerHeight">Footer height in rows.</param>
    /// <param name="minMasterWidth">Minimum width for the master pane.</param>
    /// <param name="minDetailWidth">Minimum width for the detail pane.</param>
    protected MasterDetailScreen MasterDetail(
        Rect bounds,
        int masterWidth,
        int headerHeight = 0,
        int footerHeight = 0,
        int minMasterWidth = 0,
        int minDetailWidth = 0) =>
        Screen.MasterDetail(bounds, masterWidth, headerHeight, footerHeight, minMasterWidth, minDetailWidth);

    /// <summary>
    /// Creates a dashboard-style screen scaffold with optional header and footer plus sidebar and main regions.
    /// </summary>
    /// <param name="bounds">The full screen bounds to partition.</param>
    /// <param name="sidebarWidth">Requested width for the sidebar pane.</param>
    /// <param name="headerHeight">Header height in rows.</param>
    /// <param name="footerHeight">Footer height in rows.</param>
    /// <param name="minSidebarWidth">Minimum width for the sidebar pane.</param>
    /// <param name="minMainWidth">Minimum width for the main pane.</param>
    protected DashboardScreen Dashboard(
        Rect bounds,
        int sidebarWidth,
        int headerHeight = 0,
        int footerHeight = 0,
        int minSidebarWidth = 0,
        int minMainWidth = 0) =>
        Screen.Dashboard(bounds, sidebarWidth, headerHeight, footerHeight, minSidebarWidth, minMainWidth);

    /// <summary>
    /// Creates a dialog workflow that handles modal composition plus focus capture and restoration.
    /// </summary>
    /// <param name="dialog">The dialog controlled by the workflow.</param>
    /// <param name="regionKey">The screen region key used for the modal dialog overlay.</param>
    /// <param name="fallbackFocusChain">Fallback focus order when the captured focus target no longer exists.</param>
    protected DialogWorkflow CreateDialogWorkflow(
        TeaSharp.Components.Prebuilt.DialogComponent dialog,
        ScreenRegionKey regionKey,
        ScreenFocusChain? fallbackFocusChain = null) =>
        Screen.CreateDialogWorkflow(dialog, regionKey, fallbackFocusChain);

    /// <summary>
    /// Creates a form-style screen scaffold with optional header and footer plus body and action regions.
    /// </summary>
    /// <param name="bounds">The full screen bounds to partition.</param>
    /// <param name="actionsHeight">Requested height for the action bar.</param>
    /// <param name="headerHeight">Header height in rows.</param>
    /// <param name="footerHeight">Footer height in rows.</param>
    /// <param name="minBodyHeight">Minimum height for the main form body.</param>
    /// <param name="minActionsHeight">Minimum height for the action bar.</param>
    protected FormScreen Form(
        Rect bounds,
        int actionsHeight,
        int headerHeight = 0,
        int footerHeight = 0,
        int minBodyHeight = 0,
        int minActionsHeight = 0) =>
        Screen.Form(bounds, actionsHeight, headerHeight, footerHeight, minBodyHeight, minActionsHeight);

    protected bool SetFocus(ScreenRegionKey regionKey)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.SetFocus(regionKey);
    }

    protected bool FocusNext()
    {
        EnsureScreen();
        return CanBuildScreen && Screen.FocusNext();
    }

    protected bool FocusPrevious()
    {
        EnsureScreen();
        return CanBuildScreen && Screen.FocusPrevious();
    }

    /// <summary>
    /// Creates an ordered focus chain for app-level focus helpers.
    /// </summary>
    /// <param name="regionKeys">The region keys in preferred focus order.</param>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Instance helper keeps focus-chain creation on the model API surface.")]
    protected ScreenFocusChain CreateFocusChain(params ScreenRegionKey[] regionKeys) =>
        new(regionKeys);

    /// <summary>
    /// Focuses the first available interactive region in the current screen.
    /// </summary>
    protected bool FocusFirstInteractive()
    {
        EnsureScreen();
        return CanBuildScreen && Screen.FocusFirst();
    }

    /// <summary>
    /// Focuses the first available region in the provided focus chain.
    /// </summary>
    /// <param name="focusChain">The ordered focus chain to use.</param>
    protected bool FocusFirst(ScreenFocusChain focusChain)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.FocusFirst(focusChain);
    }

    /// <summary>
    /// Advances focus through the provided focus chain.
    /// </summary>
    /// <param name="focusChain">The ordered focus chain to use.</param>
    protected bool FocusNext(ScreenFocusChain focusChain)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.FocusNext(focusChain);
    }

    /// <summary>
    /// Moves focus backward through the provided focus chain.
    /// </summary>
    /// <param name="focusChain">The ordered focus chain to use.</param>
    protected bool FocusPrevious(ScreenFocusChain focusChain)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.FocusPrevious(focusChain);
    }

    /// <summary>
    /// Handles `Tab` and `Shift+Tab` navigation using the current screen focus order.
    /// </summary>
    /// <param name="key">The key press to evaluate.</param>
    protected bool HandleTabNavigation(KeyPressMsg key)
    {
        if (key.Is(KeyCode.Tab, KeyModifiers.None))
        {
            return FocusNext();
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.Shift))
        {
            return FocusPrevious();
        }

        return false;
    }

    /// <summary>
    /// Handles `Tab` and `Shift+Tab` navigation using the provided focus chain.
    /// </summary>
    /// <param name="key">The key press to evaluate.</param>
    /// <param name="focusChain">The ordered focus chain to use.</param>
    protected bool HandleTabNavigation(KeyPressMsg key, ScreenFocusChain focusChain)
    {
        if (key.Is(KeyCode.Tab, KeyModifiers.None))
        {
            return FocusNext(focusChain);
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.Shift))
        {
            return FocusPrevious(focusChain);
        }

        return false;
    }

    /// <summary>
    /// Captures the currently focused region for later restoration.
    /// </summary>
    protected ScreenFocusSnapshot CaptureFocus()
    {
        EnsureScreen();
        return CanBuildScreen
            ? Screen.CaptureFocus()
            : default;
    }

    /// <summary>
    /// Restores a previously captured focus snapshot if possible.
    /// </summary>
    /// <param name="snapshot">The snapshot to restore.</param>
    protected bool RestoreFocus(ScreenFocusSnapshot snapshot)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.RestoreFocus(snapshot);
    }

    /// <summary>
    /// Restores a previously captured focus snapshot or falls back to the provided focus chain.
    /// </summary>
    /// <param name="snapshot">The snapshot to restore.</param>
    /// <param name="fallbackFocusChain">Fallback focus order when the snapshot can no longer be restored.</param>
    protected bool RestoreFocus(ScreenFocusSnapshot snapshot, ScreenFocusChain fallbackFocusChain)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.RestoreFocus(snapshot, fallbackFocusChain);
    }

    protected bool TryGetBounds(ScreenRegionKey regionKey, out Rect bounds)
    {
        EnsureScreen();
        if (!CanBuildScreen)
        {
            bounds = default;
            return false;
        }

        return Screen.TryGetBounds(regionKey, out bounds);
    }

    public abstract Effect? Init();

    public abstract Effect? Update(IMessage message);

    public abstract ScreenOutput Render();
}
