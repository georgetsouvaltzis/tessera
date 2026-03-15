using System.Diagnostics.CodeAnalysis;

namespace TeaSharp;

/// <summary>
/// Represents an input, runtime, or effect-driven event handled by a <see cref="TeaApp"/>.
/// </summary>
/// <remarks>
/// Application code typically handles domain-specific messages, lifecycle messages such as
/// <see cref="WindowResized"/>, and any messages emitted by <see cref="TeaEffect"/> instances. Built-in
/// controls usually translate direct input into internal state changes before raising higher-level events.
/// </remarks>
public abstract record Message;

/// <summary>
/// Represents a key press event.
/// </summary>
/// <param name="Key">The pressed key.</param>
/// <param name="Text">The text produced by the key press, when any.</param>
/// <param name="Modifiers">The modifier keys active during the key press.</param>
/// <param name="IsRepeat"><see langword="true"/> when the key press is an auto-repeat event.</param>
public sealed record KeyPressed(
    Key Key,
    string Text = "",
    ModifierKeys Modifiers = ModifierKeys.None,
    bool IsRepeat = false) : Message
{
    /// <summary>
    /// Determines whether the event matches the supplied key and modifier combination.
    /// </summary>
    /// <param name="key">The key to compare.</param>
    /// <param name="modifiers">The modifiers to compare.</param>
    /// <returns><see langword="true"/> when the event matches; otherwise, <see langword="false"/>.</returns>
    public bool Is(Key key, ModifierKeys modifiers = ModifierKeys.None)
    {
        return Key == key && Modifiers == modifiers;
    }

    /// <summary>
    /// Determines whether the event produced the supplied character.
    /// </summary>
    /// <param name="character">The character to compare.</param>
    /// <param name="ignoreCase"><see langword="true"/> to compare case-insensitively; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the event produced the supplied character; otherwise, <see langword="false"/>.</returns>
    public bool IsCharacter(char character, bool ignoreCase = true)
    {
        if (Key != Key.Character || Text.Length != 1)
        {
            return false;
        }

        var value = Text[0];
        return ignoreCase
            ? char.ToLowerInvariant(value) == char.ToLowerInvariant(character)
            : value == character;
    }

    /// <summary>
    /// Determines whether the event produced the supplied character with the supplied modifiers.
    /// </summary>
    /// <param name="character">The character to compare.</param>
    /// <param name="modifiers">The modifiers to compare.</param>
    /// <param name="ignoreCase"><see langword="true"/> to compare case-insensitively; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the event matches; otherwise, <see langword="false"/>.</returns>
    public bool IsCharacter(char character, ModifierKeys modifiers, bool ignoreCase = true)
    {
        return Modifiers == modifiers && IsCharacter(character, ignoreCase);
    }
}

/// <summary>
/// Represents a key release event.
/// </summary>
/// <param name="Key">The released key.</param>
/// <param name="Text">The text associated with the key release, when any.</param>
/// <param name="Modifiers">The modifier keys active during the key release.</param>
public sealed record KeyReleased(
    Key Key,
    string Text = "",
    ModifierKeys Modifiers = ModifierKeys.None) : Message;

/// <summary>
/// Represents a terminal resize event.
/// </summary>
/// <param name="Width">The new terminal width in character cells.</param>
/// <param name="Height">The new terminal height in character cells.</param>
public sealed record WindowResized(int Width, int Height) : Message;

/// <summary>
/// Represents a pointer event.
/// </summary>
/// <param name="Kind">The pointer event kind.</param>
/// <param name="Button">The button involved in the event.</param>
/// <param name="X">The pointer X coordinate.</param>
/// <param name="Y">The pointer Y coordinate.</param>
/// <param name="Modifiers">The modifier keys active during the event.</param>
public sealed record PointerInput(
    PointerEventKind Kind,
    PointerButton Button,
    int X,
    int Y,
    ModifierKeys Modifiers = ModifierKeys.None) : Message;

/// <summary>
/// Represents the start of a bracketed paste sequence.
/// </summary>
public sealed record PasteStarted : Message;

/// <summary>
/// Represents the end of a bracketed paste sequence.
/// </summary>
public sealed record PasteEnded : Message;

/// <summary>
/// Represents pasted text content.
/// </summary>
/// <param name="Content">The pasted content.</param>
public sealed record Pasted(string Content) : Message;

/// <summary>
/// Represents a terminal focus change.
/// </summary>
/// <param name="IsFocused"><see langword="true"/> when the terminal gained focus; otherwise, <see langword="false"/>.</param>
public sealed record FocusChanged(bool IsFocused) : Message;

/// <summary>
/// Represents an exception surfaced to the application as a message.
/// </summary>
/// <param name="Exception">The exception that was captured.</param>
public sealed record Faulted(Exception Exception) : Message;

/// <summary>
/// Represents a message that wraps an external runtime payload.
/// </summary>
/// <param name="Raw">The raw payload supplied by the runtime or host.</param>
public sealed record ExternalMessage(object Raw) : Message;

/// <summary>
/// Identifies a keyboard key independent of platform-specific key codes.
/// </summary>
public enum Key
{
    Unknown = 0,
    Character,
    Enter,
    Tab,
    Escape,
    Backspace,
    Up,
    Down,
    Left,
    Right,
    Home,
    End,
    PageUp,
    PageDown,
    Insert,
    Delete,
    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,
    F8,
    F9,
    F10,
    F11,
    F12,
}

/// <summary>
/// Represents keyboard modifiers active during an input event.
/// </summary>
[Flags]
public enum ModifierKeys
{
    None = 0,
    Shift = 1 << 0,
    Alt = 1 << 1,
    Ctrl = 1 << 2,
    Meta = 1 << 3,
}

/// <summary>
/// Identifies the kind of pointer event that occurred.
/// </summary>
public enum PointerEventKind
{
    Press = 0,
    Release = 1,
    Motion = 2,
    Wheel = 3,
}

/// <summary>
/// Identifies the pointer button involved in a pointer event.
/// </summary>
public enum PointerButton
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

/// <summary>
/// Defines the mouse tracking mode requested for a screen.
/// </summary>
public enum MouseTrackingMode
{
    None = 0,
    CellMotion = 1,
    AllMotion = 2,
}
