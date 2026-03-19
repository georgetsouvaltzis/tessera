namespace TeaSharp.Controls;

/// <summary>
/// Represents one row in a <see cref="Timeline" />.
/// </summary>
public sealed class TimelineEntry
{
    /// <summary>
    /// Initializes a timeline row.
    /// </summary>
    /// <param name="id">Stable row identifier.</param>
    /// <param name="label">Primary label text.</param>
    /// <param name="timestampText">Timestamp or temporal text shown for this row.</param>
    /// <param name="content">Optional additional content text.</param>
    /// <param name="status">Optional status text.</param>
    /// <param name="isMuted"><see langword="true" /> when the row should render muted.</param>
    public TimelineEntry(
        string id,
        string label,
        string timestampText,
        string? content = null,
        string? status = null,
        bool isMuted = false)
    {
        Id = id ?? string.Empty;
        Label = label ?? string.Empty;
        TimestampText = timestampText ?? string.Empty;
        Content = content ?? string.Empty;
        Status = status ?? string.Empty;
        IsMuted = isMuted;
    }

    /// <summary>
    /// Gets the stable row identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets or sets the primary label text.
    /// </summary>
    public string Label
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets timestamp or temporal text shown for this row.
    /// </summary>
    public string TimestampText
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets optional additional content text.
    /// </summary>
    public string Content
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets optional status text.
    /// </summary>
    public string Status
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this row should render muted.
    /// </summary>
    public bool IsMuted { get; set; }
}
