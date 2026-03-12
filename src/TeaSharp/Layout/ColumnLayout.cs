using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Core.Abstractions;
using System.ComponentModel;

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

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddAuto(ICanvasComponent component, Thickness margin = default)
        => Add(LayoutSlot.Auto(component, margin));

    public ColumnLayout AddAuto(Control control, Thickness margin = default)
        => Add(LayoutSlot.Auto(control, margin));

    public ColumnLayout AddFixed(LayoutNode content, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(content, size, margin));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddFixed(ICanvasComponent component, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(component, size, margin));

    public ColumnLayout AddFixed(Control control, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(control, size, margin));

    public ColumnLayout AddFill(LayoutNode content, Thickness margin = default)
        => Add(LayoutSlot.Fill(content, margin));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddFill(ICanvasComponent component, Thickness margin = default)
        => Add(LayoutSlot.Fill(component, margin));

    public ColumnLayout AddFill(Control control, Thickness margin = default)
        => Add(LayoutSlot.Fill(control, margin));

    public ColumnLayout AddWeighted(LayoutNode content, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(content, weight, margin));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddWeighted(ICanvasComponent component, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(component, weight, margin));

    public ColumnLayout AddWeighted(Control control, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(control, weight, margin));

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
