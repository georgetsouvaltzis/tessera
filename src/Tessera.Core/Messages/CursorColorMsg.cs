using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Reports the terminal cursor color.
/// </summary>
/// <param name="Color">The reported color value.</param>
public sealed record CursorColorMsg(string Color) : IMessage;
