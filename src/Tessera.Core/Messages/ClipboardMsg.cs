using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Carries clipboard contents returned by the terminal.
/// </summary>
/// <param name="Content">The decoded clipboard text.</param>
/// <param name="Selection">The terminal clipboard selection identifier.</param>
public sealed record ClipboardMsg(string Content, char Selection = 'c') : IMessage;
