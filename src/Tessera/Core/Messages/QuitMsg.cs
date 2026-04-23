using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Requests application shutdown.
/// </summary>
public sealed record QuitMsg : IMessage;
