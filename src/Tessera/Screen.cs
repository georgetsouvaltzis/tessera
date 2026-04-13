using System.ComponentModel;
using Tessera.Components.Composition;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Internal;
using Tessera.Layout;

namespace Tessera;

/// <summary>
///     Represents the content and screen-level options to render for the current application state.
/// </summary>
/// <remarks>
///     Use <see cref="From(Control)" /> for the normal root-control path, <see cref="From(LayoutNode)" /> when you
///     need to compose an explicit layout tree, <see cref="From(string)" /> for simple text screens, and
///     <see cref="From(Canvas)" /> for direct canvas rendering.
/// </remarks>
public sealed class Screen
{
    private readonly LayoutNode? _layout;
    private readonly string? _text;

    private Screen(string? text = null, LayoutNode? layout = null)
    {
        _text = text;
        _layout = layout;
    }

    /// <summary>
    ///     Gets the screen options applied when this screen is rendered.
    /// </summary>
    public ScreenOptions Options { get; init; } = ScreenOptions.Empty;

    /// <summary>
    ///     Gets an empty screen.
    /// </summary>
    public static Screen Empty { get; } = new(string.Empty);

    /// <summary>
    ///     Creates a screen from plain text content.
    /// </summary>
    /// <param name="content">The text content to render.</param>
    /// <returns>A screen that renders the supplied text.</returns>
    public static Screen From(string content)
    {
        return new Screen(content);
    }

    /// <summary>
    ///     Creates a screen from a canvas snapshot.
    /// </summary>
    /// <param name="canvas">The canvas to render.</param>
    /// <returns>A screen that renders the supplied canvas.</returns>
    public static Screen From(Canvas canvas)
    {
        return new Screen(canvas.Render());
    }

    /// <summary>
    ///     Creates a screen from a layout tree.
    /// </summary>
    /// <param name="layout">The layout tree to render.</param>
    /// <returns>A screen that renders the supplied layout.</returns>
    /// <remarks>
    ///     Prefer <see cref="Build(Action{WindowBuilder})" /> for the default app path.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Screen From(LayoutNode layout)
    {
        return new Screen(layout: layout ?? throw new ArgumentNullException(nameof(layout)));
    }

    /// <summary>
    ///     Creates a screen from a root control.
    /// </summary>
    /// <param name="control">The control to render.</param>
    /// <returns>A screen that renders the supplied control.</returns>
    public static Screen From(Control control)
    {
        return new Screen(layout: new ComponentLayout(control ?? throw new ArgumentNullException(nameof(control))));
    }

    /// <summary>
    ///     Creates a screen from an advanced canvas component.
    /// </summary>
    /// <param name="component">The component to render.</param>
    /// <returns>A screen that renders the supplied advanced component.</returns>
    /// <remarks>
    ///     <see cref="ICanvasComponent" /> is a render-only advanced seam. Use <see cref="Control" /> for interactive
    ///     custom widgets that participate in focus and input routing.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Screen From(ICanvasComponent component)
    {
        return new Screen(layout: new ComponentLayout(component ?? throw new ArgumentNullException(nameof(component))));
    }

    /// <summary>
    ///     Builds a screen through the imperative window builder facade.
    /// </summary>
    /// <param name="configure">The window builder callback.</param>
    /// <returns>A screen that renders the configured window layout.</returns>
    public static Screen Build(Action<WindowBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new WindowBuilder();
        configure(builder);
        return From(builder.Build());
    }

    internal ScreenRenderResult Compile(IScreenCompiler compiler, ScreenContext context, ScreenOptions defaults)
    {
        ArgumentNullException.ThrowIfNull(compiler);

        return compiler.Compile(
            new ScreenContent(_text, _layout),
            context,
            defaults.Merge(Options));
    }
}
