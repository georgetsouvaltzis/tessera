using System.ComponentModel;

namespace TeaSharp.Hosting;

/// <summary>
/// Represents the terminal size used by advanced TeaSharp hosting seams.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct TerminalSize(int Width, int Height);
