namespace TeaSharp.Widgets;

public interface IWidgetKeyMap
{
    IReadOnlyList<KeyBinding> HelpBindings { get; }
}

public sealed class ViewportKeyMap : IWidgetKeyMap
{
    public static ViewportKeyMap Default { get; } = new();

    public KeyBinding Up { get; } = new("up/k", "scroll up", "up", "k");
    public KeyBinding Down { get; } = new("down/j", "scroll down", "down", "j");
    public KeyBinding PageUp { get; } = new("pgup", "page up", "pageup");
    public KeyBinding PageDown { get; } = new("pgdn", "page down", "pagedown");
    public KeyBinding Home { get; } = new("home", "top", "home");
    public KeyBinding End { get; } = new("end", "bottom", "end");
    public KeyBinding Left { get; } = new("left/h", "scroll left", "left", "h");
    public KeyBinding Right { get; } = new("right/l", "scroll right", "right", "l");

    public IReadOnlyList<KeyBinding> HelpBindings =>
    [
        Up,
        Down,
        PageUp,
        PageDown,
        Left,
        Right,
        Home,
        End,
    ];
}

public sealed class TextInputKeyMap : IWidgetKeyMap
{
    public static TextInputKeyMap Default { get; } = new();

    public KeyBinding Submit { get; } = new("enter", "submit", "enter");
    public KeyBinding Left { get; } = new("left", "cursor left", "left");
    public KeyBinding Right { get; } = new("right", "cursor right", "right");
    public KeyBinding Home { get; } = new("home", "line start", "home");
    public KeyBinding End { get; } = new("end", "line end", "end");
    public KeyBinding WordLeft { get; } = new("alt+left", "word left", "alt+left", "ctrl+left");
    public KeyBinding WordRight { get; } = new("alt+right", "word right", "alt+right", "ctrl+right");
    public KeyBinding DeleteBackward { get; } = new("backspace", "delete left", "backspace");
    public KeyBinding DeleteForward { get; } = new("delete", "delete right", "delete");
    public KeyBinding DeleteWordBackward { get; } = new("alt+backspace", "delete word left", "alt+backspace", "ctrl+backspace");
    public KeyBinding DeleteWordForward { get; } = new("alt+delete", "delete word right", "alt+delete", "ctrl+delete");
    public KeyBinding SelectAll { get; } = new("ctrl+a", "select all", "ctrl+a");

    public IReadOnlyList<KeyBinding> HelpBindings =>
    [
        Submit,
        Left,
        Right,
        WordLeft,
        WordRight,
        DeleteBackward,
        DeleteForward,
        SelectAll,
    ];
}

public sealed class ListKeyMap : IWidgetKeyMap
{
    public static ListKeyMap Default { get; } = new();

    public KeyBinding Up { get; } = new("up/k", "move up", "up", "k");
    public KeyBinding Down { get; } = new("down/j", "move down", "down", "j");
    public KeyBinding PageUp { get; } = new("pgup", "page up", "pageup");
    public KeyBinding PageDown { get; } = new("pgdn", "page down", "pagedown");
    public KeyBinding Home { get; } = new("home", "first item", "home");
    public KeyBinding End { get; } = new("end", "last item", "end");

    public IReadOnlyList<KeyBinding> HelpBindings =>
    [
        Up,
        Down,
        PageUp,
        PageDown,
        Home,
        End,
    ];
}

public static class HelpView
{
    public static string RenderCompact(IEnumerable<KeyBinding> bindings, int maxWidth = 0)
    {
        var chunks = bindings
            .Select(binding => $"{binding.Keys} {binding.Description}")
            .ToArray();
        if (chunks.Length == 0)
        {
            return string.Empty;
        }

        if (maxWidth <= 0)
        {
            return string.Join("  |  ", chunks);
        }

        var lines = new List<string>();
        var current = string.Empty;
        foreach (var chunk in chunks)
        {
            var candidate = current.Length == 0
                ? chunk
                : $"{current}  |  {chunk}";
            if (candidate.Length <= maxWidth)
            {
                current = candidate;
                continue;
            }

            if (current.Length > 0)
            {
                lines.Add(current);
            }

            current = chunk.Length <= maxWidth
                ? chunk
                : chunk[..maxWidth];
        }

        if (current.Length > 0)
        {
            lines.Add(current);
        }

        return string.Join('\n', lines);
    }
}
