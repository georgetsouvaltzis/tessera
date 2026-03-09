using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record TextInputOptions(
    string Title = "Text Input",
    string Placeholder = "",
    string InitialValue = "",
    int MaxLength = 512,
    bool Focused = false,
    bool ShowBorder = true,
    bool ClearOnSubmit = false,
    bool ClearOnCancel = false,
    bool MaskInput = false,
    char MaskCharacter = '*',
    TextInputKeyMap? KeyMap = null,
    KeyBinding? CancelKey = null);
