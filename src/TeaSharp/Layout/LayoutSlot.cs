using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Components.Composition;
using TeaSharp.Controls;
using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Associates content with a sizing rule and outer margin within a layout container.
/// </summary>
public sealed record LayoutSlot
{
    public static LayoutSlot Auto(LayoutNode content, Thickness margin = default) =>
        new(content, LayoutLength.Auto(), margin);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static LayoutSlot Auto(ICanvasComponent component, Thickness margin = default) =>
        new(component, LayoutLength.Auto(), margin);

    public static LayoutSlot Auto(Control control, Thickness margin = default) =>
        new(control, LayoutLength.Auto(), margin);

    public static LayoutSlot Fixed(LayoutNode content, int size, Thickness margin = default) =>
        new(content, LayoutLength.Fixed(size), margin);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static LayoutSlot Fixed(ICanvasComponent component, int size, Thickness margin = default) =>
        new(component, LayoutLength.Fixed(size), margin);

    public static LayoutSlot Fixed(Control control, int size, Thickness margin = default) =>
        new(control, LayoutLength.Fixed(size), margin);

    public static LayoutSlot Fill(LayoutNode content, Thickness margin = default) =>
        new(content, LayoutLength.Fill(), margin);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static LayoutSlot Fill(ICanvasComponent component, Thickness margin = default) =>
        new(component, LayoutLength.Fill(), margin);

    public static LayoutSlot Fill(Control control, Thickness margin = default) =>
        new(control, LayoutLength.Fill(), margin);

    public static LayoutSlot Weighted(LayoutNode content, int weight, Thickness margin = default) =>
        new(content, LayoutLength.Weighted(weight), margin);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static LayoutSlot Weighted(ICanvasComponent component, int weight, Thickness margin = default) =>
        new(component, LayoutLength.Weighted(weight), margin);

    public static LayoutSlot Weighted(Control control, int weight, Thickness margin = default) =>
        new(control, LayoutLength.Weighted(weight), margin);

    /// <summary>
    /// Creates a slot for the provided TeaSharp component.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public LayoutSlot(
        ICanvasComponent component,
        LayoutLength length,
        Thickness margin = default)
        : this(
            new ComponentLayout(component),
            length,
            margin)
    {
    }

    public LayoutSlot(
        Control control,
        LayoutLength length,
        Thickness margin = default)
        : this(
            new ComponentLayout(control),
            length,
            margin)
    {
    }

    internal LayoutSlot(
        ICanvasComponent component,
        LayoutLength length,
        Thickness margin,
        ScreenRegionKey? regionKey,
        int? preferredWidth,
        int? preferredHeight,
        bool? focusable,
        bool focusOnClick,
        bool interceptsPointer,
        int layer,
        Action? onFocus)
        : this(
            new ComponentLayout(component, regionKey, preferredWidth, preferredHeight, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            length,
            margin)
    {
    }

    /// <summary>
    /// Creates a slot for the provided content.
    /// </summary>
    /// <param name="content">The layout content.</param>
    /// <param name="length">The primary-axis sizing rule.</param>
    /// <param name="margin">The outer margin applied around the slot content.</param>
    public LayoutSlot(LayoutNode content, LayoutLength length, Thickness margin = default)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Length = length;
        Margin = margin;
    }

    /// <summary>
    /// Gets the slot content.
    /// </summary>
    public LayoutNode Content { get; }

    /// <summary>
    /// Gets the primary-axis sizing rule.
    /// </summary>
    public LayoutLength Length { get; }

    /// <summary>
    /// Gets the outer margin applied to the slot.
    /// </summary>
    public Thickness Margin { get; }
}
