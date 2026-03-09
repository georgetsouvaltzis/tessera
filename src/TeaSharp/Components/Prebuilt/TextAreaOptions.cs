using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record TextAreaOptions(
    string Title = "Text Area",
    string InitialValue = "",
    bool Focused = false,
    bool ShowBorder = true,
    bool ShowLineNumbers = false,
    bool Wrap = false,
    TextInputKeyMap? InputKeyMap = null,
    ViewportKeyMap? ViewportKeyMap = null);
