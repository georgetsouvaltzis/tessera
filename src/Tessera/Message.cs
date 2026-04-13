namespace Tessera;

/// <summary>
///     Represents an input, runtime, or effect-driven event handled by a <see cref="TesseraApp" />.
/// </summary>
/// <remarks>
///     Application code typically handles domain-specific messages, lifecycle messages such as
///     <see cref="WindowResized" />, and any messages emitted by <see cref="TesseraEffect" /> instances. Built-in
///     controls usually translate direct input into internal state changes before raising higher-level events.
/// </remarks>
public abstract record Message;

/// <summary>
///     Represents a key press event.
/// </summary>
/// <param name="Key">The pressed key.</param>
/// <param name="Text">The text produced by the key press, when any.</param>
/// <param name="Modifiers">The modifier keys active during the key press.</param>
/// <param name="IsRepeat"><see langword="true" /> when the key press is an auto-repeat event.</param>
public sealed record KeyPressed(
    Key Key,
    string Text = "",
    ModifierKeys Modifiers = ModifierKeys.None,
    bool IsRepeat = false) : Message
{
    /// <summary>
    ///     Determines whether the event matches the supplied key and modifier combination.
    /// </summary>
    /// <param name="key">The key to compare.</param>
    /// <param name="modifiers">The modifiers to compare.</param>
    /// <returns><see langword="true" /> when the event matches; otherwise, <see langword="false" />.</returns>
    public bool Is(Key key, ModifierKeys modifiers = ModifierKeys.None)
    {
        return Key == key && Modifiers == modifiers;
    }

    /// <summary>
    ///     Determines whether the event produced the supplied character.
    /// </summary>
    /// <param name="character">The character to compare.</param>
    /// <param name="ignoreCase"><see langword="true" /> to compare case-insensitively; otherwise, <see langword="false" />.</param>
    /// <returns><see langword="true" /> when the event produced the supplied character; otherwise, <see langword="false" />.</returns>
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
    ///     Determines whether the event produced the supplied character with the supplied modifiers.
    /// </summary>
    /// <param name="character">The character to compare.</param>
    /// <param name="modifiers">The modifiers to compare.</param>
    /// <param name="ignoreCase"><see langword="true" /> to compare case-insensitively; otherwise, <see langword="false" />.</param>
    /// <returns><see langword="true" /> when the event matches; otherwise, <see langword="false" />.</returns>
    public bool IsCharacter(char character, ModifierKeys modifiers, bool ignoreCase = true)
    {
        return Modifiers == modifiers && IsCharacter(character, ignoreCase);
    }
}

/// <summary>
///     Represents a key release event.
/// </summary>
/// <param name="Key">The released key.</param>
/// <param name="Text">The text associated with the key release, when any.</param>
/// <param name="Modifiers">The modifier keys active during the key release.</param>
public sealed record KeyReleased(
    Key Key,
    string Text = "",
    ModifierKeys Modifiers = ModifierKeys.None) : Message;

/// <summary>
///     Represents a terminal resize event.
/// </summary>
/// <param name="Width">The new terminal width in character cells.</param>
/// <param name="Height">The new terminal height in character cells.</param>
public sealed record WindowResized(int Width, int Height) : Message;

/// <summary>
///     Represents a pointer event.
/// </summary>
/// <param name="Kind">The pointer event kind.</param>
/// <param name="Button">The button involved in the event.</param>
/// <param name="X">The pointer X coordinate.</param>
/// <param name="Y">The pointer Y coordinate.</param>
/// <param name="Modifiers">The modifier keys active during the event.</param>
/// <param name="ClickCount">
///     The consecutive click count for press events, as normalized by runtime policy.
///     Non-press events may report <c>0</c>.
/// </param>
public sealed record PointerInput(
    PointerEventKind Kind,
    PointerButton Button,
    int X,
    int Y,
    ModifierKeys Modifiers = ModifierKeys.None,
    int ClickCount = 1) : Message;

/// <summary>
///     Represents the start of a bracketed paste sequence.
/// </summary>
public sealed record PasteStarted : Message;

/// <summary>
///     Represents the end of a bracketed paste sequence.
/// </summary>
public sealed record PasteEnded : Message;

/// <summary>
///     Represents pasted text content.
/// </summary>
/// <param name="Content">The pasted content.</param>
public sealed record Pasted(string Content) : Message;

/// <summary>
///     Represents a terminal focus change.
/// </summary>
/// <param name="IsFocused"><see langword="true" /> when the terminal gained focus; otherwise, <see langword="false" />.</param>
public sealed record FocusChanged(bool IsFocused) : Message;

/// <summary>
///     Represents an exception surfaced to the application as a message.
/// </summary>
/// <param name="Exception">The exception that was captured.</param>
public sealed record Faulted(Exception Exception) : Message;

/// <summary>
///     Represents a message that wraps an external runtime payload.
/// </summary>
/// <param name="Raw">The raw payload supplied by the runtime or host.</param>
public sealed record ExternalMessage(object Raw) : Message;

/// <summary>
///     Identifies a keyboard key independent of platform-specific key codes.
/// </summary>
public enum Key
{
    /// <summary>
    ///     The value could not be determined.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The character value.
    /// </summary>
    Character,

    /// <summary>
    ///     The enter value.
    /// </summary>
    Enter,

    /// <summary>
    ///     The tab value.
    /// </summary>
    Tab,

    /// <summary>
    ///     The escape value.
    /// </summary>
    Escape,

    /// <summary>
    ///     The backspace value.
    /// </summary>
    Backspace,

    /// <summary>
    ///     The up value.
    /// </summary>
    Up,

    /// <summary>
    ///     The down value.
    /// </summary>
    Down,

