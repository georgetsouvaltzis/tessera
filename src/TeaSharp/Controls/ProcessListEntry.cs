using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents process lifecycle status for <see cref="ProcessListView"/> rows.
/// </summary>
public enum ProcessListStatus
{
    /// <summary>Process is currently running.</summary>
    Running,
    /// <summary>Process is sleeping or waiting.</summary>
    Sleeping,
    /// <summary>Process is stopped.</summary>
    Stopped,
    /// <summary>Process is zombie/defunct.</summary>
    Zombie,
    /// <summary>Process status is unknown.</summary>
    Unknown,
}

/// <summary>
/// Represents one process row rendered by <see cref="ProcessListView"/>.
/// </summary>
public sealed class ProcessListEntry
{
    /// <summary>
    /// Initializes a process row.
    /// </summary>
    /// <param name="pid">Process identifier.</param>
    /// <param name="name">Display name.</param>
    /// <param name="status">Current process status.</param>
    /// <param name="cpuPercent">CPU usage percentage.</param>
    /// <param name="memoryMb">Memory usage in MB.</param>
    public ProcessListEntry(
        int pid,
        string name,
        ProcessListStatus status = ProcessListStatus.Running,
        double cpuPercent = 0,
        double memoryMb = 0)
    {
        Pid = pid;
        Name = name ?? string.Empty;
        Status = status;
        CpuPercent = cpuPercent;
        MemoryMb = memoryMb;
    }

    /// <summary>Gets or sets process identifier.</summary>
    public int Pid { get; set; }

    /// <summary>Gets or sets display name.</summary>
    public string Name
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>Gets or sets process status.</summary>
    public ProcessListStatus Status { get; set; }

    /// <summary>Gets or sets CPU usage percentage.</summary>
    public double CpuPercent { get; set; }

    /// <summary>Gets or sets memory usage in MB.</summary>
    public double MemoryMb { get; set; }

    /// <summary>Gets or sets whether the row should render muted.</summary>
    public bool IsMuted { get; set; }

    /// <summary>Gets or sets row-specific style merged with control-level style.</summary>
    public TeaStyle Style { get; set; } = TeaStyle.Empty;
}

/// <summary>
/// Provides previous/current values when <see cref="ProcessListView.SelectionChanged"/> fires.
/// </summary>
public sealed class ProcessListSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes selection payload.
    /// </summary>
    /// <param name="previousIndex">Previously selected index.</param>
    /// <param name="selectedIndex">Current selected index.</param>
    /// <param name="previousEntry">Previously selected entry.</param>
    /// <param name="selectedEntry">Current selected entry.</param>
    public ProcessListSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        ProcessListEntry? previousEntry,
        ProcessListEntry? selectedEntry)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousEntry = previousEntry;
        SelectedEntry = selectedEntry;
    }

    /// <summary>Gets previously selected index.</summary>
    public int PreviousIndex { get; }

    /// <summary>Gets current selected index.</summary>
    public int SelectedIndex { get; }

    /// <summary>Gets previously selected row.</summary>
    public ProcessListEntry? PreviousEntry { get; }

    /// <summary>Gets current selected row.</summary>
    public ProcessListEntry? SelectedEntry { get; }
}
