using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a deterministic two-slot split layout.
/// </summary>
public sealed class SplitLayout : LayoutNode
{
    internal SplitLayout(bool horizontal, LayoutSlot first, LayoutSlot second, int gap, Thickness padding)
    {
        IsHorizontal = horizontal;
        First = first ?? throw new ArgumentNullException(nameof(first));
        Second = second ?? throw new ArgumentNullException(nameof(second));
        Gap = Math.Max(0, gap);
        Padding = padding;
    }

    /// <summary>
    /// Gets a value indicating whether the split flows horizontally (`left/right`) or vertically (`top/bottom`).
    /// </summary>
    public bool IsHorizontal { get; }

    /// <summary>
    /// Gets the first slot.
    /// </summary>
    public LayoutSlot First { get; }

    /// <summary>
    /// Gets the second slot.
    /// </summary>
    public LayoutSlot Second { get; }

    /// <summary>
    /// Gets the inter-slot gap.
    /// </summary>
    public int Gap { get; }

    /// <summary>
    /// Gets the inner padding applied before arranging the split.
    /// </summary>
    public Thickness Padding { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds) =>
        LayoutArrangement.MeasureStack(
            IsHorizontal,
            new[] { First, Second },
            Gap,
            Padding,
            availableBounds);

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path)
    {
        LayoutArrangement.ComposeStack(
            screen,
            IsHorizontal,
            new[] { First, Second },
            Gap,
            Padding,
            bounds,
            path);
    }
}
