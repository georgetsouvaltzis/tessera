using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="LogViewerComponent"/>.
/// </summary>
public sealed record LogViewerOptions(
    string Title = "Logs",
    IEnumerable<string>? InitialEntries = null,
    bool Focused = false,
    bool ShowBorder = true,
    bool AutoScroll = true,
    string InitialFilter = "",
    ViewportKeyMap? ViewportKeyMap = null,
    KeyBinding? TogglePauseKey = null,
    KeyBinding? ClearKey = null);
