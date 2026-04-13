namespace Tessera.Controls;

/// <summary>
///     Represents one entry shown by <see cref="QuickOpenOverlay" />.
/// </summary>
/// <param name="Id">Stable item identifier.</param>
/// <param name="Label">Primary item label rendered in the row.</param>
/// <param name="Description">Optional secondary description rendered after <paramref name="Label" />.</param>
public sealed record QuickOpenItem(string Id, string Label, string Description = "");
