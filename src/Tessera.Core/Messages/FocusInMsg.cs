using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Indicates that the terminal window gained focus.
/// </summary>
public sealed record FocusInMsg : IMessage;
