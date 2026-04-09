using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record ClipboardMsg(string Content, char Selection = 'c') : IMessage;

