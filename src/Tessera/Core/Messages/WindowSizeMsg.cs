using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Reports a terminal window-size change.
/// </summary>
/// <param name="Width">The window width in columns.</param>
/// <param name="Height">The window height in rows.</param>
public sealed record WindowSizeMsg(int Width, int Height) : IMessage;
