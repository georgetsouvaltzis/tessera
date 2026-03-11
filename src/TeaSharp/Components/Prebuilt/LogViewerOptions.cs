using TeaSharp.Widgets;

namespace TeaSharp.Components;

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
