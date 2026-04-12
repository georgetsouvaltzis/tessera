namespace Tessera.Components.Primitives;

/// <summary>
/// Represents canvas text mode.
/// </summary>
public enum CanvasTextMode
{
    /// <summary>
    /// Uses the fast text path.
    /// </summary>
    Fast = 0,
    /// <summary>
    /// Uses grapheme-aware text handling.
    /// </summary>
    GraphemeAware = 1,
}
