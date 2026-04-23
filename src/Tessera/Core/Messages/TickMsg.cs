using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Carries a scheduled timestamp tick.
/// </summary>
/// <param name="Timestamp">The timestamp generated for the tick.</param>
public sealed record TickMsg(DateTimeOffset Timestamp) : IMessage;
