using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a window-like screen with named sections.
/// </summary>
/// <remarks>
/// This is the default shell-style composition type for non-trivial screens. Use its named sections to keep
/// screen assembly shallow instead of building large nested layout trees.
/// </remarks>
public sealed class WindowLayout : LayoutNode
{
    /// <summary>
    /// Gets or sets the optional top section.
    /// </summary>
    public LayoutSlot? Header { get; set; }

    /// <summary>
    /// Gets or sets the optional bottom section.
    /// </summary>
    public LayoutSlot? Footer { get; set; }

    /// <summary>
    /// Gets or sets the optional left section.
    /// </summary>
    public LayoutSlot? Left { get; set; }

    /// <summary>
    /// Gets or sets the optional right section.
    /// </summary>
    public LayoutSlot? Right { get; set; }

    /// <summary>
    /// Gets or sets the main body content.
    /// </summary>
    public LayoutNode? Body { get; set; }

    /// <summary>
    /// Gets or sets optional overlay content composed over the window body.
    /// </summary>
    public LayoutNode? Overlay { get; set; }

    /// <summary>
    /// Gets or sets the gap between named sections.
    /// </summary>
    public int Gap { get; set; }

    /// <summary>
    /// Gets or sets the inner padding applied before arranging sections.
    /// </summary>
    public Thickness Padding { get; set; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return CreateRoot().Measure(availableBounds);
    }
    private LayoutNode CreateRoot()
    {
        var content = new DockLayout(
            top: Header,
            bottom: Footer,
            left: Left,
            right: Right,
            fill: Body is null ? null : LayoutSlot.Fill(Body),
            gap: Gap,
            padding: Padding);

        return Overlay is null
            ? content
            : new OverlayLayout(content, Overlay);
    }
}
