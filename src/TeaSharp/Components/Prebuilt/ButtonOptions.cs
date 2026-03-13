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
/// Defines the one-shot configuration used to construct a <see cref="ButtonComponent"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record ButtonOptions(
    string Label = "Button",
    string? Description = null,
    bool IsFocused = false,
    bool Enabled = true,
    BorderStyle Border = BorderStyle.None,
    Thickness Padding = default,
    WidgetInteractionProfile? InteractionProfile = null);
