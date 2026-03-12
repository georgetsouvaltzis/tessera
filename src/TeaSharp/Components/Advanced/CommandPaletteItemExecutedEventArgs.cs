using System.ComponentModel;

namespace TeaSharp.Components.Advanced;

/// <summary>
/// Describes a command-palette item execution.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class CommandPaletteItemExecutedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new execution payload for a command-palette item.
    /// </summary>
    /// <param name="item">The executed command item.</param>
    public CommandPaletteItemExecutedEventArgs(CommandPaletteItem item)
    {
        Item = item;
    }

    /// <summary>
    /// Gets the executed command item.
    /// </summary>
    public CommandPaletteItem Item { get; }

    /// <summary>
    /// Gets the executed command identifier.
    /// </summary>
    public string ItemId => Item.Id;
}
