namespace Tessera.Controls;

/// <summary>
/// Represents one scheduled time entry in a <see cref="SchedulerTimeline" />.
/// </summary>
public sealed class SchedulerEntry
{
    /// <summary>
    /// Initializes a scheduler entry.
    /// </summary>
    /// <param name="id">Stable entry identifier.</param>
    /// <param name="title">Primary entry title text.</param>
    /// <param name="start">Entry start timestamp (inclusive).</param>
    /// <param name="end">Entry end timestamp (exclusive).</param>
    /// <param name="details">Optional secondary details text.</param>
    /// <param name="isMuted"><see langword="true" /> when the row should render muted.</param>
    public SchedulerEntry(
        string id,
        string title,
        DateTimeOffset start,
        DateTimeOffset end,
        string? details = null,
        bool isMuted = false)
    {
        Id = id ?? string.Empty;
        Title = title ?? string.Empty;
        Start = start;
        End = end < start ? start : end;
        Details = details ?? string.Empty;
        IsMuted = isMuted;
    }

    /// <summary>
    /// Gets the stable entry identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets or sets the primary entry title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets entry start timestamp (inclusive).
    /// </summary>
    public DateTimeOffset Start { get; set; }

    /// <summary>
    /// Gets or sets entry end timestamp (exclusive).
    /// </summary>
    public DateTimeOffset End { get; set; }

    /// <summary>
    /// Gets or sets optional secondary details text.
    /// </summary>
    public string Details
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets whether this entry should render muted.
    /// </summary>
    public bool IsMuted { get; set; }
}
