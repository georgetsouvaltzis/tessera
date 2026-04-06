namespace Tessera.Widgets.Internal;

internal static class TextInputSelection
{
    public static TextInputBufferState MoveCursor(TextInputBufferState state, int target, bool extendSelection)
    {
        var clamped = Math.Clamp(target, 0, state.Value.Length);
        int? anchor = extendSelection
            ? state.SelectionAnchor ?? state.Cursor
            : null;
        return state with
        {
            Cursor = clamped,
            SelectionAnchor = anchor,
        };
    }

    public static (int Start, int End) Range(TextInputBufferState state)
    {
        var anchor = state.SelectionAnchor ?? state.Cursor;
        return (Math.Min(anchor, state.Cursor), Math.Max(anchor, state.Cursor));
    }

    public static int FindWordBoundaryLeft(string value, int cursor)
    {
        var i = Math.Clamp(cursor, 0, value.Length);
        while (i > 0 && !IsWordChar(value[i - 1]))
        {
            i--;
        }

        while (i > 0 && IsWordChar(value[i - 1]))
        {
            i--;
        }

        return i;
    }

    public static int FindWordBoundaryRight(string value, int cursor)
    {
        var i = Math.Clamp(cursor, 0, value.Length);
        while (i < value.Length && !IsWordChar(value[i]))
        {
            i++;
        }

        while (i < value.Length && IsWordChar(value[i]))
        {
            i++;
        }

        return i;
    }

    public static int MoveVerticalLine(string value, int cursor, int direction)
    {
        var (lineStart, lineEnd) = CurrentLineRange(value, cursor);
        var column = cursor - lineStart;

        if (direction < 0)
        {
            if (lineStart == 0)
            {
                return cursor;
            }

            var previousLineEnd = lineStart - 1;
            var previousLineStart = value.LastIndexOf('\n', Math.Max(0, previousLineEnd - 1));
            previousLineStart = previousLineStart < 0 ? 0 : previousLineStart + 1;
            var previousLength = previousLineEnd - previousLineStart;
            return previousLineStart + Math.Min(column, Math.Max(0, previousLength));
        }

        if (lineEnd >= value.Length)
        {
            return cursor;
        }

        var nextLineStart = lineEnd + 1;
        var nextLineEnd = value.IndexOf('\n', nextLineStart);
        if (nextLineEnd < 0)
        {
            nextLineEnd = value.Length;
        }

        var nextLength = nextLineEnd - nextLineStart;
        return nextLineStart + Math.Min(column, Math.Max(0, nextLength));
    }

    public static (int Start, int End) CurrentLineRange(string value, int cursor)
    {
        if (value.Length == 0)
        {
            return (0, 0);
        }

        var clampedCursor = Math.Clamp(cursor, 0, value.Length);
        var start = value.LastIndexOf('\n', Math.Max(0, clampedCursor - 1));
        start = start < 0 ? 0 : start + 1;

        var end = value.IndexOf('\n', clampedCursor);
        if (end < 0)
        {
            end = value.Length;
        }

        return (start, end);
    }

    private static bool IsWordChar(char value)
    {
        return char.IsLetterOrDigit(value) || value == '_';
    }
}