    /// <summary>
    ///     The left value.
    /// </summary>
    Left,

    /// <summary>
    ///     The right value.
    /// </summary>
    Right,

    /// <summary>
    ///     The home value.
    /// </summary>
    Home,

    /// <summary>
    ///     The end value.
    /// </summary>
    End,

    /// <summary>
    ///     The page up value.
    /// </summary>
    PageUp,

    /// <summary>
    ///     The page down value.
    /// </summary>
    PageDown,

    /// <summary>
    ///     The insert value.
    /// </summary>
    Insert,

    /// <summary>
    ///     The delete value.
    /// </summary>
    Delete,

    /// <summary>
    ///     The f 1 value.
    /// </summary>
    F1,

    /// <summary>
    ///     The f 2 value.
    /// </summary>
    F2,

    /// <summary>
    ///     The f 3 value.
    /// </summary>
    F3,

    /// <summary>
    ///     The f 4 value.
    /// </summary>
    F4,

    /// <summary>
    ///     The f 5 value.
    /// </summary>
    F5,

    /// <summary>
    ///     The f 6 value.
    /// </summary>
    F6,

    /// <summary>
    ///     The f 7 value.
    /// </summary>
    F7,

    /// <summary>
    ///     The f 8 value.
    /// </summary>
    F8,

    /// <summary>
    ///     The f 9 value.
    /// </summary>
    F9,

    /// <summary>
    ///     The f 10 value.
    /// </summary>
    F10,

    /// <summary>
    ///     The f 11 value.
    /// </summary>
    F11,

    /// <summary>
    ///     The f 12 value.
    /// </summary>
    F12
}

/// <summary>
///     Represents keyboard modifiers active during an input event.
/// </summary>
[Flags]
public enum ModifierKeys
{
    /// <summary>
    ///     No value is selected.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The shift value.
    /// </summary>
    Shift = 1 << 0,

    /// <summary>
    ///     The alt value.
    /// </summary>
    Alt = 1 << 1,

    /// <summary>
    ///     The ctrl value.
    /// </summary>
    Ctrl = 1 << 2,

    /// <summary>
    ///     The meta value.
    /// </summary>
    Meta = 1 << 3
}

/// <summary>
///     Identifies the kind of pointer event that occurred.
/// </summary>
public enum PointerEventKind
{
    /// <summary>
    ///     The press value.
    /// </summary>
    Press = 0,

    /// <summary>
    ///     The release value.
    /// </summary>
    Release = 1,

    /// <summary>
    ///     The motion value.
    /// </summary>
    Motion = 2,

    /// <summary>
    ///     The wheel value.
    /// </summary>
    Wheel = 3
}

/// <summary>
///     Identifies the pointer button involved in a pointer event.
/// </summary>
public enum PointerButton
{
    /// <summary>
    ///     No value is selected.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The left value.
    /// </summary>
    Left = 1,

    /// <summary>
    ///     The middle value.
    /// </summary>
    Middle = 2,

    /// <summary>
    ///     The right value.
    /// </summary>
    Right = 3,

    /// <summary>
    ///     The wheel up value.
    /// </summary>
    WheelUp = 4,

    /// <summary>
    ///     The wheel down value.
    /// </summary>
    WheelDown = 5,

    /// <summary>
    ///     The wheel left value.
    /// </summary>
    WheelLeft = 6,

    /// <summary>
    ///     The wheel right value.
    /// </summary>
    WheelRight = 7,

    /// <summary>
    ///     The backward value.
    /// </summary>
    Backward = 8,

    /// <summary>
    ///     The forward value.
    /// </summary>
    Forward = 9,

    /// <summary>
    ///     The button 10 value.
    /// </summary>
    Button10 = 10,

    /// <summary>
    ///     The button 11 value.
    /// </summary>
    Button11 = 11,

    /// <summary>
    ///     The button 12 value.
    /// </summary>
    Button12 = 12,

    /// <summary>
    ///     The button 13 value.
    /// </summary>
    Button13 = 13,

    /// <summary>
    ///     The button 14 value.
    /// </summary>
    Button14 = 14,

    /// <summary>
    ///     The button 15 value.
    /// </summary>
    Button15 = 15,

    /// <summary>
    ///     The button 16 value.
    /// </summary>
    Button16 = 16,

    /// <summary>
    ///     The button 17 value.
    /// </summary>
    Button17 = 17,

    /// <summary>
    ///     The button 18 value.
    /// </summary>
    Button18 = 18,

    /// <summary>
    ///     The button 19 value.
    /// </summary>
    Button19 = 19,

    /// <summary>
    ///     The button 20 value.
    /// </summary>
    Button20 = 20,

    /// <summary>
    ///     The button 21 value.
    /// </summary>
    Button21 = 21,

    /// <summary>
    ///     The button 22 value.
    /// </summary>
    Button22 = 22,

    /// <summary>
    ///     The button 23 value.
    /// </summary>
    Button23 = 23,

    /// <summary>
    ///     The button 24 value.
    /// </summary>
    Button24 = 24
}

/// <summary>
///     Defines the mouse tracking mode requested for a screen.
/// </summary>
public enum MouseTrackingMode
{
    /// <summary>
    ///     No value is selected.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The cell motion value.
    /// </summary>
    CellMotion = 1,

    /// <summary>
    ///     The all motion value.
    /// </summary>
    AllMotion = 2
}

/// <summary>
///     Defines pointer activation behavior used by the runtime input pipeline.
/// </summary>
public enum PointerActivationPolicy
{
    /// <summary>
    ///     Activates pointer-driven interactions on a single click.
    /// </summary>
    SingleClick = 0,

    /// <summary>
    ///     Activates pointer-driven interactions on double click.
    /// </summary>
    DoubleClick = 1
}
