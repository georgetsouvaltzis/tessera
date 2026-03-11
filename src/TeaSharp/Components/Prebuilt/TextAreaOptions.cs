using TeaSharp.Widgets;

namespace TeaSharp.Components;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TextAreaComponent"/>.
/// </summary>
public sealed record TextAreaOptions(
    string Title = "Text Area",
    string InitialValue = "",
    bool Focused = false,
    bool ShowBorder = true,
    bool ShowLineNumbers = false,
    bool Wrap = false,
    TextInputKeyMap? InputKeyMap = null,
    ViewportKeyMap? ViewportKeyMap = null);
