using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record CursorColorMsg(string Color) : IMessage;

