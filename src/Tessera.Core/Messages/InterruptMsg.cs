using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Requests runtime interruption.
/// </summary>
public sealed record InterruptMsg : IMessage;
