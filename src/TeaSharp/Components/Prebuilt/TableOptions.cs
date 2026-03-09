namespace TeaSharp.Components;

public sealed record TableOptions(
    IReadOnlyList<string> Headers,
    string Title = "Table",
    bool Focused = false,
    bool ShowBorder = true,
    int? PageSize = null);
