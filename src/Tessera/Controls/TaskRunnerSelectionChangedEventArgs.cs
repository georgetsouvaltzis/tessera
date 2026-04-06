namespace Tessera.Controls;

/// <summary>
/// Provides previous/current selection values for <see cref="TaskRunnerPanel.SelectionChanged"/>.
/// </summary>
public sealed class TaskRunnerSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the selection-change event payload.
    /// </summary>
    /// <param name="previousIndex">Index selected before the change.</param>
    /// <param name="selectedIndex">Index selected after the change.</param>
    /// <param name="previousItem">Item selected before the change.</param>
    /// <param name="selectedItem">Item selected after the change.</param>
    public TaskRunnerSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        TaskRunItem? previousItem,
        TaskRunItem? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    /// <summary>
    /// Gets index selected before the change.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets index selected after the change.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets item selected before the change.
    /// </summary>
    public TaskRunItem? PreviousItem { get; }

    /// <summary>
    /// Gets item selected after the change.
    /// </summary>
    public TaskRunItem? SelectedItem { get; }
}
