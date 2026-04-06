using Tessera.Components.Primitives;

namespace Tessera.Layout;

internal sealed class OverlayLayout : LayoutNode
{
    public OverlayLayout(IReadOnlyList<LayoutNode> items)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public OverlayLayout(params LayoutNode[] items)
        : this((IReadOnlyList<LayoutNode>)items)
    {
    }

    /// <summary>
    /// Gets the overlay items in back-to-front order.
    /// </summary>
    public IReadOnlyList<LayoutNode> Items { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 0;
        var height = 0;
        foreach (var item in Items)
        {
            var measured = item.Measure(availableBounds);
            width = Math.Max(width, measured.Width);
            height = Math.Max(height, measured.Height);
        }

        return new LayoutMeasurement(width, height);
    }
}
