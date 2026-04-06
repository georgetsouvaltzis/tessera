namespace Tessera.Controls;

/// <summary>
/// Represents one command exposed by a <see cref="CommandPalette"/>.
/// </summary>
public sealed record CommandPaletteItem(string Id, string Title, string Description = "");
