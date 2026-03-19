namespace TeaSharp.Controls;

/// <summary>
/// Provides selection details when a <see cref="TreeTable"/> row changes.
/// </summary>
public sealed class TreeTableSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new selection payload.
    /// </summary>
    /// <param name="previousIndex">Previously selected visible index.</param>
    /// <param name="selectedIndex">Currently selected visible index.</param>
    /// <param name="previousItem">Previously selected row.</param>
    /// <param name="selectedItem">Currently selected row.</param>
    public TreeTableSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        TreeTableNode? previousItem,
        TreeTableNode? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    /// <summary>
    /// Gets the previously selected visible index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets the currently selected visible index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets the previously selected row.
    /// </summary>
    public TreeTableNode? PreviousItem { get; }

    /// <summary>
    /// Gets the currently selected row.
    /// </summary>
    public TreeTableNode? SelectedItem { get; }
}
