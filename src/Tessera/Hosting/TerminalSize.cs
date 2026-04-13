using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
///     Represents the terminal size used by advanced Tessera hosting seams.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct TerminalSize(int Width, int Height);
