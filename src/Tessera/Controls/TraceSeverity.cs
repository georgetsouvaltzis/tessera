namespace Tessera.Controls;

/// <summary>
/// Identifies severity for a <see cref="TraceEntry" />.
/// </summary>
public enum TraceSeverity
{
    /// <summary>
    /// Verbose diagnostic output.
    /// </summary>
    Verbose = 0,

    /// <summary>
    /// Informational trace entry.
    /// </summary>
    Info = 1,

    /// <summary>
    /// Warning trace entry.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error trace entry.
    /// </summary>
    Error = 3,

    /// <summary>
    /// Critical trace entry.
    /// </summary>
    Critical = 4,
}
