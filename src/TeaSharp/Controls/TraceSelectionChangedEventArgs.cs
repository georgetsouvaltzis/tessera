namespace TeaSharp.Controls;

/// <summary>
/// Provides previous/current state when <see cref="TraceViewer" /> selection changes.
/// </summary>
public sealed class TraceSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes selection payload.
    /// </summary>
    /// <param name="previousIndex">Selection index before change.</param>
    /// <param name="selectedIndex">Selection index after change.</param>
    /// <param name="previousEntry">Selected entry before change.</param>
    /// <param name="selectedEntry">Selected entry after change.</param>
    public TraceSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        TraceEntry? previousEntry,
        TraceEntry? selectedEntry)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousEntry = previousEntry;
        SelectedEntry = selectedEntry;
    }

    /// <summary>
    /// Gets selection index before change.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets selection index after change.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets selected entry before change.
    /// </summary>
    public TraceEntry? PreviousEntry { get; }

    /// <summary>
    /// Gets selected entry after change.
    /// </summary>
    public TraceEntry? SelectedEntry { get; }
}
