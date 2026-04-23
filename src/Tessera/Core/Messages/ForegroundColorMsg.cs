using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Reports the terminal foreground color.
/// </summary>
/// <param name="Color">The reported color value.</param>
public sealed record ForegroundColorMsg(string Color) : IMessage;
