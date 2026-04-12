namespace Tessera.Core.Abstractions;

/// <summary>
/// Represents the text frame and cursor state that should be presented to the user.
/// </summary>
/// <param name="Content">The rendered terminal content for the frame.</param>
public readonly record struct ScreenFrame(string Content)
{
    /// <summary>
    /// Gets the zero-based cursor column, when one should be shown.
    /// </summary>
    public int? CursorX { get; init; }

    /// <summary>
    /// Gets the zero-based cursor row, when one should be shown.
    /// </summary>
    public int? CursorY { get; init; }

    /// <summary>
    /// Gets the cursor shape requested for the frame.
    /// </summary>
    public CursorStyle? CursorStyle { get; init; }

    /// <summary>
    /// Creates a frame from raw text content.
    /// </summary>
    /// <param name="content">The rendered terminal content.</param>
    /// <returns>A frame wrapping the supplied content.</returns>
    public static ScreenFrame From(string content) => new(content);
}
