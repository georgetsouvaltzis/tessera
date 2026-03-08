using TeaSharp.Core.Messages;

namespace TeaSharp.Widgets;

public sealed class KeyBinding
{
    private readonly HashSet<KeyChord> _chords;

    public KeyBinding(string keys, string description, params string[] chords)
    {
        Keys = keys;
        Description = description;
        _chords = [];

        if (chords.Length == 0)
        {
            foreach (var chord in keys.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                AddChord(chord);
            }
        }
        else
        {
            foreach (var chord in chords)
            {
                AddChord(chord);
            }
        }
    }

    public string Keys { get; }

    public string Description { get; }

    public bool Matches(KeyPressMsg key)
    {
        return KeyChord.TryFromKeyPress(key, out var chord) && _chords.Contains(chord);
    }

    public bool Matches(string chord)
    {
        return KeyChord.TryParse(chord, out var parsed) && _chords.Contains(parsed);
    }

    public static string NormalizeChord(string chord)
    {
        return chord.Trim().ToLowerInvariant();
    }

    private void AddChord(string chord)
    {
        var normalized = NormalizeChord(chord);
        if (KeyChord.TryParse(normalized, out var parsed))
        {
            _chords.Add(parsed);
            return;
        }

        throw new ArgumentException($"Unsupported key chord '{chord}'.", nameof(chord));
    }

    private readonly record struct KeyChord(KeyCode Code, char Character, KeyModifiers Modifiers)
    {
        public static bool TryFromKeyPress(KeyPressMsg key, out KeyChord chord)
        {
            chord = default;
            if (key.Code == KeyCode.Character)
            {
                if (key.Text.Length != 1)
                {
                    return false;
                }

                chord = new KeyChord(KeyCode.Character, char.ToLowerInvariant(key.Text[0]), key.Modifiers);
                return true;
            }

            chord = new KeyChord(key.Code, '\0', key.Modifiers);
            return true;
        }

        public static bool TryParse(string chord, out KeyChord parsed)
        {
            parsed = default;
            if (string.IsNullOrWhiteSpace(chord))
            {
                return false;
            }

            var modifiers = KeyModifiers.None;
            string? keyToken = null;
            var segments = chord.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (TryParseModifier(segment, out var modifier))
                {
                    modifiers |= modifier;
                    continue;
                }

                if (keyToken is not null)
                {
                    return false;
                }

                keyToken = segment;
            }

            if (keyToken is null)
            {
                return false;
            }

            if (TryParseKeyCode(keyToken, out var keyCode, out var character))
            {
                parsed = new KeyChord(keyCode, character, modifiers);
                return true;
            }

            if (keyToken.Length == 1)
            {
                parsed = new KeyChord(KeyCode.Character, char.ToLowerInvariant(keyToken[0]), modifiers);
                return true;
            }

            return false;
        }

        private static bool TryParseModifier(string token, out KeyModifiers modifier)
        {
            modifier = token switch
            {
                "ctrl" or "control" => KeyModifiers.Ctrl,
                "alt" => KeyModifiers.Alt,
                "shift" => KeyModifiers.Shift,
                "meta" => KeyModifiers.Meta,
                _ => KeyModifiers.None,
            };

            return modifier != KeyModifiers.None;
        }

        private static bool TryParseKeyCode(string token, out KeyCode keyCode, out char character)
        {
            character = '\0';
            keyCode = token switch
            {
                "enter" or "return" => KeyCode.Enter,
                "tab" => KeyCode.Tab,
                "esc" or "escape" => KeyCode.Escape,
                "backspace" => KeyCode.Backspace,
                "up" => KeyCode.Up,
                "down" => KeyCode.Down,
                "left" => KeyCode.Left,
                "right" => KeyCode.Right,
                "home" => KeyCode.Home,
                "end" => KeyCode.End,
                "pageup" or "pgup" => KeyCode.PageUp,
                "pagedown" or "pgdn" => KeyCode.PageDown,
                "insert" => KeyCode.Insert,
                "delete" => KeyCode.Delete,
                "f1" => KeyCode.F1,
                "f2" => KeyCode.F2,
                "f3" => KeyCode.F3,
                "f4" => KeyCode.F4,
                "f5" => KeyCode.F5,
                "f6" => KeyCode.F6,
                "f7" => KeyCode.F7,
                "f8" => KeyCode.F8,
                "f9" => KeyCode.F9,
                "f10" => KeyCode.F10,
                "f11" => KeyCode.F11,
                "f12" => KeyCode.F12,
                "space" => KeyCode.Character,
                _ => KeyCode.Unknown,
            };

            if (keyCode == KeyCode.Unknown)
            {
                return false;
            }

            if (keyCode == KeyCode.Character && token == "space")
            {
                character = ' ';
            }

            return true;
        }
    }
}
