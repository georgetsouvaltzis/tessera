namespace Tessera.Controls;

/// <summary>
///     Provides previous and current selection details for <see cref="SideNavRail.SelectionChanged" />.
/// </summary>
public sealed class SideNavRailSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes new event args.
    /// </summary>
    /// <param name="previousIndex">Previous selected index.</param>
    /// <param name="selectedIndex">Current selected index.</param>
    /// <param name="previousItem">Previous selected item.</param>
    /// <param name="selectedItem">Current selected item.</param>
    public SideNavRailSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        NavItem? previousItem,
        NavItem? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    /// <summary>
    ///     Gets the previous selected index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    ///     Gets the current selected index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    ///     Gets the previous selected item.
    /// </summary>
    public NavItem? PreviousItem { get; }

    /// <summary>
    ///     Gets the current selected item.
    /// </summary>
    public NavItem? SelectedItem { get; }
}
