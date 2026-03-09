namespace TeaSharp.Components;

public readonly record struct AccordionSection(string Title, IReadOnlyList<string> Lines, bool Expanded = false);
