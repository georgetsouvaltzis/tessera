using Tessera.Components.Primitives;
using Tessera.Controls;
using System.ComponentModel;
using Tessera.Components.Composition;

namespace Tessera.Layout;

/// <summary>
/// Represents a vertical list of layout items.
/// </summary>
/// <remarks>
/// Prefer object and collection initializer assembly through <see cref="Items"/> on the default path. The
/// <c>Add*</c> helpers remain available as advanced convenience methods.
/// </remarks>
public sealed class ColumnLayout : LayoutNode
{
    /// <summary>
    /// Gets the arranged column items in top-to-bottom order.
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
    /// Adds a preconfigured slot to the column.
    /// </summary>
    /// <param name="item">The slot to add.</param>
    /// <returns>The current column layout.</returns>
    public ColumnLayout Add(LayoutSlot item)
    {
        Items.Add(item ?? throw new ArgumentNullException(nameof(item)));
        return this;
    }

    /// <summary>
    /// Adds an auto-sized layout node.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddAuto(LayoutNode content, Thickness margin = default)
        => Add(LayoutSlot.Auto(content, margin));

    /// <remarks>
    /// Raw <see cref="ICanvasComponent"/> interop is render-only. Use <see cref="Control"/> when the content needs
    /// focus or input handling.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddAuto(ICanvasComponent component, Thickness margin = default)
        => Add(LayoutSlot.Auto(component, margin));

    /// <summary>
    /// Adds an auto-sized control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddAuto(Control control, Thickness margin = default)
        => Add(LayoutSlot.Auto(control, margin));

    /// <summary>
    /// Adds a fixed-size layout node.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddFixed(LayoutNode content, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(content, size, margin));

    /// <remarks>
    /// Raw <see cref="ICanvasComponent"/> interop is render-only. Use <see cref="Control"/> when the content needs
    /// focus or input handling.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddFixed(ICanvasComponent component, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(component, size, margin));

    /// <summary>
    /// Adds a fixed-size control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddFixed(Control control, int size, Thickness margin = default)
        => Add(LayoutSlot.Fixed(control, size, margin));

    /// <summary>
    /// Adds a fill layout node.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddFill(LayoutNode content, Thickness margin = default)
        => Add(LayoutSlot.Fill(content, margin));

    /// <remarks>
    /// Raw <see cref="ICanvasComponent"/> interop is render-only. Use <see cref="Control"/> when the content needs
    /// focus or input handling.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddFill(ICanvasComponent component, Thickness margin = default)
        => Add(LayoutSlot.Fill(component, margin));

    /// <summary>
    /// Adds a fill control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddFill(Control control, Thickness margin = default)
        => Add(LayoutSlot.Fill(control, margin));

    /// <summary>
    /// Adds a weighted layout node.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddWeighted(LayoutNode content, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(content, weight, margin));

    /// <remarks>
    /// Raw <see cref="ICanvasComponent"/> interop is render-only. Use <see cref="Control"/> when the content needs
    /// focus or input handling.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddWeighted(ICanvasComponent component, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(component, weight, margin));

    /// <summary>
    /// Adds a weighted control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ColumnLayout AddWeighted(Control control, int weight, Thickness margin = default)
        => Add(LayoutSlot.Weighted(control, weight, margin));

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return new StackLayout(LayoutOrientation.Vertical, [.. Items], Gap, Padding)
            .Measure(availableBounds);
    }
}
