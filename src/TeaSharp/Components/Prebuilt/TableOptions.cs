using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TableComponent"/>.
/// </summary>
public sealed record TableOptions(
    IReadOnlyList<string> Headers,
    string Title = "Table",
    bool IsFocused = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    int? PageSize = null);
