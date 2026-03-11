using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TextInputComponent"/>.
/// </summary>
public sealed record TextInputOptions(
    string Title = "Text Input",
    string Placeholder = "",
    string InitialValue = "",
    int MaxLength = 512,
    bool IsFocused = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    bool ClearOnSubmit = false,
    bool ClearOnCancel = false,
    bool MaskInput = false,
    char MaskCharacter = '*',
    TextInputKeyMap? KeyMap = null,
    KeyBinding? CancelKey = null);
