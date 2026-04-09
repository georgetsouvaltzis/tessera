using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record WindowSizeMsg(int Width, int Height) : IMessage;

