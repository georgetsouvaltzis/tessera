using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="MarkdownViewerComponent"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed record MarkdownViewerOptions(
    string Title = "Markdown",
    string InitialMarkdown = "",
    bool IsFocused = false,
    BorderStyle Border = BorderStyle.SingleLine,
    Thickness Padding = default,
    bool Wrap = false,
    bool ShowLineNumbers = false,
    ViewportKeyMap? ViewportKeyMap = null);
