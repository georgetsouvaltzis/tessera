using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;
using System.ComponentModel;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="DropdownComponent"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record DropdownOptions(
    IReadOnlyList<string>? Items = null,
    string Title = "Dropdown",
    bool IsFocused = false,
    bool IsDisabled = false,
    bool IsReadOnly = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    int MaxVisibleItems = 6,
    WidgetInteractionProfile? InteractionProfile = null,
    KeyBinding? ToggleOpenKey = null,
    KeyBinding? OpenKey = null,
    KeyBinding? CloseKey = null,
    KeyBinding? NextItemKey = null,
    KeyBinding? PreviousItemKey = null,
    KeyBinding? ConfirmSelectionKey = null);
