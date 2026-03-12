using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a window-like screen with named sections.
/// </summary>
public sealed class WindowLayout : LayoutNode
{
    public LayoutSlot? Header { get; set; }

    public LayoutSlot? Footer { get; set; }

    public LayoutSlot? Left { get; set; }

    public LayoutSlot? Right { get; set; }

    public LayoutNode? Body { get; set; }

    public LayoutNode? Overlay { get; set; }

    public int Gap { get; set; }

    public Thickness Padding { get; set; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return CreateRoot().Measure(availableBounds);
    }

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path)
    {
        CreateRoot().Compose(screen, bounds, path);
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
