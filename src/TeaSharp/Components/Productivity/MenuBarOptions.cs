using TeaSharp.Widgets;

namespace TeaSharp.Components;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="MenuBarComponent"/>.
/// </summary>
/// <param name="Items">Initial menu bar items.</param>
/// <param name="Focused">Whether the menu bar starts focused.</param>
/// <param name="Disabled">Whether the menu bar starts disabled.</param>
/// <param name="ReadOnly">Whether the menu bar can navigate without activating actions.</param>
/// <param name="NextItemKey">Optional key binding used to move to the next item.</param>
/// <param name="PreviousItemKey">Optional key binding used to move to the previous item.</param>
/// <param name="ActivateKey">Optional key binding used to activate the selected item.</param>
/// <param name="InteractionProfile">Optional mouse interaction profile.</param>
public sealed record MenuBarOptions(
    IEnumerable<MenuBarItem>? Items = null,
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    KeyBinding? NextItemKey = null,
    KeyBinding? PreviousItemKey = null,
    KeyBinding? ActivateKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
