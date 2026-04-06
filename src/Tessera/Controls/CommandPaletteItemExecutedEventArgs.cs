namespace Tessera.Controls;

/// <summary>
/// Provides the executed <see cref="CommandPaletteItem"/> for a command palette action.
/// </summary>
public sealed class CommandPaletteItemExecutedEventArgs : EventArgs
{
    public CommandPaletteItemExecutedEventArgs(CommandPaletteItem item)
    {
        Item = item;
    }

    public CommandPaletteItem Item { get; }

    public string ItemId => Item.Id;
}
