using TeaSharp.Widgets;

namespace TeaSharp.Components;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TimePickerComponent"/>.
/// </summary>
public sealed record TimePickerOptions(
    string Title = "Time Picker",
    TimeOnly? InitialValue = null,
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    bool ShowBorder = true,
    TimePickerField ActiveField = TimePickerField.Hour,
    int HourStep = 1,
    int MinuteStep = 1,
    int SecondStep = 5,
    KeyBinding? NextFieldKey = null,
    KeyBinding? PreviousFieldKey = null,
    KeyBinding? IncreaseKey = null,
    KeyBinding? DecreaseKey = null,
    KeyBinding? CommitKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
