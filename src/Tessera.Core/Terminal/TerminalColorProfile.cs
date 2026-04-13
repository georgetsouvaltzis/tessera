namespace Tessera.Core.Terminal;

/// <summary>
///     Describes the color depth supported by the current terminal session.
/// </summary>
public enum TerminalColorProfile
{
    /// <summary>
    ///     The color profile could not be determined.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The terminal supports the standard 16 ANSI colors.
    /// </summary>
    Ansi16 = 1,

    /// <summary>
    ///     The terminal supports the 256-color ANSI palette.
    /// </summary>
    Ansi256 = 2,

    /// <summary>
    ///     The terminal supports 24-bit true color output.
    /// </summary>
    TrueColor = 3
}
