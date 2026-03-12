using TeaSharp.Components.Primitives;

using TeaSharp.Components.Composition;

namespace TeaSharp.Layout;

/// <summary>
/// Represents multiple layout nodes composed over the same bounds.
/// </summary>
public sealed class OverlayLayout : LayoutNode
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

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path)
    {
        for (var index = 0; index < Items.Count; index++)
        {
            Items[index].Compose(screen, bounds, $"{path}/overlay:{index}");
        }
    }
}
