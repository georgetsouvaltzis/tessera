namespace Tessera.Controls;

/// <summary>
/// Defines severity for a <see cref="LogEntry"/>.
/// </summary>
public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4,
    Critical = 5,
}

/// <summary>
/// Represents one log row displayed by <see cref="LogTailPanel"/>.
/// </summary>
public sealed class LogEntry
{
    /// <summary>
    /// Initializes a new log entry.
    /// </summary>
    /// <param name="message">Log message text.</param>
    /// <param name="level">Log severity.</param>
    /// <param name="timestamp">Optional timestamp. Defaults to <see cref="DateTimeOffset.UtcNow"/>.</param>
    /// <param name="source">Optional source label.</param>
    public LogEntry(string message, LogLevel level = LogLevel.Info, DateTimeOffset? timestamp = null, string? source = null)
    {
        Message = message ?? string.Empty;
        Level = level;
        Timestamp = timestamp ?? DateTimeOffset.UtcNow;
        Source = source ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets log timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets log source label.
    /// </summary>
    public string Source
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets log severity.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets log message.
    /// </summary>
    public string Message
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets whether the row should be treated as muted.
    /// </summary>
    public bool IsMuted { get; set; }

    /// <summary>
    /// Gets or sets whether this row should render with error emphasis.
    /// </summary>
    public bool HasError { get; set; }
}
