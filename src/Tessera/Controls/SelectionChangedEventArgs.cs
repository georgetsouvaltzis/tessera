namespace Tessera.Controls;

/// <summary>
/// Provides the selected string item for choice-style controls.
/// </summary>
public sealed class SelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Executes selection changed event args.
    /// </summary>
    /// <param name="previousIndex">The previous index value.</param>
    /// <param name="selectedIndex">The selected index value.</param>
    /// <param name="previousItem">The previous item value.</param>
    /// <param name="selectedItem">The selected item value.</param>
    /// <returns>The result of selection changed event args.</returns>
    public SelectionChangedEventArgs(int previousIndex, int selectedIndex, string previousItem, string selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem ?? string.Empty;
        SelectedItem = selectedItem ?? string.Empty;
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
    public string PreviousItem { get; }

    /// <summary>
    /// Gets the selected item.
    /// </summary>
    public string SelectedItem { get; }
}
