using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Reports the terminal background color.
/// </summary>
/// <param name="Color">The reported color value.</param>
public sealed record BackgroundColorMsg(string Color) : IMessage;
