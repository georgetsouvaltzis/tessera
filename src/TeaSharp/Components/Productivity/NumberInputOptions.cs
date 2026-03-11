using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="NumberInputComponent"/>.
/// </summary>
public sealed record NumberInputOptions(
    string Title = "Number Input",
    double InitialValue = 0.0,
    bool IsFocused = false,
    bool IsDisabled = false,
    bool IsReadOnly = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    double Min = 0.0,
    double Max = 100.0,
    double Step = 1.0,
    int Precision = 2,
    TextInputKeyMap? InputKeyMap = null,
    KeyBinding? IncreaseKey = null,
    KeyBinding? DecreaseKey = null,
    KeyBinding? SubmitKey = null);
