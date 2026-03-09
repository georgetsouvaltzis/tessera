using System.Text;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Messages;

public sealed record KeyReleaseMsg(
    KeyCode Code,
    string Text = "",
    KeyModifiers Modifiers = KeyModifiers.None) : IMessage;
