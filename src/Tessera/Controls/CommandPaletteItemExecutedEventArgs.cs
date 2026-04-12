namespace Tessera.Controls;

/// <summary>
/// Provides the executed <see cref="CommandPaletteItem"/> for a command palette action.
/// </summary>
public sealed class CommandPaletteItemExecutedEventArgs : EventArgs
{
    /// <summary>
    /// Executes command palette item executed event args.
    /// </summary>
    /// <param name="item">The item value.</param>
    /// <returns>The result of command palette item executed event args.</returns>
    public CommandPaletteItemExecutedEventArgs(CommandPaletteItem item)
    {
        Item = item;
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    public CommandPaletteItem Item { get; }

    /// <summary>
    /// Represents item id.
    /// </summary>
    public string ItemId => Item.Id;
}
