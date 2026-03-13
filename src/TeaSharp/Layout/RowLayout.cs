using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Core.Abstractions;
using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a horizontal set of layout items.
/// </summary>
/// <remarks>
/// Prefer object and collection initializer assembly through <see cref="Items"/> on the default path. The
/// <c>Add*</c> helpers remain available as advanced convenience methods.
/// </remarks>
public sealed class RowLayout : LayoutNode
{
    /// <summary>
    /// Gets the arranged row items in left-to-right order.
    /// </summary>
    public IList<LayoutSlot> Items { get; } = [];

    /// <summary>
    /// Gets or sets the gap between items.
    /// </summary>
    public int Gap { get; set; }

    /// <summary>
    /// Gets or sets the inner padding applied before arranging items.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Adds a preconfigured slot to the row.
    /// </summary>
    /// <param name="item">The slot to add.</param>
    /// <returns>The current row layout.</returns>
    public RowLayout Add(LayoutSlot item)
    {
        Items.Add(item ?? throw new ArgumentNullException(nameof(item)));
        return this;
    }

    /// <summary>
    /// Adds an auto-sized layout node.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddAuto(LayoutNode content, Thickness margin = default)
        => Add(LayoutSlot.Auto(content, margin));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddAuto(ICanvasComponent component, Thickness margin = default)
        => Add(LayoutSlot.Auto(component, margin));

    /// <summary>
    /// Adds an auto-sized control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddAuto(Control control, Thickness margin = default)
        => Add(LayoutSlot.Auto(control, margin));

    /// <summary>
    /// Adds a fixed-size layout node.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddFixed(LayoutNode content, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(content, size, margin));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddFixed(ICanvasComponent component, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(component, size, margin));

    /// <summary>
    /// Adds a fixed-size control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddFixed(Control control, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(control, size, margin));

    /// <summary>
    /// Adds a fill layout node.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddFill(LayoutNode content, Thickness margin = default)
        => Add(LayoutSlot.Fill(content, margin));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddFill(ICanvasComponent component, Thickness margin = default)
        => Add(LayoutSlot.Fill(component, margin));

    /// <summary>
    /// Adds a fill control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddFill(Control control, Thickness margin = default)
        => Add(LayoutSlot.Fill(control, margin));

    /// <summary>
    /// Adds a weighted layout node.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddWeighted(LayoutNode content, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(content, weight, margin));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddWeighted(ICanvasComponent component, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(component, weight, margin));

    /// <summary>
    /// Adds a weighted control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public RowLayout AddWeighted(Control control, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(control, weight, margin));

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return new StackLayout(LayoutOrientation.Horizontal, [.. Items], Gap, Padding)
            .Measure(availableBounds);
    }

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path)
    {
        new StackLayout(LayoutOrientation.Horizontal, [.. Items], Gap, Padding)
            .Compose(screen, bounds, path);
    }
}
