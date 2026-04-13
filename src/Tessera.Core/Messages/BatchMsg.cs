using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Carries a batch of effects to execute together.
/// </summary>
/// <param name="Effects">The effects to execute in the batch.</param>
public sealed record BatchMsg(IReadOnlyList<Effect> Effects) : IMessage;
