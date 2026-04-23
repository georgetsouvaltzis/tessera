namespace Tessera.Core.Abstractions;

/// <summary>
///     Declares optional keyboard protocol features requested from the terminal.
/// </summary>
public readonly record struct KeyboardEnhancementOptions
{
    /// <summary>
    ///     Requests key press and release event reporting when the terminal supports it.
    /// </summary>
    public bool ReportEventTypes { get; init; }
}
