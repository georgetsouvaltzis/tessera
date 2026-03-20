namespace TeaSharp.Controls;

/// <summary>
/// Identifies the severity of a validation issue.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// Informational issue that does not block completion.
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning issue that may require user attention.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error issue that blocks successful validation.
    /// </summary>
    Error = 2,
}
