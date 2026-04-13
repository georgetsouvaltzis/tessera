using Tessera.Components.Primitives;

namespace Tessera.Layout;

internal sealed class DockLayout(
    LayoutSlot? top = null,
    LayoutSlot? bottom = null,
    LayoutSlot? left = null,
    LayoutSlot? right = null,
    LayoutSlot? fill = null,
    int gap = 0,
    Thickness padding = default) : LayoutNode
{
    /// <summary>
    ///     Gets the docked top content.
    /// </summary>
    public LayoutSlot? Top { get; } = top;

    /// <summary>
    ///     Gets the docked bottom content.
    /// </summary>
    public LayoutSlot? Bottom { get; } = bottom;

    /// <summary>
    ///     Gets the docked left content.
    /// </summary>
    public LayoutSlot? Left { get; } = left;

    /// <summary>
    ///     Gets the docked right content.
    /// </summary>
    public LayoutSlot? Right { get; } = right;

    /// <summary>
    ///     Gets the fill content.
    /// </summary>
    public LayoutSlot? Fill { get; } = fill;

    /// <summary>
    ///     Gets the gap inserted between docked regions.
    /// </summary>
    public int Gap { get; } = Math.Max(0, gap);

    /// <summary>
    ///     Gets the layout padding.
    /// </summary>
    public Thickness Padding { get; } = padding;

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return LayoutArrangement.MeasureDock(this, availableBounds);
    }
}
