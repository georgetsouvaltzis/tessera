using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Controls;
using TeaSharp.Components.Composition;
using TeaSharp.Internal;
using TeaSharp.Layout;
using System.ComponentModel;

namespace TeaSharp;

/// <summary>
/// Represents the content and screen-level options to render for the current application state.
/// </summary>
/// <remarks>
/// Use <see cref="From(Control)"/> for the normal root-control path, <see cref="From(LayoutNode)"/> when you
/// need to compose an explicit layout tree, <see cref="From(string)"/> for simple text screens, and
/// <see cref="From(Canvas)"/> for direct canvas rendering.
/// </remarks>
public sealed class Screen
{
    private readonly string? _text;
    private readonly LayoutNode? _layout;

    private Screen(string? text = null, LayoutNode? layout = null)
    {
        _text = text;
        _layout = layout;
    }

    /// <summary>
    /// Gets the screen options applied when this screen is rendered.
    /// </summary>
    public ScreenOptions Options { get; init; } = ScreenOptions.Empty;

    /// <summary>
    /// Gets an empty screen.
    /// </summary>
    public static Screen Empty { get; } = new(string.Empty);

    /// <summary>
    /// Creates a screen from plain text content.
    /// </summary>
    /// <param name="content">The text content to render.</param>
    /// <returns>A screen that renders the supplied text.</returns>
    public static Screen From(string content) => new(content ?? string.Empty);

    /// <summary>
    /// Creates a screen from a canvas snapshot.
    /// </summary>
    /// <param name="canvas">The canvas to render.</param>
    /// <returns>A screen that renders the supplied canvas.</returns>
    public static Screen From(Canvas canvas) => new(canvas.Render());

    /// <summary>
    /// Creates a screen from a layout tree.
    /// </summary>
    /// <param name="layout">The layout tree to render.</param>
    /// <returns>A screen that renders the supplied layout.</returns>
    public static Screen From(LayoutNode layout) => new(layout: layout ?? throw new ArgumentNullException(nameof(layout)));

    /// <summary>
    /// Creates a screen from a root control.
    /// </summary>
    /// <param name="control">The control to render.</param>
    /// <returns>A screen that renders the supplied control.</returns>
    public static Screen From(Control control) =>
        new(layout: new ComponentLayout(control ?? throw new ArgumentNullException(nameof(control))));

    /// <summary>
    /// Creates a screen from an advanced canvas component.
    /// </summary>
    /// <param name="component">The component to render.</param>
    /// <returns>A screen that renders the supplied advanced component.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Screen From(ICanvasComponent component) =>
        new(layout: new ComponentLayout(component ?? throw new ArgumentNullException(nameof(component))));

    internal ScreenRenderResult Compile(ScreenContext context, ScreenOptions defaults)
    {
        if (_layout is not null)
        {
            return ScreenCompiler.Compile(_layout, context, defaults.Merge(Options));
        }

        var output = new ScreenOutput(ScreenFrame.From(_text ?? string.Empty))
        {
            Terminal = defaults.Merge(Options).ToTerminalOutput(),
        };

        return new ScreenRenderResult(output, null);
    }
}
