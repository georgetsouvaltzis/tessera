using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Components.Composition;

/// <summary>
/// Represents a dashboard-style screen shell with optional header and footer plus sidebar and main regions.
/// </summary>
public sealed class DashboardScreen
{
    private readonly ScreenComposer _screen;
    private readonly List<ScreenRegionKey> _focusOrder = [];

    internal DashboardScreen(ScreenComposer screen, ScreenFrameLayout frame, Rect sidebar, Rect main)
    {
        _screen = screen;
        Frame = frame;
        Sidebar = sidebar;
        Main = main;
    }

    /// <summary>
    /// Gets the outer frame for the scaffold.
    /// </summary>
    public ScreenFrameLayout Frame { get; }

    /// <summary>
    /// Gets the header bounds.
    /// </summary>
    public Rect Header => Frame.Header;

    /// <summary>
    /// Gets the sidebar bounds.
    /// </summary>
    public Rect Sidebar { get; }

    /// <summary>
    /// Gets the main-content bounds.
    /// </summary>
    public Rect Main { get; }

    /// <summary>
    /// Gets the footer bounds.
    /// </summary>
    public Rect Footer => Frame.Footer;

    /// <summary>
    /// Gets a value indicating whether the scaffold includes a header region.
    /// </summary>
    public bool HasHeader => Frame.HasHeader;

    /// <summary>
    /// Gets a value indicating whether the scaffold includes a footer region.
    /// </summary>
    public bool HasFooter => Frame.HasFooter;

    /// <summary>
    /// Adds a component to the header region.
    /// </summary>
    public ScreenRegion AddHeader(
        ScreenRegionKey id,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        AddComponent(id, Header, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);

    /// <summary>
    /// Adds a component to the sidebar region.
    /// </summary>
    public ScreenRegion AddSidebar(
        ScreenRegionKey id,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        AddComponent(id, Sidebar, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);

    /// <summary>
    /// Adds a component to the main region.
    /// </summary>
    public ScreenRegion AddMain(
        ScreenRegionKey id,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        AddComponent(id, Main, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);

    /// <summary>
    /// Adds a component to the footer region.
    /// </summary>
    public ScreenRegion AddFooter(
        ScreenRegionKey id,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        AddComponent(id, Footer, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);

    /// <summary>
    /// Creates a focus chain from the focusable regions added through this scaffold.
    /// </summary>
    public ScreenFocusChain CreateFocusChain() => new(_focusOrder);

    private ScreenRegion AddComponent(
        ScreenRegionKey id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable,
        bool focusOnClick,
        bool interceptsPointer,
        int layer,
        Action? onFocus)
    {
        var region = _screen.AddComponent(id, bounds, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);
        if (region.Focusable && !_focusOrder.Contains(id))
        {
            _focusOrder.Add(id);
        }

        return region;
    }
}
