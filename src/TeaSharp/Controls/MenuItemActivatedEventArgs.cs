namespace TeaSharp.Controls;

public sealed class MenuItemActivatedEventArgs : EventArgs
{
    public MenuItemActivatedEventArgs(MenuItem item)
    {
        Item = item;
    }

    public MenuItem Item { get; }

    public string ItemId => Item.Id;
}
