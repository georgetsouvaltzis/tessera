using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Represents content centered within the available bounds.
/// </summary>
public sealed class CenterLayout : LayoutNode
{
    /// <summary>
    /// Creates a centered layout node around nested content.
    /// </summary>
    /// <param name="content">The content to center.</param>
    /// <param name="width">The explicit width to use, when supplied. When omitted, measured content width is used.</param>
    /// <param name="height">The explicit height to use, when supplied. When omitted, measured content height is used.</param>
    /// <param name="margin">The margin applied before centering.</param>
    public CenterLayout(LayoutNode content, int? width = null, int? height = null, Thickness margin = default)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Width = width;
        Height = height;
        Margin = margin;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public CenterLayout(
        ICanvasComponent component,
        int? width = null,
        int? height = null,
        Thickness margin = default)
        : this(
            new ComponentLayout(component),
            width,
            height,
            margin)
    {
    }

    /// <summary>
    /// Creates a centered layout node around a control.
    /// </summary>
    /// <param name="control">The control to center.</param>
    /// <param name="width">The explicit width to use, when supplied. When omitted, measured control width is used.</param>
    /// <param name="height">The explicit height to use, when supplied. When omitted, measured control height is used.</param>
    /// <param name="margin">The margin applied before centering.</param>
    public CenterLayout(
        Control control,
        int? width = null,
        int? height = null,
        Thickness margin = default)
        : this(
            new ComponentLayout(control),
            width,
            height,
            margin)
    {
    }

    internal CenterLayout(
        ICanvasComponent component,
        int? width,
        int? height,
        Thickness margin,
        ScreenRegionKey? regionKey,
        bool? focusable,
        bool focusOnClick,
        bool interceptsPointer,
        int layer,
        Action? onFocus)
        : this(
            new ComponentLayout(component, regionKey, width, height, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            width,
            height,
            margin)
    {
    }

    /// <summary>
    /// Gets the centered content.
    /// </summary>
    public LayoutNode Content { get; }

    /// <summary>
    /// Gets the explicit content width, if provided.
    /// </summary>
    public int? Width { get; }

    /// <summary>
    /// Gets the explicit content height, if provided.
    /// </summary>
    public int? Height { get; }

    /// <summary>
    /// Gets the outer margin applied before centering.
    /// </summary>
    public Thickness Margin { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var inner = availableBounds.Inset(Margin);
        var measured = Content.Measure(inner);
        var width = Width ?? measured.Width;
        var height = Height ?? measured.Height;
        return new LayoutMeasurement(
            Math.Clamp(width + Margin.Horizontal, 0, availableBounds.Width),
            Math.Clamp(height + Margin.Vertical, 0, availableBounds.Height));
    }

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path)
    {
        var inner = Rect.Intersect(bounds.Inset(Margin), bounds);
        if (inner.IsEmpty)
        {
            return;
        }

        var measured = Content.Measure(inner);
        var width = Math.Clamp(Width ?? measured.Width, 0, inner.Width);
        var height = Math.Clamp(Height ?? measured.Height, 0, inner.Height);
        var x = inner.X + Math.Max(0, (inner.Width - width) / 2);
        var y = inner.Y + Math.Max(0, (inner.Height - height) / 2);
        Content.Compose(screen, new Rect(x, y, width, height), $"{path}/center");
    }
}
