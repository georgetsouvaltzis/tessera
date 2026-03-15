using System.ComponentModel;

namespace TeaSharp.Hosting;

/// <summary>
/// Represents the cursor style requested by an advanced renderer seam.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public enum TerminalCursorStyle
{
    BlinkingBlock = 0,
    SteadyBlock = 1,
    BlinkingUnderline = 2,
    SteadyUnderline = 3,
    BlinkingBar = 4,
    SteadyBar = 5,
}
