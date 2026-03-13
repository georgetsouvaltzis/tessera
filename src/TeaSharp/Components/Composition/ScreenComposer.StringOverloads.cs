using TeaSharp.Components.Primitives;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components.Composition;

internal sealed partial class ScreenComposer
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ScreenRegion AddRegion(
        string id,
        Rect bounds,
        Action<Canvas, Rect> render,
        Func<IMessage, bool>? update = null,
        Func<MouseMsg, Rect, bool>? updateMouse = null,
        bool focusable = false,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null)
    {
        return AddRegion(new ScreenRegionKey(id), bounds, render, update, updateMouse, focusable, focusOnClick, interceptsPointer, layer, onFocus);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ScreenRegion AddComponent(
        string id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null)
    {
        return AddComponent(new ScreenRegionKey(id), bounds, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ScreenRegion AddOverlayRegion(
        string id,
        Rect bounds,
        Action<Canvas, Rect> render,
        Func<IMessage, bool>? update = null,
        Func<MouseMsg, Rect, bool>? updateMouse = null,
        bool focusable = false,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        ScreenLayer layer = ScreenLayer.Overlay,
        Action? onFocus = null)
    {
        return AddOverlayRegion(new ScreenRegionKey(id), bounds, render, update, updateMouse, focusable, focusOnClick, interceptsPointer, layer, onFocus);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ScreenRegion AddOverlayComponent(
        string id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        ScreenLayer layer = ScreenLayer.Overlay,
        Action? onFocus = null)
    {
        return AddOverlayComponent(new ScreenRegionKey(id), bounds, component, focusable, focusOnClick, interceptsPointer, layer, onFocus);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ScreenRegion AddModalComponent(
        string id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        Action? onFocus = null)
    {
        return AddModalComponent(new ScreenRegionKey(id), bounds, component, focusable, onFocus);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ScreenRegion AddPaletteComponent(
        string id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        Action? onFocus = null)
    {
        return AddPaletteComponent(new ScreenRegionKey(id), bounds, component, focusable, onFocus);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ScreenRegion AddToastOverlay(
        string id,
        Rect bounds,
        ICanvasComponent component)
    {
        return AddToastOverlay(new ScreenRegionKey(id), bounds, component);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool SetFocus(string regionId)
    {
        return SetFocus(new ScreenRegionKey(regionId));
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryGetBounds(string regionId, out Rect bounds)
    {
        return TryGetBounds(new ScreenRegionKey(regionId), out bounds);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void CompleteFrame(string? preferredFocusRegionId)
    {
        CompleteFrame(string.IsNullOrWhiteSpace(preferredFocusRegionId)
            ? (ScreenRegionKey?)null
            : new ScreenRegionKey(preferredFocusRegionId));
    }
}
