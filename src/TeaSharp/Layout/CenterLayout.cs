using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TeaSharp.Components.Composition;

namespace TeaSharp.Layout;

/// <summary>
/// Represents content centered within the available bounds.
/// </summary>
public sealed class CenterLayout : LayoutNode
{
    /// <summary>
    /// Creates an empty centered layout for object-initializer assembly.
    /// </summary>
    public CenterLayout()
    {
    }

    /// <summary>
    /// Creates a centered layout node around nested content.
    /// </summary>
    /// <param name="content">The content to center.</param>
    /// <param name="width">The explicit width to use, when supplied. When omitted, measured content width is used.</param>
    /// <param name="height">The explicit height to use, when supplied. When omitted, measured content height is used.</param>
    /// <param name="margin">The margin applied before centering.</param>
    [SetsRequiredMembers]
    public CenterLayout(LayoutNode content, int? width = null, int? height = null, Thickness margin = default)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Width = width;
        Height = height;
        Margin = margin;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SetsRequiredMembers]
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
    [SetsRequiredMembers]
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

    [SetsRequiredMembers]
    internal CenterLayout(
        ICanvasComponent component,
        int? width,
        int? height,
        Thickness margin,
        bool? focusable,
        bool focusOnClick,
        bool interceptsPointer,
        int layer,
        Action? onFocus)
        : this(
            new ComponentLayout(component, width, height, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            width,
            height,
            margin)
    {
    }

    /// <summary>
    /// Gets the centered content.
    /// </summary>
    public required LayoutNode Content { get; init; }

    /// <summary>
    /// Gets the explicit content width, if provided.
    /// </summary>
    public int? Width { get; init; }

    /// <summary>
    /// Gets the explicit content height, if provided.
    /// </summary>
    public int? Height { get; init; }

    /// <summary>
    /// Gets the outer margin applied before centering.
    /// </summary>
    public Thickness Margin { get; init; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var inner = availableBounds.Inset(Margin);
        var measured = GetContent().Measure(inner);
        var width = Width ?? measured.Width;
        var height = Height ?? measured.Height;
        return new LayoutMeasurement(
            Math.Clamp(width + Margin.Horizontal, 0, availableBounds.Width),
            Math.Clamp(height + Margin.Vertical, 0, availableBounds.Height));
    }

    private LayoutNode GetContent()
        => Content ?? throw new InvalidOperationException($"{nameof(CenterLayout)} requires {nameof(Content)} to be configured.");
}
