namespace Tessera.Components.Primitives.Internal;

internal static class CanvasAnsiScanner
{
    public static bool TryReadEscape(string text, int start, out int consumed)
    {
        consumed = 0;
        if (start < 0 || start >= text.Length || text[start] != '\e' || start + 1 >= text.Length)
        {
            return false;
        }

        if (text[start + 1] != '[')
        {
            return false;
        }

        var cursor = start + 2;
        while (cursor < text.Length)
        {
            var ch = text[cursor];
            if (ch >= '@' && ch <= '~')
            {
                consumed = cursor - start + 1;
                return true;
            }

            if (ch < '\u0020')
            {
                return false;
            }

            cursor++;
        }

        return false;
    }

    public static bool TryReadEscape(string text, int start, out string sequence, out int consumed)
    {
        if (!TryReadEscape(text, start, out consumed))
        {
            sequence = string.Empty;
            return false;
        }

        sequence = text.Substring(start, consumed);
        return true;
    }
}
