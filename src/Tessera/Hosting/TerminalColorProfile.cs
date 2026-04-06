using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Describes the terminal color support available to advanced Tessera hosting seams.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public enum TerminalColorProfile
{
    Unknown = 0,
    Ansi16 = 1,
    Ansi256 = 2,
    TrueColor = 3,
}
