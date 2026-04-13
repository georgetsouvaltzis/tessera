using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Reports the current terminal capability profile.
/// </summary>
/// <param name="Profile">The capability profile resolved for the session.</param>
public sealed record TerminalCapabilitiesMsg(TerminalCapabilityProfile Profile) : IMessage;
