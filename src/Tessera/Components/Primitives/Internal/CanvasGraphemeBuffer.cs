using System.Globalization;
using System.Text;

namespace Tessera.Components.Primitives.Internal;

internal sealed class CanvasGraphemeBuffer(int width, int height)
{
    private readonly string?[] _cells = new string?[width * height];
    private readonly bool[] _continuations = new bool[width * height];
    private readonly int _height = height;
    private readonly int _width = width;

    public void Clear(char fill = ' ')
    {
        Array.Clear(_cells, 0, _cells.Length);
        Array.Clear(_continuations, 0, _continuations.Length);
        if (fill == ' ')
        {
            return;
        }

        var text = fill.ToString();
        for (var i = 0; i < _cells.Length; i++)
        {
            _cells[i] = text;
        }
    }

    public void Set(int x, int y, char value)
    {
        SetCell(x, y, value.ToString(), 1);
    }

    public char Get(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height)
        {
            return '\0';
        }

        var index = y * _width + x;
        if (_continuations[index] || _cells[index] is not { } element || element.Length == 0)
        {
            return '\0';
        }

        return element[0];
    }

    public void WriteText(int x, int y, string text, int maxWidth)
    {
        var cx = x;
        var written = 0;
        var index = 0;
        var lastColumn = -1;
        var pendingZeroWidth = string.Empty;
        var sawAnsi = false;
        var truncated = false;

        while (index < text.Length)
        {
            if (CanvasAnsiScanner.TryReadEscape(text, index, out var consumed))
            {
                sawAnsi = true;
                if (lastColumn >= 0)
                {
                    var previousIndex = y * _width + lastColumn;
                    if (_cells[previousIndex] is { } previous)
                    {
                        _cells[previousIndex] = ConcatSlice(previous, text, index, consumed);
                    }
                }
                else
                {
                    pendingZeroWidth = ConcatSlice(pendingZeroWidth, text, index, consumed);
                }

                index += consumed;
                continue;
            }

            var element = StringInfo.GetNextTextElement(text, index);
            index += element.Length;

            if (element is "\r" or "\n")
            {
                break;
            }

            var elementWidth = TextElementWidth.Measure(element);
            if (elementWidth <= 0)
            {
                if (lastColumn >= 0)
                {
                    var previousIndex = y * _width + lastColumn;
                    if (_cells[previousIndex] is { } previous)
                    {
                        _cells[previousIndex] = string.Concat(previous, element);
                    }
                }
                else
                {
                    pendingZeroWidth = string.Concat(pendingZeroWidth, element);
                }

                continue;
            }

            if (written + elementWidth > maxWidth)
            {
                truncated = true;
                break;
            }

            if (cx >= 0 && cx < _width)
            {
                if (elementWidth == 1 || cx + 1 < _width)
                {
                    var value = pendingZeroWidth.Length == 0
                        ? element
                        : string.Concat(pendingZeroWidth, element);
                    pendingZeroWidth = string.Empty;
                    SetCell(cx, y, value, elementWidth);
                    lastColumn = cx;
                }
                else
                {
                    truncated = true;
                    break;
                }
            }
            else
            {
                lastColumn = -1;
            }

            cx += elementWidth;
            written += elementWidth;
        }

        if (truncated && sawAnsi)
        {
            if (lastColumn >= 0)
            {
                var previousIndex = y * _width + lastColumn;
                if (_cells[previousIndex] is { } previous &&
                    !previous.EndsWith("\e[0m", StringComparison.Ordinal))
                {
                    _cells[previousIndex] = string.Concat(previous, "\e[0m");
                }
            }
            else
            {
                pendingZeroWidth = string.Concat(pendingZeroWidth, "\e[0m");
            }
        }

        if (pendingZeroWidth.Length > 0 && lastColumn >= 0)
        {
            var previousIndex = y * _width + lastColumn;
            if (_cells[previousIndex] is { } previous)
            {
                _cells[previousIndex] = string.Concat(previous, pendingZeroWidth);
            }
        }
    }

    public string Render()
    {
        var sb = new StringBuilder((_width + 1) * _height);
        for (var y = 0; y < _height; y++)
        {
            if (y > 0)
            {
                sb.Append('\n');
            }

            var rowStart = y * _width;
            for (var x = 0; x < _width; x++)
            {
                var index = rowStart + x;
                if (_continuations[index])
                {
                    continue;
                }

                sb.Append(_cells[index] ?? " ");
            }
        }

        return sb.ToString();
    }

    private void SetCell(int x, int y, string value, int width)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height || width <= 0)
        {
            return;
        }

        var index = y * _width + x;
        ClearCell(index);
        _cells[index] = value;
        _continuations[index] = false;

        if (width <= 1)
        {
            return;
        }

        var nextColumn = x + 1;
        if (nextColumn >= _width)
        {
            return;
        }

        var nextIndex = y * _width + nextColumn;
        ClearCell(nextIndex);
        _cells[nextIndex] = null;
        _continuations[nextIndex] = true;
    }

    private void ClearCell(int index)
    {
        if (_continuations[index])
        {
            var previous = index - 1;
            if (previous >= 0 && previous / _width == index / _width)
            {
                _cells[previous] = null;
                _continuations[previous] = false;
            }

            _continuations[index] = false;
            _cells[index] = null;
            return;
        }

        if (_cells[index] is { } existing && TextElementWidth.Measure(existing) > 1)
        {
            var next = index + 1;
            if (next < _cells.Length && next / _width == index / _width && _continuations[next])
            {
                _continuations[next] = false;
                _cells[next] = null;
            }
        }

        _cells[index] = null;
        _continuations[index] = false;
    }

    private static string ConcatSlice(string prefix, string source, int start, int length)
    {
        if (length <= 0)
        {
            return prefix;
        }

        if (prefix.Length == 0)
        {
            return source.Substring(start, length);
        }

        return string.Create(prefix.Length + length, (prefix, source, start, length), static (destination, state) =>
        {
            state.prefix.AsSpan().CopyTo(destination);
            state.source.AsSpan(state.start, state.length).CopyTo(destination[state.prefix.Length..]);
        });
    }
}
