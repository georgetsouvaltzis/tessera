using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Indicates that the terminal window lost focus.
/// </summary>
public sealed record FocusOutMsg : IMessage;
