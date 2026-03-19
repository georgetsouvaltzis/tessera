namespace TeaSharp.Controls;

/// <summary>
/// Provides old/new state when a <see cref="Toolbar"/> selection changes.
/// </summary>
public sealed class ToolbarSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new toolbar selection-change payload.
    /// </summary>
    /// <param name="previousIndex">The selected index before the change, or <c>-1</c> when none was selected.</param>
    /// <param name="selectedIndex">The selected index after the change, or <c>-1</c> when none is selected.</param>
    /// <param name="previousItem">The selected item before the change.</param>
    /// <param name="selectedItem">The selected item after the change.</param>
    public ToolbarSelectionChangedEventArgs(int previousIndex, int selectedIndex, ToolbarItem? previousItem, ToolbarItem? selectedItem)
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
    public ToolbarItem? PreviousItem { get; }

    /// <summary>
    /// Gets the selected item after the change.
    /// </summary>
    public ToolbarItem? SelectedItem { get; }
}
