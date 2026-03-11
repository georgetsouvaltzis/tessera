using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
namespace TeaSharp.Components.UiKit;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="ModalComponent"/>.
/// </summary>
/// <param name="Title">Modal title shown in the frame.</param>
/// <param name="IsVisible">Whether the modal starts visible.</param>
/// <param name="Border">Frame border style. Use <see cref="BorderStyle.None"/> for a borderless modal.</param>
/// <param name="Padding">Inner spacing applied after the frame is resolved.</param>
/// <param name="BodyLines">Body lines rendered inside the modal.</param>
/// <param name="Theme">Optional UI theme override.</param>
public sealed record ModalOptions(
    string Title = "Modal",
    bool IsVisible = false,
    BorderStyle Border = BorderStyle.Rounded,
    Thickness Padding = default,
    IReadOnlyList<string>? BodyLines = null,
    UiTheme? Theme = null);
