using Tessera.Components.Primitives;
using Tessera.Styles;

namespace Tessera;

/// <summary>
///     Provides runtime information that can be used while building a screen.
/// </summary>
public sealed record ScreenContext
{
    /// <summary>
    ///     Gets the current screen width in character cells.
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    ///     Gets the current screen height in character cells.
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    ///     Gets the optional semantic theme configured for the current runtime.
    /// </summary>
    public TesseraTheme? Theme { get; init; }

    /// <summary>
    ///     Gets optional hierarchical theme overrides configured for the current runtime.
    /// </summary>
    public TesseraThemeOverrides? ThemeOverrides { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the application currently has terminal focus.
    /// </summary>
    public bool HasFocus { get; init; } = true;

    /// <summary>
    ///     Gets the full screen bounds for the current context.
    /// </summary>
    public Rect Bounds => new(0, 0, Width, Height);

    /// <summary>
    ///     Creates a canvas sized to the current screen bounds.
    /// </summary>
    /// <param name="textMode">The text layout mode to use for the canvas.</param>
    /// <returns>A canvas sized to the current screen.</returns>
    public Canvas CreateCanvas(CanvasTextMode textMode = CanvasTextMode.Fast)
    {
        return new Canvas(Math.Max(1, Width), Math.Max(1, Height), textMode);
    }
}
