using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public abstract record MouseMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : IMessage
{
    public abstract MouseEventType EventType { get; }
}

