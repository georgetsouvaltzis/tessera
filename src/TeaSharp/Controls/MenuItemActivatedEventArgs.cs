namespace TeaSharp.Controls;

/// <summary>
/// Provides the activated <see cref="MenuItem"/> for a menu action.
/// </summary>
public sealed class MenuItemActivatedEventArgs : EventArgs
{
    public MenuItemActivatedEventArgs(MenuItem item)
    {
        Item = item;
    }

    public MenuItem Item { get; }

    public string ItemId => Item.Id;
}
