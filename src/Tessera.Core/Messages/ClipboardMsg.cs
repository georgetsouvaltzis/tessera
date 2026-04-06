using Tessera.Core.Abstractions;
using Tessera.Core.Terminal;

namespace Tessera.Core.Messages;

public sealed record ClipboardMsg(string Content, char Selection = 'c') : IMessage;

