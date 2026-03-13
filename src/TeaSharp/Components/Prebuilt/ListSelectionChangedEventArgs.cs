using System.ComponentModel;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Describes a list selection transition.
/// </summary>
/// <typeparam name="T">The list item type.</typeparam>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ListSelectionChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Initializes a new list-selection payload.
    /// </summary>
    /// <param name="previousIndex">The previously selected index.</param>
    /// <param name="selectedIndex">The current selected index.</param>
    /// <param name="previousItem">The previously selected item.</param>
    /// <param name="selectedItem">The currently selected item.</param>
    public ListSelectionChangedEventArgs(int previousIndex, int selectedIndex, T? previousItem, T? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    /// <summary>
    /// Gets the previously selected index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets the current selected index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets the previously selected item.
    /// </summary>
    public T? PreviousItem { get; }

    /// <summary>
    /// Gets the current selected item.
    /// </summary>
    public T? SelectedItem { get; }
}
