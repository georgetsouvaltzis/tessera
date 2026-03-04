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

public sealed record MouseMsg(
    MouseEventType EventType,
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : IMessage;
