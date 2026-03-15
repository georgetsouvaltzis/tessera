using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Controls.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="LayoutContainerComponent"/>.
/// </summary>
internal sealed record LayoutContainerOptions(
    LayoutFlow Mode = LayoutFlow.Rows,
    int GridRows = 1,
    int GridColumns = 1,
    bool EnableMouseInteractions = true,
    bool EnableMouseResize = true,
    int SplitterHitThickness = 1,
    int MinPrimarySize = 8,
    int MinSecondarySize = 8,
    int? PrimarySize = null);
