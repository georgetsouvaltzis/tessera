using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="DialogComponent"/>.
/// </summary>
/// <param name="Title">Dialog title shown in the frame.</param>
/// <param name="Lines">Dialog body lines rendered inside the content area.</param>
/// <param name="Visible">Whether the dialog starts visible.</param>
/// <param name="Focused">Whether the dialog starts focused for keyboard handling.</param>
/// <param name="Border">Frame border style. Use <see cref="BorderStyle.None"/> for a borderless dialog.</param>
/// <param name="Padding">Inner spacing applied after the frame is resolved.</param>
/// <param name="Theme">Optional UI theme override.</param>
/// <param name="AcceptKey">Optional key binding used to accept the dialog.</param>
/// <param name="DismissKey">Optional key binding used to dismiss the dialog.</param>
public sealed record DialogOptions(
    string Title = "Dialog",
    IReadOnlyList<string>? Lines = null,
    bool Visible = false,
    bool Focused = false,
    BorderStyle Border = BorderStyle.Rounded,
    Thickness Padding = default,
    UiTheme? Theme = null,
    KeyBinding? AcceptKey = null,
    KeyBinding? DismissKey = null);
