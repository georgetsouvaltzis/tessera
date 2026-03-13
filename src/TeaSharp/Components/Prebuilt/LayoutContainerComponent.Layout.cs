using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using UiLayout = TeaSharp.Components.UiKit.Layout;

namespace TeaSharp.Components.Prebuilt;

internal sealed partial class LayoutContainerComponent
{
    private List<ComponentSlot> CreateSlots(List<Rect>? rects = null)
    {
        var childRects = rects ?? CreatePlaceholderRects();
        var count = Math.Min(_children.Count, childRects.Count);
        var slots = new List<ComponentSlot>(count);
        for (var i = 0; i < count; i++)
        {
            slots.Add(new ComponentSlot(_children[i].Component, childRects[i]));
        }

        return slots;
    }

    private List<Rect> CreatePlaceholderRects()
    {
        var rects = new List<Rect>(_children.Count);
        for (var i = 0; i < _children.Count; i++)
        {
            rects.Add(default);
        }

        return rects;
    }

    private List<Rect> BuildChildRects(Rect rect)
    {
        return Mode switch
        {
            LayoutFlow.Columns => BuildHorizontalRects(rect),
            LayoutFlow.Grid => BuildGridRects(rect),
            _ => BuildVerticalRects(rect),
        };
    }

    private List<Rect> BuildVerticalRects(Rect rect)
    {
        var rects = new List<Rect>(_children.Count);
        if (_children.Count == 0)
        {
            return rects;
        }

        if (_children.Count == 2 && PrimarySize.HasValue)
        {
            var (first, second) = UiLayout.SplitHorizontal(
                rect,
                PrimarySize.Value,
                minFirst: Math.Max(0, MinPrimarySize),
                minSecond: Math.Max(0, MinSecondarySize));
            rects.Add(first);
            rects.Add(second);
            return rects;
        }

        var totalWeight = _children.Sum(entry => entry.Weight);
        var y = rect.Y;
        var consumed = 0;
        for (var i = 0; i < _children.Count; i++)
        {
            var remainingHeight = rect.Height - consumed;
            if (remainingHeight <= 0)
            {
                break;
            }

            var planned = i == _children.Count - 1
                ? remainingHeight
                : Math.Max(1, (rect.Height * _children[i].Weight) / Math.Max(1, totalWeight));
            var h = Math.Min(remainingHeight, planned);
            rects.Add(new Rect(rect.X, y, rect.Width, h));
            y += h;
            consumed += h;
        }

        return rects;
    }

    private List<Rect> BuildHorizontalRects(Rect rect)
    {
        var rects = new List<Rect>(_children.Count);
        if (_children.Count == 0)
        {
            return rects;
        }

        if (_children.Count == 2 && PrimarySize.HasValue)
        {
            var (first, second) = UiLayout.SplitVertical(
                rect,
                PrimarySize.Value,
                minFirst: Math.Max(0, MinPrimarySize),
                minSecond: Math.Max(0, MinSecondarySize));
            rects.Add(first);
            rects.Add(second);
            return rects;
        }

        var totalWeight = _children.Sum(entry => entry.Weight);
        var x = rect.X;
        var consumed = 0;
        for (var i = 0; i < _children.Count; i++)
        {
            var remainingWidth = rect.Width - consumed;
            if (remainingWidth <= 0)
            {
                break;
            }

            var planned = i == _children.Count - 1
                ? remainingWidth
                : Math.Max(1, (rect.Width * _children[i].Weight) / Math.Max(1, totalWeight));
            var w = Math.Min(remainingWidth, planned);
            rects.Add(new Rect(x, rect.Y, w, rect.Height));
            x += w;
            consumed += w;
        }

        return rects;
    }

    private List<Rect> BuildGridRects(Rect rect)
    {
        var rows = Math.Max(1, GridRows);
        var columns = Math.Max(1, GridColumns);
        var cells = UiLayout.Grid(rect, rows, columns);
        var count = Math.Min(cells.Length, _children.Count);
        var rects = new List<Rect>(count);
        for (var i = 0; i < count; i++)
        {
            rects.Add(cells[i]);
        }

        return rects;
    }
}
