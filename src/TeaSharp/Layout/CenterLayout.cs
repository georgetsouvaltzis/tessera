using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Represents content centered within the available bounds.
/// </summary>
public sealed class CenterLayout : LayoutNode
{
    internal CenterLayout(LayoutNode content, int? width, int? height, Thickness margin)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Width = width;
        Height = height;
        Margin = margin;
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
