namespace Tessera.Controls;

/// <summary>
///     Represents one section within an <see cref="Accordion" />.
/// </summary>
public readonly record struct AccordionSection(string Title, IReadOnlyList<string> BodyLines, bool Expanded = false);
