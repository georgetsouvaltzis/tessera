using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Represents a key release received from the terminal.
/// </summary>
/// <param name="Code">The logical key code.</param>
/// <param name="Text">The text payload produced by the key, when any.</param>
/// <param name="Modifiers">The active modifier keys.</param>
public sealed record KeyReleaseMsg(
    KeyCode Code,
    string Text = "",
    KeyModifiers Modifiers = KeyModifiers.None) : IMessage;
