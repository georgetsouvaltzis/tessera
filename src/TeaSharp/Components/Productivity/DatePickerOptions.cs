using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="DatePickerComponent"/>.
/// </summary>
public sealed record DatePickerOptions(
    string Title = "Date Picker",
    DateOnly? InitialDate = null,
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    bool ShowBorder = true,
    KeyBinding? PreviousDayKey = null,
    KeyBinding? NextDayKey = null,
    KeyBinding? PreviousWeekKey = null,
    KeyBinding? NextWeekKey = null,
    KeyBinding? PreviousMonthKey = null,
    KeyBinding? NextMonthKey = null,
    KeyBinding? CommitKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
