namespace Tessera.Core.Messages;

/// <summary>
///     Describes the state returned by a terminal mode report.
/// </summary>
public enum ModeReportState
{
    /// <summary>
    ///     The mode state could not be determined.
    /// </summary>
    Unknown = -1,

    /// <summary>
    ///     The mode is unsupported.
    /// </summary>
    Unsupported = 0,

    /// <summary>
    ///     The mode is currently enabled.
    /// </summary>
    Set = 1,

    /// <summary>
    ///     The mode is currently disabled.
    /// </summary>
    Reset = 2,

    /// <summary>
    ///     The mode is permanently enabled.
    /// </summary>
    PermanentlySet = 3,

    /// <summary>
    ///     The mode is permanently disabled.
    /// </summary>
    PermanentlyReset = 4
}
