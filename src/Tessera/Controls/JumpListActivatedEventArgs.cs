namespace Tessera.Controls;

/// <summary>
/// Provides activation details when <see cref="JumpList.Activated" /> is raised.
/// </summary>
public sealed class JumpListActivatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes activation payload.
    /// </summary>
    /// <param name="selectedIndex">Activated item index.</param>
    /// <param name="selectedItem">Activated item.</param>
    public JumpListActivatedEventArgs(int selectedIndex, JumpListItem selectedItem)
    {
        SelectedIndex = selectedIndex;
        SelectedItem = selectedItem;
    }

    /// <summary>
    /// Gets activated item index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets activated item.
    /// </summary>
    public JumpListItem SelectedItem { get; }
}
