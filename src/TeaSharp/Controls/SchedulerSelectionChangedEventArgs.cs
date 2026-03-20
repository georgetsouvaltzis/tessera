namespace TeaSharp.Controls;

/// <summary>
/// Provides old/new state when <see cref="SchedulerTimeline" /> selection changes.
/// </summary>
public sealed class SchedulerSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a scheduler selection-change payload.
    /// </summary>
    /// <param name="previousIndex">The selected index before the change, or <c>-1</c> when none was selected.</param>
    /// <param name="selectedIndex">The selected index after the change, or <c>-1</c> when none is selected.</param>
    /// <param name="previousEntry">The selected entry before the change.</param>
    /// <param name="selectedEntry">The selected entry after the change.</param>
    public SchedulerSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        SchedulerEntry? previousEntry,
        SchedulerEntry? selectedEntry)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousEntry = previousEntry;
        SelectedEntry = selectedEntry;
    }

    /// <summary>
    /// Gets the selected index before the change.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets the selected index after the change.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets the selected entry before the change.
    /// </summary>
    public SchedulerEntry? PreviousEntry { get; }

    /// <summary>
    /// Gets the selected entry after the change.
    /// </summary>
    public SchedulerEntry? SelectedEntry { get; }
}
