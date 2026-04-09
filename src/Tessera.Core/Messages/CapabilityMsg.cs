using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record CapabilityMsg(string Name, string? Value, string Raw) : IMessage;

