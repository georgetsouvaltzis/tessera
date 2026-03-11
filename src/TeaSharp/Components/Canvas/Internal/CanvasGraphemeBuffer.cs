using System.Globalization;
using System.Text;

namespace TeaSharp.Components.Primitives.Internal;

internal sealed class CanvasGraphemeBuffer
{
    private readonly int _width;
    private readonly int _height;
    private readonly string?[] _cells;
    private readonly bool[] _continuations;

    public CanvasGraphemeBuffer(int width, int height)
    {
        _width = width;
        _height = height;
        _cells = new string?[width * height];
        _continuations = new bool[width * height];
    }

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
        SetCell(x, y, value.ToString(), width: 1);
    }

    public char Get(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height)
        {
            return '\0';
        }

        var index = (y * _width) + x;
        if (_continuations[index] || _cells[index] is not string element || element.Length == 0)
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
            if (CanvasAnsiScanner.TryReadEscape(text, index, out var ansiSequence, out var consumed))
            {
                sawAnsi = true;
                if (lastColumn >= 0)
                {
                    var previousIndex = (y * _width) + lastColumn;
                    if (_cells[previousIndex] is string previous)
                    {
                        _cells[previousIndex] = previous + ansiSequence;
                    }
                }
                else
                {
                    pendingZeroWidth += ansiSequence;
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
                    var previousIndex = (y * _width) + lastColumn;
                    if (_cells[previousIndex] is string previous)
                    {
                        _cells[previousIndex] = previous + element;
                    }
                }
                else
                {
                    pendingZeroWidth += element;
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
                    var value = pendingZeroWidth.Length == 0 ? element : pendingZeroWidth + element;
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
                var previousIndex = (y * _width) + lastColumn;
                if (_cells[previousIndex] is string previous && !previous.EndsWith("\u001b[0m", StringComparison.Ordinal))
                {
                    _cells[previousIndex] = previous + "\u001b[0m";
                }
            }
            else
            {
                pendingZeroWidth += "\u001b[0m";
            }
        }

        if (pendingZeroWidth.Length > 0 && lastColumn >= 0)
        {
            var previousIndex = (y * _width) + lastColumn;
            if (_cells[previousIndex] is string previous)
            {
                _cells[previousIndex] = previous + pendingZeroWidth;
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

        var index = (y * _width) + x;
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

        var nextIndex = (y * _width) + nextColumn;
        ClearCell(nextIndex);
        _cells[nextIndex] = null;
        _continuations[nextIndex] = true;
    }

    private void ClearCell(int index)
    {
        if (_continuations[index])
        {
            var previous = index - 1;
            if (previous >= 0 && (previous / _width) == (index / _width))
            {
                _cells[previous] = null;
                _continuations[previous] = false;
            }

            _continuations[index] = false;
            _cells[index] = null;
            return;
        }

        if (_cells[index] is string existing && TextElementWidth.Measure(existing) > 1)
        {
            var next = index + 1;
            if (next < _cells.Length && (next / _width) == (index / _width) && _continuations[next])
            {
                _continuations[next] = false;
                _cells[next] = null;
            }
        }

        _cells[index] = null;
        _continuations[index] = false;
    }
}
