using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public sealed class ScreenComposer
{
    private readonly List<ScreenRegion> _regions = [];

    public IReadOnlyList<ScreenRegion> Regions => _regions;

    public string? FocusedRegionId { get; private set; }

    public bool RouteMouseWheelToFocusedRegion { get; set; } = true;

    public void BeginFrame()
    {
        foreach (var region in _regions)
        {
            region.ApplyFocus(false, invokeFocus: false);
        }

        _regions.Clear();
    }

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
        var region = new ScreenRegion(id, bounds, render, update, updateMouse, focusable, focusOnClick, interceptsPointer, layer, focusTarget: null, onFocus);
        AddRegion(region);
        return region;
    }

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
        var stateful = component as IStatefulComponent;
        var mouseStateful = component as IMouseStatefulComponent;
        var focusTarget = component as IFocusableComponent;
        Func<IMessage, bool>? update = stateful is null
            ? null
            : message => stateful.Update(message);
        Func<MouseMsg, Rect, bool>? updateMouse = mouseStateful is null
            ? null
            : (message, bounds) => mouseStateful.UpdateMouse(message, bounds);
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
        return region;
    }

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
        return AddRegion(id, bounds, render, update, updateMouse, focusable, focusOnClick, interceptsPointer, (int)layer, onFocus);
    }

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
        return AddComponent(id, bounds, component, focusable, focusOnClick, interceptsPointer, (int)layer, onFocus);
    }

    public ScreenRegion AddModalComponent(
        string id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        Action? onFocus = null)
    {
        return AddOverlayComponent(id, bounds, component, focusable, focusOnClick: true, interceptsPointer: true, layer: ScreenLayer.Modal, onFocus);
    }

    public ScreenRegion AddPaletteComponent(
        string id,
        Rect bounds,
        ICanvasComponent component,
        bool? focusable = null,
        Action? onFocus = null)
    {
        return AddOverlayComponent(id, bounds, component, focusable, focusOnClick: true, interceptsPointer: true, layer: ScreenLayer.Palette, onFocus);
    }

    public ScreenRegion AddToastOverlay(
        string id,
        Rect bounds,
        ICanvasComponent component)
    {
        return AddOverlayComponent(id, bounds, component, focusable: false, focusOnClick: false, interceptsPointer: false, layer: ScreenLayer.Toast);
    }

    public void CompleteFrame(string? preferredFocusRegionId = null)
    {
        if (_regions.Count == 0)
        {
            FocusedRegionId = null;
            return;
        }

        if (!string.IsNullOrWhiteSpace(preferredFocusRegionId) && ApplyFocus(preferredFocusRegionId!, invokeFocus: false))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(FocusedRegionId) && ApplyFocus(FocusedRegionId!, invokeFocus: false))
        {
            return;
        }

        var firstFocusable = FindFocusableIndex(startIndex: -1, step: 1);
        if (firstFocusable >= 0)
        {
            ApplyFocus(_regions[firstFocusable].Id, invokeFocus: false);
            return;
        }

        FocusedRegionId = null;
    }

    public bool Update(IMessage message)
    {
        if (message is MouseMsg mouse)
        {
            return UpdateMouse(mouse);
        }

        if (!TryGetFocusedRegion(out var region))
        {
            return false;
        }

        return region.Update(message);
    }

    public bool UpdateMouse(MouseMsg message)
    {
        var changed = false;
        var targetIndex = FindTopMostRegion(message.X, message.Y);
        if (targetIndex < 0 && RouteMouseWheelToFocusedRegion && message is MouseWheelMsg && TryGetFocusedRegionIndex(out var focusedIndex))
        {
            targetIndex = focusedIndex;
        }

        if (targetIndex < 0)
        {
            return false;
        }

        var target = _regions[targetIndex];
        if (message is MouseClickMsg { Button: MouseButton.Left } && target.Focusable && target.FocusOnClick)
        {
            changed |= ApplyFocus(target.Id, invokeFocus: true);
        }

        changed |= target.UpdateMouse(message);
        return changed;
    }

    public bool SetFocus(string regionId)
    {
        return ApplyFocus(regionId, invokeFocus: true);
    }

    public bool FocusNext()
    {
        var startIndex = TryGetFocusedRegionIndex(out var focusedIndex)
            ? focusedIndex
            : -1;
        var targetIndex = FindFocusableIndex(startIndex, 1);
        return targetIndex >= 0 && ApplyFocus(_regions[targetIndex].Id, invokeFocus: true);
    }

    public bool FocusPrevious()
    {
        var startIndex = TryGetFocusedRegionIndex(out var focusedIndex)
            ? focusedIndex
            : _regions.Count;
        var targetIndex = FindFocusableIndex(startIndex, -1);
        return targetIndex >= 0 && ApplyFocus(_regions[targetIndex].Id, invokeFocus: true);
    }

    public bool TryGetBounds(string regionId, out Rect bounds)
    {
        foreach (var region in _regions)
        {
            if (!string.Equals(region.Id, regionId, StringComparison.Ordinal))
            {
                continue;
            }

            bounds = region.Bounds;
            return true;
        }

        bounds = default;
        return false;
    }

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
        if (string.Equals(region.Id, FocusedRegionId, StringComparison.Ordinal))
        {
            region.ApplyFocus(true, invokeFocus: false);
        }
        else
        {
            region.ApplyFocus(false, invokeFocus: false);
        }
    }

    private bool ApplyFocus(string regionId, bool invokeFocus)
    {
        var matched = false;
        foreach (var region in _regions)
        {
            var shouldFocus = region.Focusable && string.Equals(region.Id, regionId, StringComparison.Ordinal);
            region.ApplyFocus(shouldFocus, invokeFocus && shouldFocus);
            matched |= shouldFocus;
        }

        if (matched)
        {
            FocusedRegionId = regionId;
            return true;
        }

        return false;
    }

    private int FindFocusableIndex(int startIndex, int step)
    {
        if (_regions.Count == 0)
        {
            return -1;
        }

        for (var offset = 1; offset <= _regions.Count; offset++)
        {
            var index = startIndex + (offset * step);
            if (index < 0)
            {
                index += _regions.Count;
            }
            else if (index >= _regions.Count)
            {
                index -= _regions.Count;
            }

            if (_regions[index].Focusable)
            {
                return index;
            }
        }

        return -1;
    }

    private int FindTopMostRegion(int x, int y)
    {
        ScreenRegion? best = null;
        var bestIndex = -1;
        for (var i = 0; i < _regions.Count; i++)
        {
            var region = _regions[i];
            if (!region.Bounds.Contains(x, y) || !region.InterceptsPointer)
            {
                continue;
            }

            if (best is null || region.Layer >= best.Layer)
            {
                best = region;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private bool TryGetFocusedRegion(out ScreenRegion region)
    {
        region = default!;
        if (!TryGetFocusedRegionIndex(out var focusedIndex))
        {
            return false;
        }

        region = _regions[focusedIndex];
        return true;
    }

    private bool TryGetFocusedRegionIndex(out int focusedIndex)
    {
        focusedIndex = -1;
        if (string.IsNullOrWhiteSpace(FocusedRegionId))
        {
            return false;
        }

        for (var i = 0; i < _regions.Count; i++)
        {
            if (!string.Equals(_regions[i].Id, FocusedRegionId, StringComparison.Ordinal))
            {
                continue;
            }

            focusedIndex = i;
            return true;
        }

        return false;
    }
}
