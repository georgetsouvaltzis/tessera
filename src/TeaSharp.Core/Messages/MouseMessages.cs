using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Messages;

public enum MouseEventType
{
    Press = 0,
    Release = 1,
    Motion = 2,
    Wheel = 3,
}

public enum MouseButton
{
    None = 0,
    Left = 1,
    Middle = 2,
    Right = 3,
    WheelUp = 4,
    WheelDown = 5,
    WheelLeft = 6,
    WheelRight = 7,
    Backward = 8,
    Forward = 9,
    Button10 = 10,
    Button11 = 11,
}

public abstract record MouseMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : IMessage
{
    public abstract MouseEventType EventType { get; }
}

public sealed record MouseClickMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : MouseMsg(Button, X, Y, Modifiers)
{
    public override MouseEventType EventType => MouseEventType.Press;
}

public sealed record MouseReleaseMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : MouseMsg(Button, X, Y, Modifiers)
{
    public override MouseEventType EventType => MouseEventType.Release;
}

public sealed record MouseMotionMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : MouseMsg(Button, X, Y, Modifiers)
{
    public override MouseEventType EventType => MouseEventType.Motion;
}

public sealed record MouseWheelMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : MouseMsg(Button, X, Y, Modifiers)
{
    public override MouseEventType EventType => MouseEventType.Wheel;
}
