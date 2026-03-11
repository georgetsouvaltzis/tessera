using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Prebuilt;

public sealed record LabelOptions(
    string Text = "",
    string? Title = null,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default);
