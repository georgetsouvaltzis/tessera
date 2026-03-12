using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;
using System.ComponentModel;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="MenuBarComponent"/>.
/// </summary>
/// <param name="Items">Initial menu bar items.</param>
/// <param name="IsFocused">Whether the menu bar starts focused.</param>
/// <param name="IsDisabled">Whether the menu bar starts disabled.</param>
/// <param name="IsReadOnly">Whether the menu bar can navigate without activating actions.</param>
/// <param name="NextItemKey">Optional key binding used to move to the next item.</param>
/// <param name="PreviousItemKey">Optional key binding used to move to the previous item.</param>
/// <param name="ActivateKey">Optional key binding used to activate the selected item.</param>
/// <param name="InteractionProfile">Optional mouse interaction profile.</param>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed record MenuBarOptions(
    IEnumerable<MenuBarItem>? Items = null,
    bool IsFocused = false,
    bool IsDisabled = false,
    bool IsReadOnly = false,
    KeyBinding? NextItemKey = null,
    KeyBinding? PreviousItemKey = null,
    KeyBinding? ActivateKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
