namespace Tessera.Widgets.Internal;

internal static class TextInputFrameBuilder
{
    public static TextInputFrame Build(
        string value,
        string placeholder,
        bool multiline,
        bool maskInput,
        char maskCharacter,
        int cursor,
        int width)
    {
        if (width <= 0)
        {
            return new TextInputFrame(string.Empty, 0, PlaceholderVisible: false);
        }

        var isPlaceholder = value.Length == 0;
        var raw = isPlaceholder ? placeholder : value;
        var visible = maskInput && !isPlaceholder
            ? new string(maskCharacter, raw.Length)
            : raw;
        var currentLineRange = (Start: 0, End: visible.Length);

        if (multiline && !isPlaceholder)
        {
            currentLineRange = TextInputSelection.CurrentLineRange(visible, cursor);
            visible = visible[currentLineRange.Start..currentLineRange.End];
        }

        var lineCursor = 0;
        if (!isPlaceholder)
        {
            var unclampedCursor = multiline ? cursor - currentLineRange.Start : cursor;
            lineCursor = Math.Clamp(unclampedCursor, 0, visible.Length);
        }

        var start = 0;
        if (lineCursor >= width)
        {
            start = lineCursor - width + 1;
        }
        else if (visible.Length > width)
        {
            start = Math.Max(0, visible.Length - width);
        }

        start = Math.Clamp(start, 0, Math.Max(0, visible.Length - 1));
        var text = BuildWindowText(visible, start, width);

        var cursorColumn = Math.Clamp(lineCursor - start, 0, Math.Max(0, width - 1));
        return new TextInputFrame(text, cursorColumn, isPlaceholder);
    }

    private static string BuildWindowText(string visible, int start, int width)
    {
        return string.Create(width, (visible, start, width), static (destination, state) =>
        {
            destination.Fill(' ');
            if ((uint)state.start >= (uint)state.visible.Length)
            {
                return;
            }

            var sourceLength = Math.Min(state.width, state.visible.Length - state.start);
            state.visible.AsSpan(state.start, sourceLength).CopyTo(destination);
        });
    }
}
