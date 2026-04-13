namespace Tessera.Controls;

/// <summary>
///     Provides old/new state when <see cref="PaneTabs" /> selection changes.
/// </summary>
public sealed class PaneTabSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a selection-change payload.
    /// </summary>
    /// <param name="previousIndex">Selection index before change.</param>
    /// <param name="selectedIndex">Selection index after change.</param>
    /// <param name="previousItem">Selected tab before change.</param>
    /// <param name="selectedItem">Selected tab after change.</param>
    public PaneTabSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        PaneTabItem? previousItem,
        PaneTabItem? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    /// <summary>
    ///     Gets selection index before change.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    ///     Gets selection index after change.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    ///     Gets selected tab before change.
    /// </summary>
    public PaneTabItem? PreviousItem { get; }

    /// <summary>
    ///     Gets selected tab after change.
    /// </summary>
    public PaneTabItem? SelectedItem { get; }
}
