using Tessera.Core.Abstractions;
using Tessera.Core.Terminal;

namespace Tessera.Core.Messages;

public sealed record TerminalCapabilitiesMsg(TerminalCapabilityProfile Profile) : IMessage;

