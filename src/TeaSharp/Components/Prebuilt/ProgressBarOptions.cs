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
/// Defines the one-shot configuration used to construct a <see cref="ProgressBarComponent"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record ProgressBarOptions(
    string Title = "Progress",
    double InitialValue = 0.0,
    bool IsFocused = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    double Step = 0.05,
    KeyBinding? DecreaseKey = null,
    KeyBinding? IncreaseKey = null);
