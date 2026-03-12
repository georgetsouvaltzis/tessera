namespace TeaSharp.Controls;

public readonly record struct AccordionSection(string Title, IReadOnlyList<string> BodyLines, bool Expanded = false);
