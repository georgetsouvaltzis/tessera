using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="ContextMenuComponent"/>.
/// </summary>
/// <param name="Items">Initial context-menu items.</param>
/// <param name="Title">Border title shown when the menu renders with a border.</param>
/// <param name="Focused">Whether the menu starts focused.</param>
/// <param name="Disabled">Whether the menu starts disabled.</param>
/// <param name="ReadOnly">Whether the menu can navigate without executing actions.</param>
/// <param name="ShowBorder">Whether the menu renders with a border.</param>
/// <param name="NextItemKey">Optional key binding used to move to the next item.</param>
/// <param name="PreviousItemKey">Optional key binding used to move to the previous item.</param>
/// <param name="ExecuteKey">Optional key binding used to execute the selected item.</param>
/// <param name="CloseKey">Optional key binding used to close the menu.</param>
/// <param name="InteractionProfile">Optional mouse interaction profile.</param>
public sealed record ContextMenuOptions(
    IEnumerable<ContextMenuItem>? Items = null,
    string Title = "Context",
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    bool ShowBorder = true,
    KeyBinding? NextItemKey = null,
    KeyBinding? PreviousItemKey = null,
    KeyBinding? ExecuteKey = null,
    KeyBinding? CloseKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
