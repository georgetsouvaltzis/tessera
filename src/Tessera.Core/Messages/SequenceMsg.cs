using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record SequenceMsg(IReadOnlyList<Effect> Effects) : IMessage;
