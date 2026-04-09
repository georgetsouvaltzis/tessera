
namespace Tessera.Core.Messages;

public sealed record MouseClickMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : MouseMsg(Button, X, Y, Modifiers)
{
    public override MouseEventType EventType => MouseEventType.Press;
}

