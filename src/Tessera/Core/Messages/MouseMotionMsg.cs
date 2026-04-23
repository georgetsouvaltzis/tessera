namespace Tessera.Core.Messages;

/// <summary>
///     Represents a mouse motion event.
/// </summary>
/// <param name="Button">The button state associated with the motion.</param>
/// <param name="X">The zero-based column position.</param>
/// <param name="Y">The zero-based row position.</param>
/// <param name="Modifiers">The active modifier keys.</param>
public sealed record MouseMotionMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : MouseMsg(Button, X, Y, Modifiers)
{
    /// <inheritdoc />
    public override MouseEventType EventType => MouseEventType.Motion;
}
