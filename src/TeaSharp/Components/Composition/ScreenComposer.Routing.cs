using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components.Composition;

public sealed partial class ScreenComposer
{
    private void CompleteTypedFrame(ScreenRegionKey? preferredFocusRegionKey)
    {
        if (_regions.Count == 0)
        {
            FocusedRegionKey = null;
            return;
        }

        if (_requestedFocusRegionKey is { } requestedRegionKey && ApplyFocus(requestedRegionKey, invokeFocus: false))
        {
            return;
        }

        if (_frameFocusOverrideRequested && FocusedRegionKey is { } requestedFocusKey && ApplyFocus(requestedFocusKey, invokeFocus: false))
        {
            return;
        }

        if (preferredFocusRegionKey is { } preferredKey && ApplyFocus(preferredKey, invokeFocus: false))
        {
            return;
        }

        if (FocusedRegionKey is { } focusedKey && ApplyFocus(focusedKey, invokeFocus: false))
        {
            return;
        }

        var firstFocusable = FindFocusableIndex(startIndex: -1, step: 1);
        if (firstFocusable >= 0)
        {
            ApplyFocus(_regions[firstFocusable].Id, invokeFocus: false);
            return;
        }

        FocusedRegionKey = null;
    }

    private bool UpdateTyped(IMessage message)
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

    private bool FocusRelative(int step)
    {
        var startIndex = TryGetFocusedRegionIndex(out var focusedIndex)
            ? focusedIndex
            : step > 0 ? -1 : _regions.Count;
        var targetIndex = FindFocusableIndex(startIndex, step);
        return targetIndex >= 0 && ApplyFocus(_regions[targetIndex].Id, invokeFocus: true);
    }

    private bool FocusFirstTyped() =>
        FocusFirstTyped(regionKeys: null);

    private bool FocusFirstTyped(IReadOnlyList<ScreenRegionKey>? regionKeys)
    {
        if (regionKeys is null)
        {
            var firstFocusableIndex = FindFocusableIndex(startIndex: -1, step: 1);
            return firstFocusableIndex >= 0 && ApplyFocus(_regions[firstFocusableIndex].Id, invokeFocus: true);
        }

        var targetIndex = FindFocusableIndex(regionKeys, startPosition: -1, step: 1);
        return targetIndex >= 0 && ApplyFocus(regionKeys[targetIndex], invokeFocus: true);
    }

    private bool FocusRelativeTyped(IReadOnlyList<ScreenRegionKey>? regionKeys, int step)
    {
        if (regionKeys is null)
        {
            return FocusRelative(step);
        }

        var startPosition = FindFocusPosition(regionKeys);
        if (startPosition < 0)
        {
            startPosition = step > 0 ? -1 : regionKeys.Count;
        }

        var targetPosition = FindFocusableIndex(regionKeys, startPosition, step);
        return targetPosition >= 0 && ApplyFocus(regionKeys[targetPosition], invokeFocus: true);
    }

    private bool TryGetTypedBounds(ScreenRegionKey regionKey, out Rect bounds)
    {
        foreach (var region in _regions)
        {
            if (region.Id != regionKey)
            {
                continue;
            }

            bounds = region.Bounds;
            return true;
        }

        bounds = default;
        return false;
    }

    private bool ApplyFocus(ScreenRegionKey regionKey, bool invokeFocus)
    {
        var matched = false;
        foreach (var region in _regions)
        {
            var shouldFocus = region.Focusable && region.Id == regionKey;
            region.ApplyFocus(shouldFocus, invokeFocus && shouldFocus);
            matched |= shouldFocus;
        }

        if (matched)
        {
            FocusedRegionKey = regionKey;
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

    private int FindFocusableIndex(IReadOnlyList<ScreenRegionKey> regionKeys, int startPosition, int step)
    {
        if (regionKeys.Count == 0)
        {
            return -1;
        }

        for (var offset = 1; offset <= regionKeys.Count; offset++)
        {
            var position = startPosition + (offset * step);
            if (position < 0)
            {
                position += regionKeys.Count;
            }
            else if (position >= regionKeys.Count)
            {
                position -= regionKeys.Count;
            }

            if (TryGetFocusableRegionIndex(regionKeys[position], out _))
            {
                return position;
            }
        }

        return -1;
    }

    private int FindFocusPosition(IReadOnlyList<ScreenRegionKey> regionKeys)
    {
        if (FocusedRegionKey is not { } focusedRegionKey)
        {
            return -1;
        }

        for (var i = 0; i < regionKeys.Count; i++)
        {
            if (regionKeys[i] == focusedRegionKey)
            {
                return i;
            }
        }

        return -1;
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
        if (FocusedRegionKey is null)
        {
            return false;
        }

        for (var i = 0; i < _regions.Count; i++)
        {
            if (_regions[i].Id != FocusedRegionKey)
            {
                continue;
            }

            focusedIndex = i;
            return true;
        }

        return false;
    }

    private bool TryGetFocusableRegionIndex(ScreenRegionKey regionKey, out int regionIndex)
    {
        regionIndex = -1;
        for (var i = 0; i < _regions.Count; i++)
        {
            if (_regions[i].Id != regionKey || !_regions[i].Focusable)
            {
                continue;
            }

            regionIndex = i;
            return true;
        }

        return false;
    }
}
