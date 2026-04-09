using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record KeyReleaseMsg(
    KeyCode Code,
    string Text = "",
    KeyModifiers Modifiers = KeyModifiers.None) : IMessage;
