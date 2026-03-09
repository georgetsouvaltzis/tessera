using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Messages;

public sealed record ClipboardMsg(string Content, char Selection = 'c') : IMessage;

