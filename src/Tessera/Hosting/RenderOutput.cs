using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
///     Represents the rendered output passed to advanced Tessera renderer seams.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct RenderOutput(string Content)
{
    /// <summary>
    ///     Gets or sets the cursor x.
    /// </summary>
    public int? CursorX { get; init; }

    /// <summary>
    ///     Gets or sets the cursor y.
    /// </summary>
    public int? CursorY { get; init; }

    /// <summary>
    ///     Gets or sets the cursor style.
    /// </summary>
    public TerminalCursorStyle? CursorStyle { get; init; }

    /// <summary>
    ///     Gets or sets the screen options.
    /// </summary>
    public ScreenOptions ScreenOptions { get; init; } = ScreenOptions.Empty;
}
