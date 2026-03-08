using System.Text;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Messages;

[Flags]
public enum KeyModifiers
{
    None = 0,
    Shift = 1 << 0,
    Alt = 1 << 1,
    Ctrl = 1 << 2,
    Meta = 1 << 3,
}

public enum KeyCode
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

public sealed record KeyPressMsg(
    KeyCode Code,
    string Text = "",
    KeyModifiers Modifiers = KeyModifiers.None,
    bool IsRepeat = false) : IMessage
{
    public bool Is(KeyCode code, KeyModifiers modifiers = KeyModifiers.None)
    {
        return Code == code && Modifiers == modifiers;
    }

    public bool IsCharacter(char character, bool ignoreCase = true)
    {
        if (Code != KeyCode.Character || Text.Length != 1)
        {
            return false;
        }

        var value = Text[0];
        return ignoreCase
            ? char.ToLowerInvariant(value) == char.ToLowerInvariant(character)
            : value == character;
    }

    public bool IsCharacter(char character, KeyModifiers modifiers, bool ignoreCase = true)
    {
        return Modifiers == modifiers && IsCharacter(character, ignoreCase);
    }

    public bool TryGetDigit(out int oneBasedDigit)
    {
        oneBasedDigit = 0;
        if (Code != KeyCode.Character || Text.Length != 1)
        {
            return false;
        }

        var c = Text[0];
        if (!char.IsDigit(c))
        {
            return false;
        }

        oneBasedDigit = c - '0';
        return true;
    }

    public string Keystroke()
    {
        var parts = new List<string>(4);
        if (Modifiers.HasFlag(KeyModifiers.Ctrl)) parts.Add("ctrl");
        if (Modifiers.HasFlag(KeyModifiers.Alt)) parts.Add("alt");
        if (Modifiers.HasFlag(KeyModifiers.Shift)) parts.Add("shift");
        if (Modifiers.HasFlag(KeyModifiers.Meta)) parts.Add("meta");

        var key = Code == KeyCode.Character
            ? Text
            : Code.ToString().ToLowerInvariant();

        parts.Add(string.IsNullOrEmpty(key) ? "unknown" : key);

        var sb = new StringBuilder();
        for (var i = 0; i < parts.Count; i++)
        {
            if (i > 0) sb.Append('+');
            sb.Append(parts[i]);
        }

        return sb.ToString();
    }
}

public sealed record KeyReleaseMsg(
    KeyCode Code,
    string Text = "",
    KeyModifiers Modifiers = KeyModifiers.None) : IMessage;
