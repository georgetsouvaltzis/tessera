namespace Tessera.Controls;

/// <summary>
///     Represents an item rendered by <see cref="JumpList" />.
/// </summary>
public sealed record JumpListItem
{
    /// <summary>
    ///     Initializes a new jump-list item.
    /// </summary>
    /// <param name="id">Stable item identifier.</param>
    /// <param name="label">Primary item label.</param>
    /// <param name="isPinned">Whether the item is pinned.</param>
    /// <param name="isRecent">Whether the item is recent.</param>
    /// <param name="isDisabled">Whether the item is disabled.</param>
    /// <exception cref="ArgumentException"><paramref name="id" /> is empty or whitespace.</exception>
    public JumpListItem(
        string id,
        string label,
        bool isPinned = false,
        bool isRecent = false,
        bool isDisabled = false)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Jump list item id must be non-empty.", nameof(id));
        }

        Id = id;
        Label = label;
        IsPinned = isPinned;
        IsRecent = isRecent;
        IsDisabled = isDisabled;
    }

    /// <summary>
    ///     Gets item identifier.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    ///     Gets item label.
    /// </summary>
    public string Label { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the item is pinned.
    /// </summary>
    public bool IsPinned { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the item is recent.
    /// </summary>
    public bool IsRecent { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the item is disabled.
    /// </summary>
    public bool IsDisabled { get; init; }
}
