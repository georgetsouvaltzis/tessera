using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a vertical list of layout items.
/// </summary>
public sealed class ColumnLayout : LayoutNode
{
    public IList<LayoutSlot> Items { get; } = [];

    public int Gap { get; set; }

    public Thickness Padding { get; set; }

    public ColumnLayout Add(LayoutSlot item)
    {
        Items.Add(item ?? throw new ArgumentNullException(nameof(item)));
        return this;
    }

    public ColumnLayout AddAuto(LayoutNode content, Thickness margin = default)
        => Add(LayoutSlot.Auto(content, margin));

    public ColumnLayout AddAuto(ICanvasComponent component, Thickness margin = default)
        => Add(LayoutSlot.Auto(component, margin));

    public ColumnLayout AddFixed(LayoutNode content, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(content, size, margin));

    public ColumnLayout AddFixed(ICanvasComponent component, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(component, size, margin));

    public ColumnLayout AddFill(LayoutNode content, Thickness margin = default)
        => Add(LayoutSlot.Fill(content, margin));

    public ColumnLayout AddFill(ICanvasComponent component, Thickness margin = default)
        => Add(LayoutSlot.Fill(component, margin));

    public ColumnLayout AddWeighted(LayoutNode content, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(content, weight, margin));

    public ColumnLayout AddWeighted(ICanvasComponent component, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(component, weight, margin));

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return new StackLayout(LayoutOrientation.Vertical, [.. Items], Gap, Padding)
            .Measure(availableBounds);
    }

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path)
    {
        new StackLayout(LayoutOrientation.Vertical, [.. Items], Gap, Padding)
            .Compose(screen, bounds, path);
    }
}
