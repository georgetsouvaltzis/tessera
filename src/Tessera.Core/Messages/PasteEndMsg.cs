using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Marks the end of a bracketed paste sequence.
/// </summary>
public sealed record PasteEndMsg : IMessage;
