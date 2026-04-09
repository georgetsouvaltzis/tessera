using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record PasteMsg(string Content) : IMessage;

