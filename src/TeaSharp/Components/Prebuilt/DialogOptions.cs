using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="DialogComponent"/>.
/// </summary>
public sealed record DialogOptions(
    string Title = "Dialog",
    IReadOnlyList<string>? Lines = null,
    bool Visible = false,
    bool Focused = false,
    BorderStyle BorderStyle = BorderStyle.Rounded,
    UiTheme? Theme = null,
    KeyBinding? AcceptKey = null,
    KeyBinding? DismissKey = null);
