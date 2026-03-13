namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Describes a string-option selection transition.
/// </summary>
internal sealed class OptionSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new option-selection payload.
    /// </summary>
    /// <param name="previousIndex">The previously selected index.</param>
    /// <param name="selectedIndex">The current selected index.</param>
    /// <param name="previousItem">The previously selected item.</param>
    /// <param name="selectedItem">The current selected item.</param>
    public OptionSelectionChangedEventArgs(int previousIndex, int selectedIndex, string previousItem, string selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem ?? string.Empty;
        SelectedItem = selectedItem ?? string.Empty;
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
    public string PreviousItem { get; }

    /// <summary>
    /// Gets the current selected item.
    /// </summary>
    public string SelectedItem { get; }
}
