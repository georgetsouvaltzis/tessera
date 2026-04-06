namespace Tessera.Controls;

/// <summary>
/// Represents one toolbar item.
/// </summary>
/// <param name="Id">The stable item identifier.</param>
/// <param name="Label">The item label shown in the toolbar.</param>
public sealed record ToolbarItem(string Id, string Label);
