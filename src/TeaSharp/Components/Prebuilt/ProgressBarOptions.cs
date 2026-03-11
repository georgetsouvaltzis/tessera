using TeaSharp.Widgets;

namespace TeaSharp.Components;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="ProgressBarComponent"/>.
/// </summary>
public sealed record ProgressBarOptions(
    string Title = "Progress",
    double InitialValue = 0.0,
    bool Focused = false,
    bool ShowBorder = true,
    double Step = 0.05,
    KeyBinding? DecreaseKey = null,
    KeyBinding? IncreaseKey = null);
