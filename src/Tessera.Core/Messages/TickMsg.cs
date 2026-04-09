using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record TickMsg(DateTimeOffset Timestamp) : IMessage;

