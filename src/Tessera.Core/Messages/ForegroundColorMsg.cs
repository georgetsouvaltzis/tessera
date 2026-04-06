using Tessera.Core.Abstractions;
using Tessera.Core.Terminal;

namespace Tessera.Core.Messages;

public sealed record ForegroundColorMsg(string Color) : IMessage;

