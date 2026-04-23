namespace Tessera.Core.Messages;

/// <summary>
///     Represents a mouse wheel event.
/// </summary>
/// <param name="Button">The wheel direction encoded as a mouse button.</param>
/// <param name="X">The zero-based column position.</param>
/// <param name="Y">The zero-based row position.</param>
/// <param name="Modifiers">The active modifier keys.</param>
public sealed record MouseWheelMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : MouseMsg(Button, X, Y, Modifiers)
{
    /// <inheritdoc />
    public override MouseEventType EventType => MouseEventType.Wheel;
}
