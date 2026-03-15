using System.ComponentModel;
using TeaSharp.Internal;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class KeyBinding
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

    public bool Matches(KeyPressed key)
    {
        return KeyChord.TryFromKeyPress(key, out var chord) && _chords.Contains(chord);
    }

    public bool Matches(global::TeaSharp.Core.Messages.KeyPressMsg key)
    {
        return TeaMessageAdapter.ToPublic(key) is KeyPressed mapped && Matches(mapped);
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

    private readonly record struct KeyChord(Key Code, char Character, ModifierKeys Modifiers)
    {
        public static bool TryFromKeyPress(KeyPressed key, out KeyChord chord)
        {
            chord = default;
            if (key.Key == Key.Character)
            {
                if (key.Text.Length != 1)
                {
                    return false;
                }

                chord = new KeyChord(Key.Character, char.ToLowerInvariant(key.Text[0]), key.Modifiers);
                return true;
            }

            chord = new KeyChord(key.Key, '\0', key.Modifiers);
            return true;
        }

        public static bool TryParse(string chord, out KeyChord parsed)
        {
            parsed = default;
            if (string.IsNullOrWhiteSpace(chord))
            {
                return false;
            }

            if (string.Equals(chord, "+", StringComparison.Ordinal))
            {
                parsed = new KeyChord(Key.Character, '+', ModifierKeys.None);
                return true;
            }

            var modifiers = ModifierKeys.None;
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
                parsed = new KeyChord(Key.Character, char.ToLowerInvariant(keyToken[0]), modifiers);
                return true;
            }

            return false;
        }

        private static bool TryParseModifier(string token, out ModifierKeys modifier)
        {
            modifier = token switch
            {
                "ctrl" or "control" => ModifierKeys.Ctrl,
                "alt" => ModifierKeys.Alt,
                "shift" => ModifierKeys.Shift,
                "meta" => ModifierKeys.Meta,
                _ => ModifierKeys.None,
            };

            return modifier != ModifierKeys.None;
        }

        private static bool TryParseKeyCode(string token, out Key keyCode, out char character)
        {
            character = '\0';
            keyCode = token switch
            {
                "enter" or "return" => Key.Enter,
                "tab" => Key.Tab,
                "esc" or "escape" => Key.Escape,
                "backspace" => Key.Backspace,
                "up" => Key.Up,
                "down" => Key.Down,
                "left" => Key.Left,
                "right" => Key.Right,
                "home" => Key.Home,
                "end" => Key.End,
                "pageup" or "pgup" => Key.PageUp,
                "pagedown" or "pgdn" => Key.PageDown,
                "insert" => Key.Insert,
                "delete" => Key.Delete,
                "f1" => Key.F1,
                "f2" => Key.F2,
                "f3" => Key.F3,
                "f4" => Key.F4,
                "f5" => Key.F5,
                "f6" => Key.F6,
                "f7" => Key.F7,
                "f8" => Key.F8,
                "f9" => Key.F9,
                "f10" => Key.F10,
                "f11" => Key.F11,
                "f12" => Key.F12,
                "space" => Key.Character,
                "plus" => Key.Character,
                _ => Key.Unknown,
            };

            if (keyCode == Key.Unknown)
            {
                return false;
            }

            if (keyCode == Key.Character && token == "space")
            {
                character = ' ';
            }
            else if (keyCode == Key.Character && token == "plus")
            {
                character = '+';
            }

            return true;
        }
    }
}
