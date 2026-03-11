namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TableComponent"/>.
/// </summary>
public sealed record TableOptions(
    IReadOnlyList<string> Headers,
    string Title = "Table",
    bool Focused = false,
    bool ShowBorder = true,
    int? PageSize = null);
