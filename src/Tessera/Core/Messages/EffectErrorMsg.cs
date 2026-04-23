using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Carries an exception raised while executing an effect.
/// </summary>
/// <param name="Exception">The exception captured from the effect.</param>
public sealed record EffectErrorMsg(Exception Exception) : IMessage;
