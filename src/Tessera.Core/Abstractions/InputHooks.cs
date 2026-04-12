using Tessera.Core.Messages;

namespace Tessera.Core.Abstractions;

/// <summary>
/// Configures optional runtime callbacks for raw input handling.
/// </summary>
public readonly record struct InputHooks
{
    /// <summary>
    /// Handles mouse input before it reaches normal app-level processing.
    /// </summary>
    public Func<MouseMsg, Effect?>? OnMouse { get; init; }
}
