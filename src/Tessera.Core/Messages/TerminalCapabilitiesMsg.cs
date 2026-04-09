using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record TerminalCapabilitiesMsg(TerminalCapabilityProfile Profile) : IMessage;

