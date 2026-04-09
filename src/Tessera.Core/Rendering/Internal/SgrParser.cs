using System.Buffers;

namespace Tessera.Core.Rendering.Internal;

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

        ApplyParameters(text.AsSpan(index + 2, cursor - (index + 2)), ref state);
        currentStyle = state.ToEscapeSequence();
        index = cursor + 1;
        return true;
    }

    private static void ApplyParameters(ReadOnlySpan<char> parameters, ref SgrStyleState state)
    {
        if (parameters.Length == 0)
        {
            Span<int> reset = stackalloc int[1];
            reset[0] = 0;
            state.Apply(reset);
            return;
        }

        Span<int> stack = stackalloc int[16];
        var values = stack;
        int[]? rented = null;
        var count = 0;
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
                EnsureCapacity(ref values, ref rented, count + 1);
                values[count++] = hasValue ? value : 0;
                value = 0;
                hasValue = false;
            }
        }

        EnsureCapacity(ref values, ref rented, count + 1);
        values[count++] = hasValue ? value : 0;
        state.Apply(values[..count]);

        if (rented is not null)
        {
            ArrayPool<int>.Shared.Return(rented);
        }
    }

    private static void EnsureCapacity(ref Span<int> values, ref int[]? rented, int required)
    {
        if (required <= values.Length)
        {
            return;
        }

        var newSize = values.Length * 2;
        while (newSize < required)
        {
            newSize *= 2;
        }

        var next = ArrayPool<int>.Shared.Rent(newSize);
        values.CopyTo(next);
        if (rented is not null)
        {
            ArrayPool<int>.Shared.Return(rented);
        }

        rented = next;
        values = next;
    }
}
