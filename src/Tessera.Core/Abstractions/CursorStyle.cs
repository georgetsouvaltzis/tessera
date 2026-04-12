namespace Tessera.Core.Abstractions;

/// <summary>
/// Describes the cursor shape requested for the next rendered frame.
/// </summary>
public enum CursorStyle
{
    /// <summary>
    /// A blinking block cursor.
    /// </summary>
    BlinkingBlock = 1,

    /// <summary>
    /// A steady block cursor.
    /// </summary>
    SteadyBlock = 2,

    /// <summary>
    /// A blinking underline cursor.
    /// </summary>
    BlinkingUnderline = 3,

    /// <summary>
    /// A steady underline cursor.
    /// </summary>
    SteadyUnderline = 4,

    /// <summary>
    /// A blinking bar cursor.
    /// </summary>
    BlinkingBar = 5,

    /// <summary>
    /// A steady bar cursor.
    /// </summary>
    SteadyBar = 6,
}
