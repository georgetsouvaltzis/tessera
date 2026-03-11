using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TimePickerComponent"/>.
/// </summary>
public sealed record TimePickerOptions(
    string Title = "Time Picker",
    TimeOnly? InitialValue = null,
    bool IsFocused = false,
    bool IsDisabled = false,
    bool IsReadOnly = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
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
