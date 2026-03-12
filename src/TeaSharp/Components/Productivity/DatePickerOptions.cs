using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="DatePickerComponent"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed record DatePickerOptions(
    string Title = "Date Picker",
    DateOnly? InitialDate = null,
    bool IsFocused = false,
    bool IsDisabled = false,
    bool IsReadOnly = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    KeyBinding? PreviousDayKey = null,
    KeyBinding? NextDayKey = null,
    KeyBinding? PreviousWeekKey = null,
    KeyBinding? NextWeekKey = null,
    KeyBinding? PreviousMonthKey = null,
    KeyBinding? NextMonthKey = null,
    KeyBinding? CommitKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
