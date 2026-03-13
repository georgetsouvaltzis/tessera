namespace TeaSharp.Controls;

/// <summary>
/// Provides the executed <see cref="ContextMenuItem"/> for a context menu action.
/// </summary>
public sealed class ContextMenuItemExecutedEventArgs : EventArgs
{
    public ContextMenuItemExecutedEventArgs(ContextMenuItem item)
    {
        Item = item;
    }

    public ContextMenuItem Item { get; }

    public string ItemId => Item.Id;
}
