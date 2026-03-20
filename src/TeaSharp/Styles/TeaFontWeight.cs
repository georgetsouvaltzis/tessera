namespace TeaSharp.Styles;

/// <summary>
/// Represents typography emphasis intent for terminal-rendered text.
/// </summary>
/// <remarks>
/// This maps to ANSI SGR emphasis flags only. It does not control terminal font families, font size, or real font weight engines.
/// </remarks>
public enum TeaFontWeight
{
    /// <summary>
    /// Requests normal emphasis by disabling bold and dim SGR emphasis flags.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Requests bold emphasis using the ANSI bold SGR flag.
    /// </summary>
    Bold = 1,

    /// <summary>
    /// Requests dim emphasis using the ANSI dim SGR flag.
    /// </summary>
    Dim = 2,
}
