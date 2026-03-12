using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Components.Composition;

namespace TeaSharp.Layout;

/// <summary>
/// Associates content with a sizing rule and outer margin within a layout container.
/// </summary>
public sealed record LayoutSlot
{
    /// <summary>
    /// Creates a slot for the provided TeaSharp component.
    /// </summary>
    public LayoutSlot(
        ICanvasComponent component,
        LayoutLength length,
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
