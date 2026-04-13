using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Carries an ordered effect sequence.
/// </summary>
/// <param name="Effects">The effects to execute in order.</param>
public sealed record SequenceMsg(IReadOnlyList<Effect> Effects) : IMessage;
