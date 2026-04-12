using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Reports keyboard enhancement flags negotiated with the terminal.
/// </summary>
/// <param name="Flags">The raw enhancement flag bitmask.</param>
public sealed record KeyboardEnhancementsMsg(int Flags) : IMessage
{
    /// <summary>
    /// Determines whether key disambiguation support is available.
    /// </summary>
    /// <returns><see langword="true" /> when key disambiguation is supported.</returns>
    public bool SupportsKeyDisambiguation() => Flags > 0;

    /// <summary>
    /// Determines whether distinct key event types are reported.
    /// </summary>
    /// <returns><see langword="true" /> when event-type reporting is supported.</returns>
    public bool SupportsEventTypes() => (Flags & 0b10) != 0;
}
