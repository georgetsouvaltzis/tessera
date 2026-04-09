using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record ForegroundColorMsg(string Color) : IMessage;

