using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record BatchMsg(IReadOnlyList<Effect> Effects) : IMessage;
