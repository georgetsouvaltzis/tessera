using Tessera.Components.Primitives;

namespace Tessera.Layout;

internal sealed class StackLayout : LayoutNode
{
    public StackLayout(LayoutOrientation orientation, IReadOnlyList<LayoutSlot> children, int gap = 0,
        Thickness padding = default)
    {
        Orientation = orientation;
        Children = children ?? throw new ArgumentNullException(nameof(children));
        Gap = Math.Max(0, gap);
        Padding = padding;
    }

    public StackLayout(LayoutOrientation orientation, int gap = 0, Thickness padding = default,
        params LayoutSlot[] children)
        : this(orientation, children, gap, padding)
    {
    }

    internal StackLayout(bool horizontal, IReadOnlyList<LayoutSlot> children, int gap, Thickness padding)
    {
        Orientation = horizontal ? LayoutOrientation.Horizontal : LayoutOrientation.Vertical;
        Children = children ?? throw new ArgumentNullException(nameof(children));
        Gap = Math.Max(0, gap);
        Padding = padding;
    }

    /// <summary>
    ///     Gets a value indicating whether the stack flows horizontally or vertically.
    /// </summary>
    public LayoutOrientation Orientation { get; }

    public bool IsHorizontal => Orientation == LayoutOrientation.Horizontal;

    /// <summary>
    ///     Gets the stack children in layout order.
    /// </summary>
    public IReadOnlyList<LayoutSlot> Children { get; }

    /// <summary>
    ///     Gets the inter-item gap.
    /// </summary>
    public int Gap { get; }

    /// <summary>
    ///     Gets the inner padding applied to the arranged content.
    /// </summary>
    public Thickness Padding { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return LayoutArrangement.MeasureStack(IsHorizontal, Children, Gap, Padding, availableBounds);
    }
}
