using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a grouped container with optional frame styling and nested content.
/// </summary>
public sealed class PanelLayout : LayoutNode
{
    public PanelLayout(LayoutNode content, string? title = null, BorderStyle border = BorderStyle.None, Thickness padding = default, Thickness margin = default)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Title = title;
        Border = border;
        Padding = padding;
        Margin = margin;
    }

    public PanelLayout(
        ICanvasComponent component,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default,
        ScreenRegionKey? regionKey = null,
        int? preferredWidth = null,
        int? preferredHeight = null,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null)
        : this(
            new ComponentLayout(component, regionKey, preferredWidth, preferredHeight, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            title,
            border,
            padding,
            margin)
    {
    }

    /// <summary>
    /// Gets the nested layout content.
    /// </summary>
    public LayoutNode Content { get; }

    /// <summary>
    /// Gets the optional panel title.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// Gets the frame border style.
    /// </summary>
    public BorderStyle Border { get; }

    /// <summary>
    /// Gets the inner panel padding.
    /// </summary>
    public Thickness Padding { get; }

    /// <summary>
    /// Gets the outer panel margin.
    /// </summary>
    public Thickness Margin { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var inner = Rect.Intersect(availableBounds.Inset(Margin), availableBounds);
        var contentBounds = FrameLayout.ResolveContentRect(inner, Border, Padding);
        var measured = Content.Measure(contentBounds);
        var width = measured.Width + Margin.Horizontal + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        var height = measured.Height + Margin.Vertical + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);

        if (!string.IsNullOrWhiteSpace(Title))
        {
            width = Math.Max(width, Title!.Length + (Border == BorderStyle.None ? 0 : 4));
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path)
    {
        var outer = Rect.Intersect(bounds.Inset(Margin), bounds);
        if (outer.IsEmpty)
        {
            return;
        }

        if (Border != BorderStyle.None)
        {
            screen.AddRegion(
                LayoutRegionKeys.Generated(path, "panel"),
                outer,
                (canvas, rect) => canvas.DrawBox(rect, Title, Border),
                focusable: false,
                focusOnClick: false,
                interceptsPointer: false);
        }

        var contentRect = FrameLayout.ResolveContentRect(outer, Border, Padding);
        if (contentRect.IsEmpty)
        {
            return;
        }

        Content.Compose(screen, contentRect, $"{path}/content");
    }
}
