namespace TeaSharp.Controls;

public sealed class CommandPaletteItemExecutedEventArgs : EventArgs
{
    public CommandPaletteItemExecutedEventArgs(CommandPaletteItem item)
    {
        Item = item;
    }

    public CommandPaletteItem Item { get; }

    public string ItemId => Item.Id;
}
