using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Carries terminal input that could not be decoded into a richer message.
/// </summary>
/// <param name="Raw">The undecoded raw terminal payload.</param>
public sealed record UnknownInputMsg(string Raw) : IMessage;
