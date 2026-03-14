using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using TeaSharp.Components.Composition.Internal;
using TeaSharp.Layout;

namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed partial class ScreenComposer
{
    private readonly List<ScreenRegion> _regions = [];
    private bool _frameFocusOverrideRequested;
    private ScreenRegionKey? _requestedFocusRegionKey;
    private long _requestedFocusOrder;

    public IReadOnlyList<ScreenRegion> Regions => _regions;

    public ScreenRegionKey? FocusedRegionKey { get; private set; }

    public string? FocusedRegionId => FocusedRegionKey?.Value;

    public bool RouteMouseWheelToFocusedRegion { get; set; } = true;

    /// <summary>
    /// Creates a common screen shell split into header, body, and footer regions.
    /// </summary>
    /// <param name="bounds">The full screen bounds to partition.</param>
    /// <param name="headerHeight">Header height in rows.</param>
    /// <param name="footerHeight">Footer height in rows.</param>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Instance call keeps the fluent screen-composition entrypoint on ScreenComposer.")]
    public ScreenFrameLayout Frame(Rect bounds, int headerHeight = 0, int footerHeight = 0)
    {
        var totalHeight = Math.Max(0, bounds.Height);
        var safeHeaderHeight = Math.Clamp(headerHeight, 0, totalHeight);
        var remainingAfterHeader = Math.Max(0, totalHeight - safeHeaderHeight);
        var safeFooterHeight = Math.Clamp(footerHeight, 0, remainingAfterHeader);
        var headerKey = new ScreenRegionKey("__frame.header");
        var bodyKey = new ScreenRegionKey("__frame.body");
        var footerKey = new ScreenRegionKey("__frame.footer");
        var resolved = LayoutBoundsResolver.Resolve(
            Dock.Layout(
                top: safeHeaderHeight == 0
                    ? null
                    : Slot.Fixed(safeHeaderHeight, LayoutBoundsResolver.Placeholder(), regionKey: headerKey, preferredHeight: safeHeaderHeight, focusable: false, focusOnClick: false, interceptsPointer: false),
                bottom: safeFooterHeight == 0
                    ? null
                    : Slot.Fixed(safeFooterHeight, LayoutBoundsResolver.Placeholder(), regionKey: footerKey, preferredHeight: safeFooterHeight, focusable: false, focusOnClick: false, interceptsPointer: false),
                fill: Slot.Fill(LayoutBoundsResolver.Placeholder(), regionKey: bodyKey, focusable: false, focusOnClick: false, interceptsPointer: false)),
            bounds);

        var header = resolved.TryGetValue(headerKey, out var headerBounds)
            ? headerBounds
            : new Rect(bounds.X, bounds.Y, bounds.Width, 0);
        var body = resolved.TryGetValue(bodyKey, out var bodyBounds)
            ? bodyBounds
            : new Rect(bounds.X, bounds.Y + safeHeaderHeight, bounds.Width, Math.Max(0, totalHeight - safeHeaderHeight - safeFooterHeight));
        var footer = resolved.TryGetValue(footerKey, out var footerBounds)
            ? footerBounds
            : new Rect(bounds.X, body.Bottom, bounds.Width, 0);

        return new ScreenFrameLayout(bounds, header, body, footer);
    }

    /// <summary>
    /// Composes a public layout node into screen regions within the provided bounds.
    /// </summary>
    /// <param name="layout">The layout tree to compose.</param>
    /// <param name="bounds">The layout bounds.</param>
    public void Compose(LayoutNode layout, Rect bounds)
    {
        ArgumentNullException.ThrowIfNull(layout);
        layout.Compose(this, bounds, "root");
    }

    public void BeginFrame()
    {
        _frameFocusOverrideRequested = false;
        _requestedFocusRegionKey = null;
        _requestedFocusOrder = 0;
        foreach (var region in _regions)
        {
            region.ApplyFocus(false, invokeFocus: false);
        }

        _regions.Clear();
    }

    public ScreenRegion AddRegion(
        ScreenRegionKey id,
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
        var region = new ScreenRegion(id, bounds, render, update, updateMouse, focusable, focusOnClick, interceptsPointer, layer, focusTarget: null, onFocus);
        AddRegion(region);
        return region;
    }

    public ScreenRegion AddComponent(
        ScreenRegionKey id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null)
    {
        var stateful = component as IStatefulComponent;
        var mouseStateful = component as IMouseStatefulComponent;
        var focusTarget = component as IFocusableComponent;
        Func<IMessage, bool>? update = stateful is null
            ? null
            : message => stateful.Update(message);
        Func<MouseMsg, Rect, bool>? updateMouse = mouseStateful is null
            ? null
            : (message, bounds) => mouseStateful.UpdateMouse(message, bounds);
        var requestedFocus = focusTarget?.IsFocused == true;
        var requestOrder = 0L;
        if (component is TeaSharp.Controls.IFocusRequestSource focusRequestSource
            && focusRequestSource.TryConsumeFocusRequest(out var explicitRequestOrder))
        {
            requestedFocus = true;
            requestOrder = explicitRequestOrder;
        }
        var region = new ScreenRegion(
            id,
            bounds,
            component.Render,
            update,
            updateMouse,
            focusable ?? focusTarget is not null,
            focusOnClick,
            interceptsPointer,
            layer,
            focusTarget,
            onFocus);
        AddRegion(region);

        if (requestedFocus)
        {
            if (requestOrder > 0)
            {
                if (_requestedFocusRegionKey is null || requestOrder >= _requestedFocusOrder)
                {
                    _requestedFocusRegionKey = id;
                    _requestedFocusOrder = requestOrder;
                    _frameFocusOverrideRequested = true;
                }
            }
            else
            {
                ApplyFocus(id, invokeFocus: false);
                _frameFocusOverrideRequested = true;
            }
        }

        return region;
    }

    public ScreenRegion AddOverlayRegion(
        ScreenRegionKey id,
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
        return AddRegion(id, bounds, render, update, updateMouse, focusable, focusOnClick, interceptsPointer, (int)layer, onFocus);
    }

    public ScreenRegion AddOverlayComponent(
        ScreenRegionKey id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        ScreenLayer layer = ScreenLayer.Overlay,
        Action? onFocus = null)
    {
        return AddComponent(id, bounds, component, focusable, focusOnClick, interceptsPointer, (int)layer, onFocus);
    }

    public ScreenRegion AddModalComponent(
        ScreenRegionKey id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        Action? onFocus = null)
    {
        return AddOverlayComponent(id, bounds, component, focusable, focusOnClick: true, interceptsPointer: true, layer: ScreenLayer.Modal, onFocus);
    }

    public ScreenRegion AddPaletteComponent(
        ScreenRegionKey id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        Action? onFocus = null)
    {
        return AddOverlayComponent(id, bounds, component, focusable, focusOnClick: true, interceptsPointer: true, layer: ScreenLayer.Palette, onFocus);
    }

    public ScreenRegion AddToastOverlay(
        ScreenRegionKey id,
        Rect bounds,
        ICanvasComponent component)
    {
        return AddOverlayComponent(id, bounds, component, focusable: false, focusOnClick: false, interceptsPointer: false, layer: ScreenLayer.Toast);
    }

    public void CompleteFrame(ScreenRegionKey? preferredFocusRegionKey = null) =>
        CompleteTypedFrame(preferredFocusRegionKey);

    public bool Update(IMessage message) =>
        UpdateTyped(message);

    public bool SetFocus(ScreenRegionKey regionKey)
    {
        var changed = ApplyFocus(regionKey, invokeFocus: true);
        _frameFocusOverrideRequested |= changed;
        return changed;
    }

    public bool FocusNext() =>
        FocusRelative(1);

    public bool FocusPrevious() =>
        FocusRelative(-1);

    /// <summary>
    /// Focuses the first focusable region in the composed screen.
    /// </summary>
    public bool FocusFirst() =>
        FocusFirstTyped();

    /// <summary>
    /// Focuses the first available region in the provided focus chain.
    /// </summary>
    /// <param name="focusChain">The ordered focus chain to use.</param>
    public bool FocusFirst(ScreenFocusChain focusChain) =>
        FocusFirstTyped(focusChain?.RegionKeys);

    /// <summary>
    /// Advances focus using the provided focus chain.
    /// </summary>
    /// <param name="focusChain">The ordered focus chain to use.</param>
    public bool FocusNext(ScreenFocusChain focusChain) =>
        FocusRelativeTyped(focusChain?.RegionKeys, 1);

    /// <summary>
    /// Moves focus backward using the provided focus chain.
    /// </summary>
    /// <param name="focusChain">The ordered focus chain to use.</param>
    public bool FocusPrevious(ScreenFocusChain focusChain) =>
        FocusRelativeTyped(focusChain?.RegionKeys, -1);

    /// <summary>
    /// Captures the currently focused region for later restoration.
    /// </summary>
    public ScreenFocusSnapshot CaptureFocus() =>
        new(FocusedRegionKey);

    /// <summary>
    /// Restores a previously captured focus snapshot if the region still exists.
    /// </summary>
    /// <param name="snapshot">The snapshot to restore.</param>
    public bool RestoreFocus(ScreenFocusSnapshot snapshot) =>
        snapshot.RegionKey is { } regionKey && ApplyFocus(regionKey, invokeFocus: true);

    /// <summary>
    /// Restores a previously captured focus snapshot, or falls back to the provided chain.
    /// </summary>
    /// <param name="snapshot">The snapshot to restore.</param>
    /// <param name="fallbackFocusChain">Fallback focus order when the snapshot can no longer be restored.</param>
    public bool RestoreFocus(ScreenFocusSnapshot snapshot, ScreenFocusChain fallbackFocusChain)
    {
        if (RestoreFocus(snapshot))
        {
            return true;
        }

        return FocusFirst(fallbackFocusChain);
    }

    public bool TryGetBounds(ScreenRegionKey regionKey, out Rect bounds) =>
        TryGetTypedBounds(regionKey, out bounds);

    public void Render(Canvas canvas)
    {
        foreach (var region in _regions.OrderBy(region => region.Layer))
        {
            region.Render(canvas);
        }
    }

    private void AddRegion(ScreenRegion region)
    {
        _regions.Add(region);
        if (region.Id == FocusedRegionKey)
        {
            region.ApplyFocus(true, invokeFocus: false);
        }
        else
        {
            region.ApplyFocus(false, invokeFocus: false);
        }
    }

}
