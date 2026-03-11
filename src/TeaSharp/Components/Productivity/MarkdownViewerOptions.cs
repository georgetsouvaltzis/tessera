using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="MarkdownViewerComponent"/>.
/// </summary>
public sealed record MarkdownViewerOptions(
    string Title = "Markdown",
    string InitialMarkdown = "",
    bool Focused = false,
    bool ShowBorder = true,
    bool Wrap = false,
    bool ShowLineNumbers = false,
    ViewportKeyMap? ViewportKeyMap = null);
