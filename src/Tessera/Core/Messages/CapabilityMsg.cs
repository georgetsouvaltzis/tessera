using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Carries a terminal capability response.
/// </summary>
/// <param name="Name">The capability name.</param>
/// <param name="Value">The decoded capability value, when present.</param>
/// <param name="Raw">The raw terminal payload.</param>
public sealed record CapabilityMsg(string Name, string? Value, string Raw) : IMessage;
