namespace TeaSharp.Components.UiKit;

public readonly record struct AccordionSection(string Title, IReadOnlyList<string> Lines, bool Expanded = false);
