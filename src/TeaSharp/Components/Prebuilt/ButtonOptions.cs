using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="ButtonComponent"/>.
/// </summary>
public sealed record ButtonOptions(
    string Label = "Button",
    string? Description = null,
    bool Focused = false,
    bool Enabled = true,
    bool ShowBorder = false,
    WidgetInteractionProfile? InteractionProfile = null);
