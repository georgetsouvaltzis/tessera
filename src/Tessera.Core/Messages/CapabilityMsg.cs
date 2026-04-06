using Tessera.Core.Abstractions;
using Tessera.Core.Terminal;

namespace Tessera.Core.Messages;

public sealed record CapabilityMsg(string Name, string? Value, string Raw) : IMessage;

