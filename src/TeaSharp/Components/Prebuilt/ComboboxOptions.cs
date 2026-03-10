using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record ComboboxOptions(
    IReadOnlyList<string>? Items = null,
    string Title = "Combobox",
    string Placeholder = "",
    string InitialFilter = "",
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    bool ShowBorder = true,
    int MaxVisibleItems = 6,
    TextInputKeyMap? InputKeyMap = null,
    WidgetInteractionProfile? InteractionProfile = null,
    KeyBinding? OpenKey = null,
    KeyBinding? CloseKey = null,
    KeyBinding? NextItemKey = null,
    KeyBinding? PreviousItemKey = null,
    KeyBinding? ConfirmSelectionKey = null);
