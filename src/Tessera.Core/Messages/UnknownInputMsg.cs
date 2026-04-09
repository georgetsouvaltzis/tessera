using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record UnknownInputMsg(string Raw) : IMessage;

