namespace Tessera.Core.Abstractions;

/// <summary>
/// Describes the progress state requested from the host terminal or shell integration.
/// </summary>
public enum TerminalProgressState
{
    /// <summary>
    /// Clears any active progress indication.
    /// </summary>
    None = 0,

    /// <summary>
    /// Shows normal determinate progress.
    /// </summary>
    Default = 1,

    /// <summary>
    /// Shows an error progress state.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Shows indeterminate progress.
    /// </summary>
    Indeterminate = 3,

    /// <summary>
    /// Shows a warning progress state.
    /// </summary>
    Warning = 4,
}
