using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Associates content with a sizing rule and outer margin within a layout container.
/// </summary>
public sealed record LayoutSlot
{
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
