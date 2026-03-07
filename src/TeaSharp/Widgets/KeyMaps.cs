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
    public KeyBinding WordLeft { get; } = new("alt+left|alt+b", "word left", "alt+left", "ctrl+left", "alt+b", "meta+b");
    public KeyBinding WordRight { get; } = new("alt+right|alt+f", "word right", "alt+right", "ctrl+right", "alt+f", "meta+f");
    public KeyBinding DeleteBackward { get; } = new("backspace", "delete left", "backspace");
    public KeyBinding DeleteForward { get; } = new("delete", "delete right", "delete");
    public KeyBinding DeleteWordBackward { get; } = new("alt+backspace|alt+h", "delete word left", "alt+backspace", "ctrl+backspace", "alt+h", "meta+h");
    public KeyBinding DeleteWordForward { get; } = new("alt+delete|alt+d", "delete word right", "alt+delete", "ctrl+delete", "alt+d", "meta+d");
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
    public static string RenderColumns(
        IEnumerable<KeyBinding> bindings,
        int maxWidth,
        int minColumnWidth = 24,
        int columnGap = 3)
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
            return string.Join('\n', chunks);
        }

        var contentWidth = Math.Max(minColumnWidth, chunks.Max(chunk => chunk.Length));
        var perColumn = contentWidth + Math.Max(1, columnGap);
        var columns = Math.Max(1, (maxWidth + Math.Max(1, columnGap)) / perColumn);
        if (columns <= 1)
        {
            return string.Join('\n', chunks.Select(chunk => chunk.Length <= maxWidth ? chunk : chunk[..maxWidth]));
        }

        var rows = (int)Math.Ceiling(chunks.Length / (double)columns);
        var lines = new List<string>(rows);
        for (var row = 0; row < rows; row++)
        {
            var line = new System.Text.StringBuilder(maxWidth);
            for (var column = 0; column < columns; column++)
            {
                var index = row + (column * rows);
                if (index >= chunks.Length)
                {
                    continue;
                }

                var chunk = chunks[index];
                var rendered = chunk.Length <= contentWidth
                    ? chunk
                    : chunk[..contentWidth];

                if (line.Length > 0)
                {
                    line.Append(' ', Math.Max(1, columnGap));
                }

                if (column == columns - 1)
                {
                    line.Append(rendered);
                }
                else
                {
                    line.Append(rendered.PadRight(contentWidth));
                }
            }

            lines.Add(line.ToString().TrimEnd());
        }

        return string.Join('\n', lines);
    }

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
