namespace Tessera.Controls;

/// <summary>
/// Provides the selected string item for choice-style controls.
/// </summary>
public sealed class SelectionChangedEventArgs : EventArgs
{
    public SelectionChangedEventArgs(int previousIndex, int selectedIndex, string previousItem, string selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem ?? string.Empty;
        SelectedItem = selectedItem ?? string.Empty;
    }

    public int PreviousIndex { get; }

    public int SelectedIndex { get; }

    public string PreviousItem { get; }

    public string SelectedItem { get; }
}
