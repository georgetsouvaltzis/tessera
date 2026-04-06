namespace Tessera.Controls;

/// <summary>
/// Represents one navigation item rendered by <see cref="SideNavRail" />.
/// </summary>
public sealed record NavItem
{
    /// <summary>
    /// Initializes a new navigation item.
    /// </summary>
    /// <param name="id">Stable item identifier.</param>
    /// <param name="label">Display label.</param>
    /// <param name="icon">Optional icon or short glyph text.</param>
    /// <param name="badge">Optional badge text.</param>
    /// <param name="isDisabled"><see langword="true"/> when the item is disabled.</param>
    public NavItem(
        string id,
        string label,
        string? icon = null,
        string? badge = null,
        bool isDisabled = false)
    {
        Id = id ?? string.Empty;
        Label = label ?? string.Empty;
        Icon = icon;
        Badge = badge;
        IsDisabled = isDisabled;
    }

    /// <summary>
    /// Gets the stable item identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the display label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the optional icon text.
    /// </summary>
    public string? Icon { get; }

    /// <summary>
    /// Gets the optional badge text.
    /// </summary>
    public string? Badge { get; }

    /// <summary>
    /// Gets a value indicating whether the item is disabled.
    /// </summary>
    public bool IsDisabled { get; }
}
