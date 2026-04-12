using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Represents the cursor style requested by an advanced renderer seam.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public enum TerminalCursorStyle
{
    /// <summary>
    /// The blinking block value.
    /// </summary>
    BlinkingBlock = 0,
    /// <summary>
    /// The steady block value.
    /// </summary>
    SteadyBlock = 1,
    /// <summary>
    /// The blinking underline value.
    /// </summary>
    BlinkingUnderline = 2,
    /// <summary>
    /// The steady underline value.
    /// </summary>
    SteadyUnderline = 3,
    /// <summary>
    /// The blinking bar value.
    /// </summary>
    BlinkingBar = 4,
    /// <summary>
    /// The steady bar value.
    /// </summary>
    SteadyBar = 5,
}
