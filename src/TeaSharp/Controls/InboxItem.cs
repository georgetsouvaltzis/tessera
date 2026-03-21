namespace TeaSharp.Controls;

/// <summary>
/// Represents one persistent notification entry in a <see cref="NotificationInbox" />.
/// </summary>
public sealed class InboxItem
{
    /// <summary>
    /// Initializes an inbox item.
    /// </summary>
    /// <param name="id">Stable item identifier.</param>
    /// <param name="message">Primary message text.</param>
    /// <param name="level">Semantic severity.</param>
    /// <param name="createdAt">Creation timestamp.</param>
    /// <param name="source">Optional source/category text.</param>
    /// <param name="isRead"><see langword="true" /> when already read.</param>
    /// <param name="isPinned"><see langword="true" /> when pinned.</param>
    public InboxItem(
        string id,
        string message,
        NotificationLevel level,
        DateTimeOffset createdAt,
        string? source = null,
        bool isRead = false,
        bool isPinned = false)
    {
        Id = id ?? string.Empty;
        Message = message ?? string.Empty;
        Level = level;
        CreatedAt = createdAt;
        Source = source ?? string.Empty;
        IsRead = isRead;
        IsPinned = isPinned;
    }

    /// <summary>
    /// Gets stable item identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets or sets primary message text.
    /// </summary>
    public string Message
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets semantic severity.
    /// </summary>
    public NotificationLevel Level { get; set; }

    /// <summary>
    /// Gets or sets creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets optional source/category text.
    /// </summary>
    public string Source
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets whether item is read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets whether item is pinned.
    /// </summary>
    public bool IsPinned { get; set; }
}
