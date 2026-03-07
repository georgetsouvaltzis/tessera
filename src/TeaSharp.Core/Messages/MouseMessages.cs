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
    Button12 = 12,
    Button13 = 13,
    Button14 = 14,
    Button15 = 15,
    Button16 = 16,
    Button17 = 17,
    Button18 = 18,
    Button19 = 19,
    Button20 = 20,
    Button21 = 21,
    Button22 = 22,
    Button23 = 23,
    Button24 = 24,
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
