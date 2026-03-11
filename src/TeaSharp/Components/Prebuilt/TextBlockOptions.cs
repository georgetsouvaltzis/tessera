using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Layout;
namespace TeaSharp.Components.Prebuilt;

public sealed record TextBlockOptions(
    string Text = "",
    string? Title = null,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    TeaSharp.Styles.TeaStyle TextStyle = default,
    HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Left,
    VerticalAlignment VerticalAlignment = VerticalAlignment.Top);
