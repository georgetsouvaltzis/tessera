namespace TeaSharp.Controls;

/// <summary>
/// Represents one tab item in <see cref="PaneTabs" />.
/// </summary>
public sealed class PaneTabItem
{
    /// <summary>
    /// Initializes a pane tab item.
    /// </summary>
    /// <param name="id">Stable tab identifier.</param>
    /// <param name="title">Tab title.</param>
    /// <param name="isDisabled"><see langword="true" /> when tab should be non-selectable.</param>
    public PaneTabItem(string id, string title, bool isDisabled = false)
    {
        Id = id ?? string.Empty;
        Title = title ?? string.Empty;
        IsDisabled = isDisabled;
    }

    /// <summary>
    /// Gets stable tab identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets or sets tab title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets whether tab is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Gets or sets whether tab has unsaved changes.
    /// </summary>
    public bool IsDirty { get; set; }
}
