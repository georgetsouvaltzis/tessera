namespace Tessera.Controls;

/// <summary>
///     Defines item category for <see cref="ActivityFeedItem" />.
/// </summary>
public enum ActivityFeedItemKind
{
    /// <summary>
    ///     The informational state.
    /// </summary>
    Info = 0,

    /// <summary>
    ///     The success state.
    /// </summary>
    Success = 1,

    /// <summary>
    ///     The warning state.
    /// </summary>
    Warning = 2,

    /// <summary>
    ///     The error state.
    /// </summary>
    Error = 3
}

/// <summary>
///     Represents one timeline row in <see cref="ActivityFeed" />.
/// </summary>
public sealed class ActivityFeedItem
{
    /// <summary>
    ///     Initializes a new activity item.
    /// </summary>
    /// <param name="actor">Actor that produced the activity.</param>
    /// <param name="action">Action verb for the event.</param>
    /// <param name="target">Optional target object.</param>
    /// <param name="details">Optional detail text.</param>
    /// <param name="kind">Category used for row styling.</param>
    /// <param name="timestamp">Optional timestamp. Defaults to <see cref="DateTimeOffset.UtcNow" />.</param>
    public ActivityFeedItem(
        string actor,
        string action,
        string? target = null,
        string? details = null,
        ActivityFeedItemKind kind = ActivityFeedItemKind.Info,
        DateTimeOffset? timestamp = null)
    {
        Actor = actor;
        Action = action;
        Target = target ?? string.Empty;
        Details = details ?? string.Empty;
        Kind = kind;
        Timestamp = timestamp ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Gets or sets event timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    ///     Gets or sets actor name.
    /// </summary>
    public string Actor { get; set; }

    /// <summary>
    ///     Gets or sets action text.
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    ///     Gets or sets target text.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    ///     Gets or sets additional details.
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    ///     Gets or sets category used by row-style mapping.
    /// </summary>
    public ActivityFeedItemKind Kind { get; set; }

    /// <summary>
    ///     Gets or sets whether the item should be emphasized as unread.
    /// </summary>
    public bool IsUnread { get; set; } = true;

    /// <summary>
    ///     Gets or sets whether the item is muted.
    /// </summary>
    public bool IsMuted { get; set; }

    /// <summary>
    ///     Gets or sets whether the item has error state.
    /// </summary>
    public bool HasError { get; set; }
}
