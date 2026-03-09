using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record TabsOptions(
    IEnumerable<string> Tabs,
    bool Focused = false,
    bool EnableNumericShortcuts = true,
    KeyBinding? NextTabKey = null,
    KeyBinding? PreviousTabKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
