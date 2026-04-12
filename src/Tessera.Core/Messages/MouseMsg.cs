using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Base contract for decoded mouse input.
/// </summary>
/// <param name="Button">The button or wheel source associated with the event.</param>
/// <param name="X">The zero-based column position.</param>
/// <param name="Y">The zero-based row position.</param>
/// <param name="Modifiers">The active modifier keys.</param>
public abstract record MouseMsg(
    MouseButton Button,
    int X,
    int Y,
    KeyModifiers Modifiers = KeyModifiers.None) : IMessage
{
    /// <summary>
    /// Gets the specific mouse event category.
    /// </summary>
    public abstract MouseEventType EventType { get; }
}
