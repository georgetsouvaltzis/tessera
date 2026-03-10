using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public sealed class LayoutContainerComponent : IStatefulComponent, IMouseStatefulComponent
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

    public LayoutContainerMode Mode { get; set; } = LayoutContainerMode.Vertical;

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
        var composer = CreateComposer();
        return composer.Update(message);
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

        var composer = CreateComposer(rects);
        changed |= composer.Update(message);
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

    private ComponentComposer CreateComposer(List<Rect>? rects = null)
    {
        var composer = new ComponentComposer
        {
            ClickToFocusEnabled = true,
            RouteMouseWheelToFocusedSlot = true,
            KeyboardRoutingMode = KeyboardRoutingMode.FocusedOnly,
        };

        var childRects = rects ?? CreatePlaceholderRects();
        var count = Math.Min(_children.Count, childRects.Count);
        for (var i = 0; i < count; i++)
        {
            composer.Add(_children[i].Component, childRects[i]);
        }

        return composer;
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
            LayoutContainerMode.Horizontal => BuildHorizontalRects(rect),
            LayoutContainerMode.Grid => BuildGridRects(rect),
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
            var (first, second) = Layout.SplitHorizontal(
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
            var (first, second) = Layout.SplitVertical(
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
        var cells = Layout.Grid(rect, rows, columns);
        var count = Math.Min(cells.Length, _children.Count);
        var rects = new List<Rect>(count);
        for (var i = 0; i < count; i++)
        {
            rects.Add(cells[i]);
        }

        return rects;
    }

    private bool HandleSplitMouse(MouseMsg message, Rect bounds, List<Rect> rects, out bool consumed)
    {
        consumed = false;
        if (!EnableMouseResize || Mode == LayoutContainerMode.Grid || _children.Count != 2 || rects.Count < 2)
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
        var totalSize = Mode == LayoutContainerMode.Horizontal
            ? bounds.Width
            : bounds.Height;
        if (totalSize <= 0)
        {
            return false;
        }

        var requested = Mode == LayoutContainerMode.Horizontal
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
        if (Mode == LayoutContainerMode.Horizontal)
        {
            var center = firstRect.Right;
            var start = center - (thickness / 2);
            splitterHit = Rect.Intersect(new Rect(start, bounds.Y, thickness, bounds.Height), bounds);
            return !splitterHit.IsEmpty;
        }

        if (Mode == LayoutContainerMode.Vertical)
        {
            var center = firstRect.Bottom;
            var start = center - (thickness / 2);
            splitterHit = Rect.Intersect(new Rect(bounds.X, start, bounds.Width, thickness), bounds);
            return !splitterHit.IsEmpty;
        }

        return false;
    }
}
