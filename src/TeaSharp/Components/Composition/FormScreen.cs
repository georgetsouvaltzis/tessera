using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

/// <summary>
/// Represents a form-style screen shell with optional header and footer plus body and action regions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class FormScreen
{
    private readonly ScreenComposer _screen;
    private readonly List<ScreenRegionKey> _focusOrder = [];

    internal FormScreen(ScreenComposer screen, ScreenFrameLayout frame, Rect body, Rect actions)
    {
        _screen = screen;
        Frame = frame;
        Body = body;
        Actions = actions;
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
    /// Gets the main form-body bounds.
    /// </summary>
    public Rect Body { get; }

    /// <summary>
    /// Gets the action-bar bounds.
    /// </summary>
    public Rect Actions { get; }

    /// <summary>
    /// Gets the footer bounds.
    /// </summary>
    public Rect Footer => Frame.Footer;

    /// <summary>
    /// Gets a value indicating whether the scaffold includes a header region.
    /// </summary>
    public bool HasHeader => Frame.HasHeader;

    /// <summary>
    /// Gets a value indicating whether the scaffold includes an actions region.
    /// </summary>
    public bool HasActions => !Actions.IsEmpty;

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
    /// Adds a component to the main body region.
    /// </summary>
    public ScreenRegion AddBody(
        ScreenRegionKey id,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        AddComponent(id, Body, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);

    /// <summary>
    /// Adds a component to the action-bar region.
    /// </summary>
    public ScreenRegion AddActions(
        ScreenRegionKey id,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        AddComponent(id, Actions, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);

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
