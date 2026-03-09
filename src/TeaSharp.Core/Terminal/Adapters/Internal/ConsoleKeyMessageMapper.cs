using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Terminal;

internal static class ConsoleKeyMessageMapper
{
    public static IMessage? Map(ConsoleKeyInfo key)
    {
        var modifiers = KeyModifiers.None;
        if ((key.Modifiers & ConsoleModifiers.Control) != 0)
        {
            modifiers |= KeyModifiers.Ctrl;
        }

        if ((key.Modifiers & ConsoleModifiers.Alt) != 0)
        {
            modifiers |= KeyModifiers.Alt;
        }

        if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
        {
            modifiers |= KeyModifiers.Shift;
        }

        return key.Key switch
        {
            ConsoleKey.UpArrow => new KeyPressMsg(KeyCode.Up, string.Empty, modifiers),
            ConsoleKey.DownArrow => new KeyPressMsg(KeyCode.Down, string.Empty, modifiers),
            ConsoleKey.LeftArrow => new KeyPressMsg(KeyCode.Left, string.Empty, modifiers),
            ConsoleKey.RightArrow => new KeyPressMsg(KeyCode.Right, string.Empty, modifiers),
            ConsoleKey.Enter => new KeyPressMsg(KeyCode.Enter, string.Empty, modifiers),
            ConsoleKey.Tab => new KeyPressMsg(KeyCode.Tab, string.Empty, modifiers),
            ConsoleKey.Backspace => new KeyPressMsg(KeyCode.Backspace, string.Empty, modifiers),
            ConsoleKey.Escape => new KeyPressMsg(KeyCode.Escape, string.Empty, modifiers),
            ConsoleKey.F1 => new KeyPressMsg(KeyCode.F1, string.Empty, modifiers),
            ConsoleKey.F2 => new KeyPressMsg(KeyCode.F2, string.Empty, modifiers),
            ConsoleKey.F3 => new KeyPressMsg(KeyCode.F3, string.Empty, modifiers),
            ConsoleKey.F4 => new KeyPressMsg(KeyCode.F4, string.Empty, modifiers),
            ConsoleKey.F5 => new KeyPressMsg(KeyCode.F5, string.Empty, modifiers),
            ConsoleKey.F6 => new KeyPressMsg(KeyCode.F6, string.Empty, modifiers),
            ConsoleKey.F7 => new KeyPressMsg(KeyCode.F7, string.Empty, modifiers),
            ConsoleKey.F8 => new KeyPressMsg(KeyCode.F8, string.Empty, modifiers),
            ConsoleKey.F9 => new KeyPressMsg(KeyCode.F9, string.Empty, modifiers),
            ConsoleKey.F10 => new KeyPressMsg(KeyCode.F10, string.Empty, modifiers),
            ConsoleKey.F11 => new KeyPressMsg(KeyCode.F11, string.Empty, modifiers),
            ConsoleKey.F12 => new KeyPressMsg(KeyCode.F12, string.Empty, modifiers),
            _ => ToCharacterMessage(key, modifiers),
        };
    }

    private static IMessage? ToCharacterMessage(ConsoleKeyInfo key, KeyModifiers modifiers)
    {
        if (modifiers.HasFlag(KeyModifiers.Ctrl) && key.Key is >= ConsoleKey.A and <= ConsoleKey.Z)
        {
            var ch = (char)('a' + (key.Key - ConsoleKey.A));
            return new KeyPressMsg(KeyCode.Character, ch.ToString(), modifiers);
        }

        if (key.KeyChar == '\0')
        {
            return null;
        }

        if (key.KeyChar == '\u0003')
        {
            return new KeyPressMsg(KeyCode.Character, "c", modifiers | KeyModifiers.Ctrl);
        }

        return new KeyPressMsg(KeyCode.Character, key.KeyChar.ToString(), modifiers);
    }
}
