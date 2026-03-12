namespace TeaSharp.Controls;

public sealed class ContextMenuItemExecutedEventArgs : EventArgs
{
    public ContextMenuItemExecutedEventArgs(ContextMenuItem item)
    {
        Item = item;
    }

    public ContextMenuItem Item { get; }

    public string ItemId => Item.Id;
}
