namespace Tessera.Controls;

/// <summary>
/// Provides the activated <see cref="CommandBarItem"/> when a command-bar action is triggered.
/// </summary>
public sealed class CommandBarItemActivatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new activation payload.
    /// </summary>
    /// <param name="item">The activated command item.</param>
    public CommandBarItemActivatedEventArgs(CommandBarItem item)
    {
        Item = item;
    }

    /// <summary>
    /// Gets the activated command item.
    /// </summary>
    public CommandBarItem Item { get; }

    /// <summary>
    /// Gets the activated command identifier.
    /// </summary>
    public string ItemId => Item.Id;
}
