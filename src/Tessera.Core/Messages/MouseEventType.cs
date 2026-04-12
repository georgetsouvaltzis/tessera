
namespace Tessera.Core.Messages;

/// <summary>
/// Describes the category of a mouse event.
/// </summary>
public enum MouseEventType
{
    /// <summary>A button press event.</summary>
    Press = 0,
    /// <summary>A button release event.</summary>
    Release = 1,
    /// <summary>A pointer movement event.</summary>
    Motion = 2,
    /// <summary>A mouse wheel event.</summary>
    Wheel = 3,
}
