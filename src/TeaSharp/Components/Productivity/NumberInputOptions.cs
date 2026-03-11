using TeaSharp.Widgets;

namespace TeaSharp.Components;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="NumberInputComponent"/>.
/// </summary>
public sealed record NumberInputOptions(
    string Title = "Number Input",
    double InitialValue = 0.0,
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    bool ShowBorder = true,
    double Min = 0.0,
    double Max = 100.0,
    double Step = 1.0,
    int Precision = 2,
    TextInputKeyMap? InputKeyMap = null,
    KeyBinding? IncreaseKey = null,
    KeyBinding? DecreaseKey = null,
    KeyBinding? SubmitKey = null);
