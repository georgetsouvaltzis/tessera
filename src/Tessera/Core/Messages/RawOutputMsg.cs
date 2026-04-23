using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Carries raw terminal output that should be written without further processing.
/// </summary>
/// <param name="Content">The raw terminal output content.</param>
public sealed record RawOutputMsg(string Content) : IMessage;
