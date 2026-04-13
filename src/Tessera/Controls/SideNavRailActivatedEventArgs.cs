namespace Tessera.Controls;

/// <summary>
///     Provides activation details for <see cref="SideNavRail.Activated" />.
/// </summary>
public sealed class SideNavRailActivatedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes new event args.
    /// </summary>
    /// <param name="selectedIndex">Activated item index.</param>
    /// <param name="selectedItem">Activated item.</param>
    public SideNavRailActivatedEventArgs(int selectedIndex, NavItem selectedItem)
    {
        SelectedIndex = selectedIndex;
        SelectedItem = selectedItem ?? throw new ArgumentNullException(nameof(selectedItem));
    }

    /// <summary>
    ///     Gets activated item index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    ///     Gets activated item.
    /// </summary>
    public NavItem SelectedItem { get; }
}
