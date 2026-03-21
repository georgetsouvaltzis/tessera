namespace TeaSharp.Controls;

/// <summary>
/// Represents the lifecycle state of one task row in <see cref="TaskRunnerPanel"/>.
/// </summary>
public enum TaskRunStatus
{
    /// <summary>
    /// The task is queued and has not started.
    /// </summary>
    Queued = 0,

    /// <summary>
    /// The task is currently running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// The task completed successfully.
    /// </summary>
    Succeeded = 2,

    /// <summary>
    /// The task completed with a failure.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// The task was intentionally skipped.
    /// </summary>
    Skipped = 4,

    /// <summary>
    /// The task was canceled before completion.
    /// </summary>
    Canceled = 5,
}
