namespace Tessera.Core.Abstractions;

/// <summary>
///     Represents a terminal-integrated progress indicator request.
/// </summary>
/// <param name="State">The progress state to apply.</param>
/// <param name="Value">The progress value, typically in the range 0-100.</param>
public readonly record struct TerminalProgress(
    TerminalProgressState State,
    int Value);
