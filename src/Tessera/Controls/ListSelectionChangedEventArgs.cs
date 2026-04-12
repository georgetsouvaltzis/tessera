namespace Tessera.Controls;

/// <summary>
/// Provides the selected item for a list-style control.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public sealed class ListSelectionChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Executes list selection changed event args.
    /// </summary>
    /// <param name="previousIndex">The previous index value.</param>
    /// <param name="selectedIndex">The selected index value.</param>
    /// <param name="previousItem">The previous item value.</param>
    /// <param name="selectedItem">The selected item value.</param>
    /// <returns>The result of list selection changed event args.</returns>
    public ListSelectionChangedEventArgs(int previousIndex, int selectedIndex, T? previousItem, T? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    /// <summary>
    /// Gets the previous index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets the selected index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets the previous item.
    /// </summary>
    public T? PreviousItem { get; }

    /// <summary>
    /// Gets the selected item.
    /// </summary>
    public T? SelectedItem { get; }
}
