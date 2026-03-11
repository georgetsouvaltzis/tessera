using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Represents an ordered row or column layout.
/// </summary>
public sealed class StackLayout : LayoutNode
{
    internal StackLayout(bool horizontal, IReadOnlyList<LayoutSlot> children, int gap, Thickness padding)
    {
        IsHorizontal = horizontal;
        Children = children ?? throw new ArgumentNullException(nameof(children));
        Gap = Math.Max(0, gap);
        Padding = padding;
    }

    /// <summary>
    /// Gets a value indicating whether the stack flows horizontally or vertically.
    /// </summary>
    public bool IsHorizontal { get; }

    /// <summary>
    /// Gets the stack children in layout order.
    /// </summary>
    public IReadOnlyList<LayoutSlot> Children { get; }

    /// <summary>
    /// Gets the inter-item gap.
    /// </summary>
    public int Gap { get; }

    /// <summary>
    /// Gets the inner padding applied to the arranged content.
    /// </summary>
    public Thickness Padding { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds) =>
        LayoutArrangement.MeasureStack(IsHorizontal, Children, Gap, Padding, availableBounds);

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path) =>
        LayoutArrangement.ComposeStack(screen, IsHorizontal, Children, Gap, Padding, bounds, path);
}
