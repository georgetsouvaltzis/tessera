using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Marks the start of a bracketed paste sequence.
/// </summary>
public sealed record PasteStartMsg : IMessage;
