namespace TeaSharp.Components.Productivity;

/// <summary>
/// Describes a context-menu item execution.
/// </summary>
public sealed class ContextMenuItemExecutedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new execution payload for a context-menu item.
    /// </summary>
    /// <param name="item">The executed item.</param>
    public ContextMenuItemExecutedEventArgs(ContextMenuItem item)
    {
        Item = item;
    }

    /// <summary>
    /// Gets the executed menu item.
    /// </summary>
    public ContextMenuItem Item { get; }

    /// <summary>
    /// Gets the executed item identifier.
    /// </summary>
    public string ItemId => Item.Id;
}
