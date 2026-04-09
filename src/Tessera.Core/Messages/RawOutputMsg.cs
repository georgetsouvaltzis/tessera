using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record RawOutputMsg(string Content) : IMessage;

