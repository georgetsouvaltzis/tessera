using TeaSharp.Core.Messages;

namespace TeaSharp.Widgets;

public sealed class KeyBinding
{
    private readonly HashSet<string> _chords;

    public KeyBinding(string keys, string description, params string[] chords)
    {
        Keys = keys;
        Description = description;
        _chords = [];

        if (chords.Length == 0)
        {
            foreach (var chord in keys.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                _chords.Add(NormalizeChord(chord));
            }
        }
        else
        {
            foreach (var chord in chords)
            {
                _chords.Add(NormalizeChord(chord));
            }
        }
    }

    public string Keys { get; }

    public string Description { get; }

    public bool Matches(KeyPressMsg key)
    {
        var stroke = NormalizeChord(key.Keystroke());
        return _chords.Contains(stroke);
    }

    public bool Matches(string chord)
    {
        return _chords.Contains(NormalizeChord(chord));
    }

    public static string NormalizeChord(string chord)
    {
        return chord.Trim().ToLowerInvariant();
    }
}
