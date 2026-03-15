using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TeaSharp.Components.Composition;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a grouped container with optional frame styling and nested content.
/// </summary>
/// <remarks>
/// Border and padding reduce the inner content area before the nested content is measured and composed.
/// </remarks>
public sealed class PanelLayout : LayoutNode
{
    /// <summary>
    /// Creates an empty panel layout for object-initializer assembly.
    /// </summary>
    public PanelLayout()
    {
    }

    /// <summary>
    /// Creates a panel layout around nested content.
    /// </summary>
    /// <param name="content">The content shown inside the panel.</param>
    /// <param name="title">The optional panel title.</param>
    /// <param name="border">The frame border style.</param>
    /// <param name="padding">The inner panel padding.</param>
    /// <param name="margin">The outer panel margin.</param>
    [SetsRequiredMembers]
    public PanelLayout(LayoutNode content, string? title = null, BorderStyle border = BorderStyle.None, Thickness padding = default, Thickness margin = default)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Title = title;
        Border = border;
        Padding = padding;
        Margin = margin;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SetsRequiredMembers]
    public PanelLayout(
        ICanvasComponent component,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default)
        : this(
            new ComponentLayout(component),
            title,
            border,
            padding,
            margin)
    {
    }

    /// <summary>
    /// Creates a panel layout around a control.
    /// </summary>
    /// <param name="control">The control shown inside the panel.</param>
    /// <param name="title">The optional panel title.</param>
    /// <param name="border">The frame border style.</param>
    /// <param name="padding">The inner panel padding.</param>
    /// <param name="margin">The outer panel margin.</param>
    [SetsRequiredMembers]
    public PanelLayout(
        Control control,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default)
        : this(
            new ComponentLayout(control),
            title,
            border,
            padding,
            margin)
    {
    }

    [SetsRequiredMembers]
    internal PanelLayout(
        ICanvasComponent component,
        string? title,
        BorderStyle border,
        Thickness padding,
        Thickness margin,
        int? preferredWidth,
        int? preferredHeight,
        bool? focusable,
        bool focusOnClick,
        bool interceptsPointer,
        int layer,
        Action? onFocus)
        : this(
            new ComponentLayout(component, preferredWidth, preferredHeight, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            title,
            border,
            padding,
            margin)
    {
    }

    /// <summary>
    /// Gets the nested layout content.
    /// </summary>
    public required LayoutNode Content { get; init; }

    /// <summary>
    /// Gets the optional panel title.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the frame border style.
    /// </summary>
    public BorderStyle Border { get; init; }

    /// <summary>
    /// Gets the inner panel padding.
    /// </summary>
    public Thickness Padding { get; init; }

    /// <summary>
    /// Gets the outer panel margin.
    /// </summary>
    public Thickness Margin { get; init; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var inner = Rect.Intersect(availableBounds.Inset(Margin), availableBounds);
        var contentBounds = FrameLayout.ResolveContentRect(inner, Border, Padding);
        var measured = GetContent().Measure(contentBounds);
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

    private LayoutNode GetContent()
        => Content ?? throw new InvalidOperationException($"{nameof(PanelLayout)} requires {nameof(Content)} to be configured.");
}
