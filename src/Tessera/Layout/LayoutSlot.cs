using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Tessera.Components.Composition;
using Tessera.Controls;

namespace Tessera.Layout;

/// <summary>
///     Associates content with a sizing rule and outer margin within a layout container.
/// </summary>
/// <remarks>
///     The sizing rule applies on the parent layout's primary axis. In stack-style layouts, <c>Fill</c> consumes the
///     remaining space as one share, while <c>Weighted</c> divides the remaining space proportionally across siblings.
/// </remarks>
public sealed record LayoutSlot
{
    /// <summary>
    ///     Creates an empty slot for object-initializer assembly.
    /// </summary>
    public LayoutSlot()
    {
    }

    /// <summary>
    ///     Creates a slot for the provided Tessera component.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SetsRequiredMembers]
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

    /// <summary>
    ///     Creates a slot for the provided control.
    /// </summary>
    /// <param name="control">The control to place in the slot.</param>
    /// <param name="length">The primary-axis sizing rule.</param>
    /// <param name="margin">The outer margin applied around the slot content.</param>
    [SetsRequiredMembers]
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

    /// <summary>
    ///     Creates a slot for the provided content.
    /// </summary>
    /// <param name="content">The layout content.</param>
    /// <param name="length">The primary-axis sizing rule.</param>
    /// <param name="margin">The outer margin applied around the slot content.</param>
    [SetsRequiredMembers]
    public LayoutSlot(LayoutNode content, LayoutLength length, Thickness margin = default)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Length = length;
        Margin = margin;
    }

    /// <summary>
    ///     Gets the slot content.
    /// </summary>
    public required LayoutNode Content { get; init; }

    /// <summary>
    ///     Gets the primary-axis sizing rule.
    /// </summary>
    public required LayoutLength Length { get; init; }

    /// <summary>
    ///     Gets the outer margin applied to the slot.
    /// </summary>
    public Thickness Margin { get; init; }

    /// <summary>
    ///     Creates an auto-sized slot for layout content.
    /// </summary>
    /// <param name="content">The content placed in the slot.</param>
    /// <param name="margin">The margin applied around the slot.</param>
    public static LayoutSlot Auto(LayoutNode content, Thickness margin = default)
    {
        return new LayoutSlot(content, LayoutLength.Auto(), margin);
    }

    /// <summary>
    ///     Creates an auto-sized slot for a raw advanced component.
    /// </summary>
    /// <remarks>
    ///     Raw <see cref="ICanvasComponent" /> interop is render-only. Use <see cref="Control" /> when the content needs
    ///     focus or input handling.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static LayoutSlot Auto(ICanvasComponent component, Thickness margin = default)
    {
        return new LayoutSlot(component, LayoutLength.Auto(), margin);
    }

    /// <summary>
    ///     Creates an auto-sized slot for a control.
    /// </summary>
    /// <param name="control">The control placed in the slot.</param>
    /// <param name="margin">The margin applied around the slot.</param>
    public static LayoutSlot Auto(Control control, Thickness margin = default)
    {
        return new LayoutSlot(control, LayoutLength.Auto(), margin);
    }

    /// <summary>
    ///     Creates a fixed-size slot for layout content.
    /// </summary>
    /// <param name="content">The content placed in the slot.</param>
    /// <param name="size">The fixed primary-axis size.</param>
    /// <param name="margin">The margin applied around the slot.</param>
    public static LayoutSlot Fixed(LayoutNode content, int size, Thickness margin = default)
    {
        return new LayoutSlot(content, LayoutLength.Fixed(size), margin);
    }

    /// <summary>
    ///     Creates a fixed-size slot for a raw advanced component.
    /// </summary>
    /// <remarks>
    ///     Raw <see cref="ICanvasComponent" /> interop is render-only. Use <see cref="Control" /> when the content needs
    ///     focus or input handling.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static LayoutSlot Fixed(ICanvasComponent component, int size, Thickness margin = default)
    {
        return new LayoutSlot(component, LayoutLength.Fixed(size), margin);
    }

    /// <summary>
    ///     Creates a fixed-size slot for a control.
    /// </summary>
    /// <param name="control">The control placed in the slot.</param>
    /// <param name="size">The fixed primary-axis size.</param>
    /// <param name="margin">The margin applied around the slot.</param>
    public static LayoutSlot Fixed(Control control, int size, Thickness margin = default)
    {
        return new LayoutSlot(control, LayoutLength.Fixed(size), margin);
    }

    /// <summary>
    ///     Creates a fill slot for layout content.
    /// </summary>
    /// <param name="content">The content placed in the slot.</param>
    /// <param name="margin">The margin applied around the slot.</param>
    /// <remarks>
    ///     Use this when the slot should take one share of the remaining space on the parent layout's primary axis.
    /// </remarks>
    public static LayoutSlot Fill(LayoutNode content, Thickness margin = default)
    {
        return new LayoutSlot(content, LayoutLength.Fill(), margin);
    }

    /// <summary>
    ///     Creates a fill slot for a raw advanced component.
    /// </summary>
    /// <remarks>
    ///     Raw <see cref="ICanvasComponent" /> interop is render-only. Use <see cref="Control" /> when the content needs
    ///     focus or input handling.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static LayoutSlot Fill(ICanvasComponent component, Thickness margin = default)
    {
        return new LayoutSlot(component, LayoutLength.Fill(), margin);
    }

    /// <summary>
    ///     Creates a fill slot for a control.
    /// </summary>
    /// <param name="control">The control placed in the slot.</param>
    /// <param name="margin">The margin applied around the slot.</param>
    /// <remarks>
    ///     Use this when the control should take one share of the remaining space on the parent layout's primary axis.
    /// </remarks>
    public static LayoutSlot Fill(Control control, Thickness margin = default)
    {
        return new LayoutSlot(control, LayoutLength.Fill(), margin);
    }

    /// <summary>
    ///     Creates a weighted slot for layout content.
    /// </summary>
    /// <param name="content">The content placed in the slot.</param>
    /// <param name="weight">The relative fill weight.</param>
    /// <param name="margin">The margin applied around the slot.</param>
    /// <remarks>
    ///     Use this when remaining primary-axis space should be divided proportionally across multiple weighted siblings.
    /// </remarks>
    public static LayoutSlot Weighted(LayoutNode content, int weight, Thickness margin = default)
    {
        return new LayoutSlot(content, LayoutLength.Weighted(weight), margin);
    }

    /// <summary>
    ///     Creates a weighted slot for a raw advanced component.
    /// </summary>
    /// <remarks>
    ///     Raw <see cref="ICanvasComponent" /> interop is render-only. Use <see cref="Control" /> when the content needs
    ///     focus or input handling.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static LayoutSlot Weighted(ICanvasComponent component, int weight, Thickness margin = default)
    {
        return new LayoutSlot(component, LayoutLength.Weighted(weight), margin);
    }

    /// <summary>
    ///     Creates a weighted slot for a control.
    /// </summary>
    /// <param name="control">The control placed in the slot.</param>
    /// <param name="weight">The relative fill weight.</param>
    /// <param name="margin">The margin applied around the slot.</param>
    /// <remarks>
    ///     Use this when remaining primary-axis space should be divided proportionally across multiple weighted siblings.
    /// </remarks>
    public static LayoutSlot Weighted(Control control, int weight, Thickness margin = default)
    {
        return new LayoutSlot(control, LayoutLength.Weighted(weight), margin);
    }
}
