using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Represents the rendered output passed to advanced Tessera renderer seams.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct RenderOutput(string Content)
{
    public int? CursorX { get; init; }

    public int? CursorY { get; init; }

    public TerminalCursorStyle? CursorStyle { get; init; }

    public ScreenOptions ScreenOptions { get; init; } = ScreenOptions.Empty;
}
