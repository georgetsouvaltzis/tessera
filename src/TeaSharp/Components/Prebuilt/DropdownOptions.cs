using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="DropdownComponent"/>.
/// </summary>
public sealed record DropdownOptions(
    IReadOnlyList<string>? Items = null,
    string Title = "Dropdown",
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    bool ShowBorder = true,
    int MaxVisibleItems = 6,
    WidgetInteractionProfile? InteractionProfile = null,
    KeyBinding? ToggleOpenKey = null,
    KeyBinding? OpenKey = null,
    KeyBinding? CloseKey = null,
    KeyBinding? NextItemKey = null,
    KeyBinding? PreviousItemKey = null,
    KeyBinding? ConfirmSelectionKey = null);
