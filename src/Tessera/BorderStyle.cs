namespace Tessera;

/// <summary>
/// Defines the frame style used when a component renders a border.
/// </summary>
public enum BorderStyle
{
    /// <summary>
    /// No border is rendered.
    /// </summary>
    None = -1,
    /// <summary>
    /// Single-line box drawing border.
    /// </summary>
    SingleLine = 0,
    /// <summary>
    /// Rounded box drawing border.
    /// </summary>
    Rounded = 1,
    /// <summary>
    /// Heavy box drawing border.
    /// </summary>
    Heavy = 2,
    /// <summary>
    /// ASCII fallback border.
    /// </summary>
    Ascii = 3,
}
