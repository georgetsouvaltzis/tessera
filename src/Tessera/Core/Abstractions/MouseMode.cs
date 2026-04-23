namespace Tessera.Core.Abstractions;

/// <summary>
///     Describes the level of mouse reporting requested from the terminal.
/// </summary>
public enum MouseMode
{
    /// <summary>
    ///     Disables mouse reporting.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Reports button events and motion while a button is pressed.
    /// </summary>
    CellMotion = 1,

    /// <summary>
    ///     Reports all motion, including hover movement.
    /// </summary>
    AllMotion = 2
}
