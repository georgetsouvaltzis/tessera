using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;
using System.ComponentModel;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="LogViewerComponent"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record LogViewerOptions(
    string Title = "Logs",
    IEnumerable<string>? InitialEntries = null,
    bool IsFocused = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    bool AutoScroll = true,
    string InitialFilter = "",
    ViewportKeyMap? ViewportKeyMap = null,
    KeyBinding? TogglePauseKey = null,
    KeyBinding? ClearKey = null);
