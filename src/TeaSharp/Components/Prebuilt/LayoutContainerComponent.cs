using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Composition.Internal;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components.Prebuilt;

internal sealed partial class LayoutContainerComponent : IStatefulComponent, IMouseStatefulComponent
{
    private readonly List<(ICanvasComponent Component, int Weight)> _children = [];
    private bool _draggingSplit;

    public LayoutContainerComponent()
    {
    }

    public LayoutContainerComponent(LayoutContainerOptions options)
    {
        Mode = options.Mode;
        GridRows = options.GridRows;
        GridColumns = options.GridColumns;
        EnableMouseInteractions = options.EnableMouseInteractions;
        EnableMouseResize = options.EnableMouseResize;
        SplitterHitThickness = options.SplitterHitThickness;
        MinPrimarySize = options.MinPrimarySize;
        MinSecondarySize = options.MinSecondarySize;
        PrimarySize = options.PrimarySize;
    }

    public LayoutFlow Mode { get; set; } = LayoutFlow.Rows;

    public int GridRows { get; set; } = 1;

    public int GridColumns { get; set; } = 1;

    public bool EnableMouseInteractions { get; set; } = true;

    public bool EnableMouseResize { get; set; } = true;

    public int SplitterHitThickness { get; set; } = 1;

    public int MinPrimarySize { get; set; } = 8;

    public int MinSecondarySize { get; set; } = 8;

    public int? PrimarySize { get; private set; }

    public IReadOnlyList<(ICanvasComponent Component, int Weight)> Children => _children;

    public void Clear()
    {
        _children.Clear();
        _draggingSplit = false;
        PrimarySize = null;
    }

    public void Add(ICanvasComponent component, int weight = 1)
    {
        _children.Add((component, Math.Max(1, weight)));
    }

    public void SetPrimarySize(int size)
    {
        PrimarySize = Math.Max(0, size);
    }

    public void ClearPrimarySize()
    {
        PrimarySize = null;
    }

    public bool Update(IMessage message)
    {
        var slots = CreateSlots();
        var focusedSlotIndex = ComponentRouting.DetectFocusedSlotIndex(slots);
        return ComponentRouting.Update(
            slots,
            message,
            true,
            true,
            KeyboardRoutingMode.FocusedOnly,
            ref focusedSlotIndex);
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (!EnableMouseInteractions || _children.Count == 0 || bounds.IsEmpty)
        {
            return false;
        }

        var rects = BuildChildRects(bounds);
        var changed = HandleSplitMouse(message, bounds, rects, out var splitConsumed);
        if (splitConsumed)
        {
            return changed;
        }

        var slots = CreateSlots(rects);
        var focusedSlotIndex = ComponentRouting.DetectFocusedSlotIndex(slots);
        changed |= ComponentRouting.Update(
            slots,
            message,
            true,
            true,
            KeyboardRoutingMode.FocusedOnly,
            ref focusedSlotIndex);
        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (_children.Count == 0)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var rects = BuildChildRects(clipped);
        var count = Math.Min(_children.Count, rects.Count);
        for (var i = 0; i < count; i++)
        {
            _children[i].Component.Render(canvas, rects[i]);
        }
    }

    private bool HandleSplitMouse(MouseMsg message, Rect bounds, List<Rect> rects, out bool consumed)
    {
        consumed = false;
        if (!EnableMouseResize || Mode == LayoutFlow.Grid || _children.Count != 2 || rects.Count < 2)
        {
            return false;
        }

        if (message is MouseReleaseMsg { Button: MouseButton.Left } && _draggingSplit)
        {
            _draggingSplit = false;
            consumed = true;
            return true;
        }

        if (!TryGetSplitterHitRect(bounds, rects[0], out var splitterHit))
        {
            return false;
        }

        if (message is MouseClickMsg { Button: MouseButton.Left } click
            && splitterHit.Contains(click.X, click.Y))
        {
            _draggingSplit = true;
            consumed = true;
            return true;
        }

        if (message is MouseMotionMsg motion && _draggingSplit)
        {
            consumed = true;
            return ApplyDraggedPrimarySize(bounds, motion.X, motion.Y);
        }

        return false;
    }

    private bool ApplyDraggedPrimarySize(Rect bounds, int x, int y)
    {
        var totalSize = Mode == LayoutFlow.Columns
            ? bounds.Width
            : bounds.Height;
        if (totalSize <= 0)
        {
            return false;
        }

        var requested = Mode == LayoutFlow.Columns
            ? x - bounds.X
            : y - bounds.Y;
        var minFirst = Math.Clamp(MinPrimarySize, 0, totalSize);
        var maxSecond = Math.Max(0, totalSize - minFirst);
        var minSecond = Math.Clamp(MinSecondarySize, 0, maxSecond);
        var clamped = Math.Clamp(requested, minFirst, totalSize - minSecond);

        if (PrimarySize == clamped)
        {
            return false;
        }

        PrimarySize = clamped;
        return true;
    }

    private bool TryGetSplitterHitRect(Rect bounds, Rect firstRect, out Rect splitterHit)
    {
        splitterHit = default;
        var thickness = Math.Max(1, SplitterHitThickness);
        if (Mode == LayoutFlow.Columns)
        {
            var center = firstRect.Right;
            var start = center - (thickness / 2);
            splitterHit = Rect.Intersect(new Rect(start, bounds.Y, thickness, bounds.Height), bounds);
            return !splitterHit.IsEmpty;
        }

        if (Mode == LayoutFlow.Rows)
        {
            var center = firstRect.Bottom;
            var start = center - (thickness / 2);
            splitterHit = Rect.Intersect(new Rect(bounds.X, start, bounds.Width, thickness), bounds);
            return !splitterHit.IsEmpty;
        }

        return false;
    }
}
