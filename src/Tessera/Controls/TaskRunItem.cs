namespace Tessera.Controls;

/// <summary>
/// Represents one build/test/deploy task row rendered by <see cref="TaskRunnerPanel"/>.
/// </summary>
public sealed class TaskRunItem
{
    /// <summary>
    /// Initializes a new task-run item.
    /// </summary>
    /// <param name="id">Stable task identifier.</param>
    /// <param name="name">Display name shown in the row body.</param>
    /// <param name="status">Current task status.</param>
    /// <param name="description">Optional description text.</param>
    /// <param name="updatedAt">Optional last-update timestamp. Defaults to <see cref="DateTimeOffset.UtcNow"/>.</param>
    public TaskRunItem(
        string id,
        string name,
        TaskRunStatus status = TaskRunStatus.Queued,
        string? description = null,
        DateTimeOffset? updatedAt = null)
    {
        Id = id ?? string.Empty;
        Name = name ?? string.Empty;
        Status = status;
        Description = description ?? string.Empty;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets or sets the stable task identifier.
    /// </summary>
    public string Id
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets display name shown in the task row.
    /// </summary>
    public string Name
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets optional description text rendered after <see cref="Name"/>.
    /// </summary>
    public string Description
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets current task status.
    /// </summary>
    public TaskRunStatus Status { get; set; }

    /// <summary>
    /// Gets or sets last-update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the row should render with muted emphasis.
    /// </summary>
    public bool IsMuted { get; set; }
}
