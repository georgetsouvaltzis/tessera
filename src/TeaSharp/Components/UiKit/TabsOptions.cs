using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TabsComponent"/>.
/// </summary>
public sealed record TabsOptions(
    IEnumerable<string> Tabs,
    bool Focused = false,
    bool EnableNumericShortcuts = true,
    KeyBinding? NextTabKey = null,
    KeyBinding? PreviousTabKey = null,
    WidgetInteractionProfile? InteractionProfile = null);
