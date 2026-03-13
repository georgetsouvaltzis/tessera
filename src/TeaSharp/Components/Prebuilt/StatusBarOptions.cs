using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.ComponentModel;
namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="StatusBarComponent"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record StatusBarOptions(
    string LeftText = "",
    string RightText = "",
    UiTheme? Theme = null);
