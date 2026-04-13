using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Carries pasted text received from the terminal.
/// </summary>
/// <param name="Content">The pasted text content.</param>
public sealed record PasteMsg(string Content) : IMessage;
