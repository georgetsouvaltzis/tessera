using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="ContextMenuComponent"/>.
/// </summary>
/// <param name="Items">Initial context-menu items.</param>
/// <param name="Title">Frame title shown when the menu renders with a border.</param>
/// <param name="IsFocused">Whether the menu starts focused.</param>
/// <param name="IsDisabled">Whether the menu starts disabled.</param>
/// <param name="IsReadOnly">Whether the menu can navigate without executing actions.</param>
/// <param name="Border">Frame border style. Use <see cref="BorderStyle.None"/> for no border.</param>
/// <param name="Padding">Inner spacing applied after the frame is resolved.</param>
/// <param name="NextItemKey">Optional key binding used to move to the next item.</param>
/// <param name="PreviousItemKey">Optional key binding used to move to the previous item.</param>
/// <param name="ExecuteKey">Optional key binding used to execute the selected item.</param>
/// <param name="CloseKey">Optional key binding used to close the menu.</param>
/// <param name="InteractionProfile">Optional mouse interaction profile.</param>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record ContextMenuOptions(
    IEnumerable<ContextMenuItem>? Items = null,
    string Title = "Context",
    bool IsFocused = false,
    bool IsDisabled = false,
    bool IsReadOnly = false,
    BorderStyle Border = BorderStyle.Rounded,
    Thickness Padding = default,
    KeyBinding? NextItemKey = null,
    KeyBinding? PreviousItemKey = null,
    KeyBinding? ExecuteKey = null,
    KeyBinding? CloseKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
