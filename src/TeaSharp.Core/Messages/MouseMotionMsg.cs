using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Messages;

public sealed record MouseMotionMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : MouseMsg(Button, X, Y, Modifiers)
{
    public override MouseEventType EventType => MouseEventType.Motion;
}

