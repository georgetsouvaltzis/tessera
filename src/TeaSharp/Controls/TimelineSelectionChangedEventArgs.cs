namespace TeaSharp.Controls;

/// <summary>
/// Provides old/new state when <see cref="Timeline" /> selection changes.
/// </summary>
public sealed class TimelineSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a timeline selection-change payload.
    /// </summary>
    /// <param name="previousIndex">The selected index before the change, or <c>-1</c> when none was selected.</param>
    /// <param name="selectedIndex">The selected index after the change, or <c>-1</c> when none is selected.</param>
    /// <param name="previousItem">The selected item before the change.</param>
    /// <param name="selectedItem">The selected item after the change.</param>
    public TimelineSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        TimelineEntry? previousItem,
        TimelineEntry? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
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
    /// Gets the selected item before the change.
    /// </summary>
    public TimelineEntry? PreviousItem { get; }

    /// <summary>
    /// Gets the selected item after the change.
    /// </summary>
    public TimelineEntry? SelectedItem { get; }
}
