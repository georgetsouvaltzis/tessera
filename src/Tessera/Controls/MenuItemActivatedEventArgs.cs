namespace Tessera.Controls;

/// <summary>
/// Provides the activated <see cref="MenuItem"/> for a menu action.
/// </summary>
public sealed class MenuItemActivatedEventArgs : EventArgs
{
    /// <summary>
    /// Executes menu item activated event args.
    /// </summary>
    /// <param name="item">The item value.</param>
    /// <returns>The result of menu item activated event args.</returns>
    public MenuItemActivatedEventArgs(MenuItem item)
    {
        Item = item;
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    public MenuItem Item { get; }

    /// <summary>
    /// Represents item id.
    /// </summary>
    public string ItemId => Item.Id;
}
