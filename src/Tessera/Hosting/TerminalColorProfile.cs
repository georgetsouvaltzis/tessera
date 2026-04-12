using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Describes the terminal color support available to advanced Tessera hosting seams.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public enum TerminalColorProfile
{
    /// <summary>
    /// The value could not be determined.
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// The ansi 16 value.
    /// </summary>
    Ansi16 = 1,
    /// <summary>
    /// The ansi 256 value.
    /// </summary>
    Ansi256 = 2,
    /// <summary>
    /// The true color value.
    /// </summary>
    TrueColor = 3,
}
