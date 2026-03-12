using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a docked layout with optional top, bottom, left, right, and fill content.
/// </summary>
public sealed class DockLayout : LayoutNode
{
    public DockLayout(
        LayoutSlot? top = null,
        LayoutSlot? bottom = null,
        LayoutSlot? left = null,
        LayoutSlot? right = null,
        LayoutSlot? fill = null,
        int gap = 0,
        Thickness padding = default)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
        Fill = fill;
        Gap = Math.Max(0, gap);
        Padding = padding;
    }

    /// <summary>
    /// Gets the docked top content.
    /// </summary>
    public LayoutSlot? Top { get; }

    /// <summary>
    /// Gets the docked bottom content.
    /// </summary>
    public LayoutSlot? Bottom { get; }

    /// <summary>
    /// Gets the docked left content.
    /// </summary>
    public LayoutSlot? Left { get; }

    /// <summary>
    /// Gets the docked right content.
    /// </summary>
    public LayoutSlot? Right { get; }

    /// <summary>
    /// Gets the fill content.
    /// </summary>
    public LayoutSlot? Fill { get; }

    /// <summary>
    /// Gets the gap inserted between docked regions.
    /// </summary>
    public int Gap { get; }

    /// <summary>
    /// Gets the layout padding.
    /// </summary>
    public Thickness Padding { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds) =>
        LayoutArrangement.MeasureDock(this, availableBounds);

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path) =>
        LayoutArrangement.ComposeDock(screen, this, bounds, path);
}
