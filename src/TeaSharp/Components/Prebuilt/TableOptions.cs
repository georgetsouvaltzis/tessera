using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.ComponentModel;
namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="TableComponent"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record TableOptions(
    IReadOnlyList<string> Headers,
    string Title = "Table",
    bool IsFocused = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    int? PageSize = null);
