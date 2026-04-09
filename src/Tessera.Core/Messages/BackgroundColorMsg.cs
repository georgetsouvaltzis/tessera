using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record BackgroundColorMsg(string Color) : IMessage;

