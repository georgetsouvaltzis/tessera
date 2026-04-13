namespace Tessera.Controls;

/// <summary>
///     Provides the executed <see cref="ContextMenuItem" /> for a context menu action.
/// </summary>
public sealed class ContextMenuItemExecutedEventArgs : EventArgs
{
    /// <summary>
    ///     Executes context menu item executed event args.
    /// </summary>
    /// <param name="item">The item value.</param>
    /// <returns>The result of context menu item executed event args.</returns>
    public ContextMenuItemExecutedEventArgs(ContextMenuItem item)
    {
        Item = item;
    }

    /// <summary>
    ///     Gets the item.
    /// </summary>
    public ContextMenuItem Item { get; }

    /// <summary>
    ///     Represents item id.
    /// </summary>
    public string ItemId => Item.Id;
}
