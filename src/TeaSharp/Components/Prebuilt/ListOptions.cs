using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="ListComponent{T}"/>.
/// </summary>
public sealed record ListOptions<T>(
    IEnumerable<T> Items,
    Func<T, string> ToText,
    string Title = "List",
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    ListKeyMap? KeyMap = null);
