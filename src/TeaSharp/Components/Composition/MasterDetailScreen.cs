using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

/// <summary>
/// Represents a master-detail screen shell with optional header and footer regions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class MasterDetailScreen
{
    private readonly ScreenComposer _screen;
    private readonly List<ScreenRegionKey> _focusOrder = [];

    internal MasterDetailScreen(ScreenComposer screen, ScreenFrameLayout frame, Rect master, Rect detail)
    {
        _screen = screen;
        Frame = frame;
        Master = master;
        Detail = detail;
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
    /// Gets the master-pane bounds.
    /// </summary>
    public Rect Master { get; }

    /// <summary>
    /// Gets the detail-pane bounds.
    /// </summary>
    public Rect Detail { get; }

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
        Action? onFocus = null)
    {
        return AddComponent(id, Header, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);
    }

    /// <summary>
    /// Adds a component to the master region.
    /// </summary>
    public ScreenRegion AddMaster(
        ScreenRegionKey id,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null)
    {
        return AddComponent(id, Master, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);
    }

    /// <summary>
    /// Adds a component to the detail region.
    /// </summary>
    public ScreenRegion AddDetail(
        ScreenRegionKey id,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null)
    {
        return AddComponent(id, Detail, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);
    }

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
        Action? onFocus = null)
    {
        return AddComponent(id, Footer, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);
    }

    /// <summary>
    /// Creates a focus chain from the focusable regions added through this scaffold.
    /// </summary>
    public ScreenFocusChain CreateFocusChain()
    {
        return new ScreenFocusChain(_focusOrder);
    }

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
