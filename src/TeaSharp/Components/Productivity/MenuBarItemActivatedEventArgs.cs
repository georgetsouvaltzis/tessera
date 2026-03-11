namespace TeaSharp.Components.Productivity;

/// <summary>
/// Describes a menu-bar item activation.
/// </summary>
public sealed class MenuBarItemActivatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new activation payload for a menu-bar item.
    /// </summary>
    /// <param name="item">The activated item.</param>
    public MenuBarItemActivatedEventArgs(MenuBarItem item)
    {
        Item = item;
    }

    /// <summary>
    /// Gets the activated menu item.
    /// </summary>
    public MenuBarItem Item { get; }

    /// <summary>
    /// Gets the activated item identifier.
    /// </summary>
    public string ItemId => Item.Id;
}
