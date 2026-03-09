namespace TeaSharp.Core.Rendering;

internal static class SgrParser
{
    public static bool TryRead(string text, ref int index, ref SgrStyleState state, out string currentStyle)
    {
        currentStyle = state.ToEscapeSequence();

        if (text[index] != '\u001b')
        {
            return false;
        }

        if (index + 2 >= text.Length || text[index + 1] != '[')
        {
            return false;
        }

        var cursor = index + 2;
        while (cursor < text.Length && text[cursor] != 'm')
        {
            var ch = text[cursor];
            if (!char.IsDigit(ch) && ch != ';' && ch != ':')
            {
                return false;
            }

            cursor++;
        }

        if (cursor >= text.Length || text[cursor] != 'm')
        {
            return false;
        }

        state.Apply(ParseParameters(text.AsSpan(index + 2, cursor - (index + 2))));
        currentStyle = state.ToEscapeSequence();
        index = cursor + 1;
        return true;
    }

    private static int[] ParseParameters(ReadOnlySpan<char> parameters)
    {
        if (parameters.Length == 0)
        {
            return [0];
        }

        var values = new List<int>(8);
        var value = 0;
        var hasValue = false;

        foreach (var ch in parameters)
        {
            if (char.IsDigit(ch))
            {
                value = (value * 10) + (ch - '0');
                hasValue = true;
                continue;
            }

            if (ch is ';' or ':')
            {
                values.Add(hasValue ? value : 0);
                value = 0;
                hasValue = false;
            }
        }

        values.Add(hasValue ? value : 0);
        return [.. values];
    }
}
