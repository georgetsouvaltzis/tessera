using System.Diagnostics.CodeAnalysis;

namespace TeaSharp;

public abstract record Message;

public sealed record KeyPressed(
    Key Key,
    string Text = "",
    ModifierKeys Modifiers = ModifierKeys.None,
    bool IsRepeat = false) : Message
{
    public bool Is(Key key, ModifierKeys modifiers = ModifierKeys.None)
    {
        return Key == key && Modifiers == modifiers;
    }

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

    public bool IsCharacter(char character, ModifierKeys modifiers, bool ignoreCase = true)
    {
        return Modifiers == modifiers && IsCharacter(character, ignoreCase);
    }
}

public sealed record KeyReleased(
    Key Key,
    string Text = "",
    ModifierKeys Modifiers = ModifierKeys.None) : Message;

public sealed record WindowResized(int Width, int Height) : Message;

public sealed record PointerInput(
    PointerEventKind Kind,
    PointerButton Button,
    int X,
    int Y,
    ModifierKeys Modifiers = ModifierKeys.None) : Message;

public sealed record PasteStarted : Message;

public sealed record PasteEnded : Message;

public sealed record Pasted(string Content) : Message;

public sealed record FocusChanged(bool IsFocused) : Message;

public sealed record Faulted(Exception Exception) : Message;

public sealed record ExternalMessage(object Raw) : Message;

internal sealed record RuntimeMessage(
    global::TeaSharp.Core.Abstractions.IMessage Raw) : Message;

internal sealed record MessageEnvelope(Message Message) : global::TeaSharp.Core.Abstractions.IMessage;

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

[Flags]
public enum ModifierKeys
{
    None = 0,
    Shift = 1 << 0,
    Alt = 1 << 1,
    Ctrl = 1 << 2,
    Meta = 1 << 3,
}

public enum PointerEventKind
{
    Press = 0,
    Release = 1,
    Motion = 2,
    Wheel = 3,
}

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

public enum MouseTrackingMode
{
    None = 0,
    CellMotion = 1,
    AllMotion = 2,
}
