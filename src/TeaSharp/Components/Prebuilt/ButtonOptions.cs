using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record ButtonOptions(
    string Label = "Button",
    string? Description = null,
    bool Focused = false,
    bool Enabled = true,
    bool ShowBorder = false,
    WidgetInteractionProfile? InteractionProfile = null);
