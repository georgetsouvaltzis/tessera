using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TextAreaComponent"/>.
/// </summary>
public sealed record TextAreaOptions(
    string Title = "Text Area",
    string InitialValue = "",
    bool Focused = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    bool ShowLineNumbers = false,
    bool Wrap = false,
    TextInputKeyMap? InputKeyMap = null,
    ViewportKeyMap? ViewportKeyMap = null);
