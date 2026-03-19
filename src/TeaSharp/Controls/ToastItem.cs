namespace TeaSharp.Controls;

/// <summary>
/// Represents one toast entry tracked by <see cref="ToastCenter" />.
/// </summary>
/// <param name="Id">Stable toast identifier.</param>
/// <param name="Message">Toast message text.</param>
/// <param name="Level">Toast severity level.</param>
/// <param name="CreatedAtUtc">UTC timestamp when the toast was created.</param>
/// <param name="Timeout">Optional timeout metadata for expiration.</param>
/// <param name="IsMuted"><see langword="true" /> when the toast should render as muted.</param>
public sealed record ToastItem(
    string Id,
    string Message,
    NotificationLevel Level,
    DateTimeOffset CreatedAtUtc,
    TimeSpan? Timeout = null,
    bool IsMuted = false);
