using System.Text;
using System.Globalization;

namespace TeaSharp.Components;

public enum CanvasTextMode
{
    Fast = 0,
    GraphemeAware = 1,
}

public sealed class Canvas
{
    private readonly char[]? _cells;
    private readonly string?[]? _graphemeCells;
    private readonly bool[]? _graphemeContinuation;

    public Canvas(int width, int height, CanvasTextMode textMode = CanvasTextMode.Fast)
    {
        Width = Math.Max(1, width);
        Height = Math.Max(1, height);
        TextMode = textMode;
        if (TextMode == CanvasTextMode.Fast)
        {
            _cells = new char[Width * Height];
        }
        else
        {
            _graphemeCells = new string?[Width * Height];
            _graphemeContinuation = new bool[Width * Height];
        }

        Clear();
    }

    public int Width { get; }

    public int Height { get; }

    public CanvasTextMode TextMode { get; }

    public Rect Bounds => new(0, 0, Width, Height);

    public void Clear(char fill = ' ')
    {
        if (TextMode == CanvasTextMode.Fast)
        {
            Array.Fill(_cells!, fill);
            return;
        }

        Array.Clear(_graphemeCells!, 0, _graphemeCells!.Length);
        Array.Clear(_graphemeContinuation!, 0, _graphemeContinuation!.Length);
        if (fill != ' ')
        {
            var text = fill.ToString();
            for (var i = 0; i < _graphemeCells.Length; i++)
            {
                _graphemeCells[i] = text;
            }
        }
    }

    public void Set(int x, int y, char value)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return;
        }

        if (TextMode == CanvasTextMode.Fast)
        {
            _cells![(y * Width) + x] = value;
            return;
        }

        SetGraphemeCell(x, y, value.ToString(), width: 1);
    }

    public char Get(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return '\0';
        }

        var index = (y * Width) + x;
        if (TextMode == CanvasTextMode.Fast)
        {
            return _cells![index];
        }

        if (_graphemeContinuation![index] || _graphemeCells![index] is not string element || element.Length == 0)
        {
            return '\0';
        }

        return element[0];
    }

    public void WriteText(int x, int y, string text, int maxWidth = int.MaxValue)
    {
        if (string.IsNullOrEmpty(text) || y < 0 || y >= Height || maxWidth <= 0)
        {
            return;
        }

        if (TextMode == CanvasTextMode.Fast)
        {
            WriteTextFast(x, y, text, maxWidth);
            return;
        }

        WriteTextGrapheme(x, y, text, maxWidth);
    }

    private void WriteTextFast(int x, int y, string text, int maxWidth)
    {
        var cx = x;
        var written = 0;
        foreach (var ch in text)
        {
            if (ch is '\r' or '\n')
            {
                break;
            }

            if (written >= maxWidth)
            {
                break;
            }

            Set(cx, y, ch);
            cx++;
            written++;
        }
    }

    private void WriteTextGrapheme(int x, int y, string text, int maxWidth)
    {
        var cx = x;
        var written = 0;
        var index = 0;
        var lastColumn = -1;

        while (index < text.Length)
        {
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
                    var previousIndex = (y * Width) + lastColumn;
                    if (_graphemeCells![previousIndex] is string previous)
                    {
                        _graphemeCells[previousIndex] = previous + element;
                    }
                }

                continue;
            }

            if (written + elementWidth > maxWidth)
            {
                break;
            }

            if (cx >= 0 && cx < Width)
            {
                if (elementWidth == 1 || cx + 1 < Width)
                {
                    SetGraphemeCell(cx, y, element, elementWidth);
                    lastColumn = cx;
                }
                else
                {
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
    }

    public void DrawHorizontalLine(int x, int y, int width, char value = '─')
    {
        if (width <= 0 || y < 0 || y >= Height)
        {
            return;
        }

        var start = Math.Max(0, x);
        var end = Math.Min(Width, x + width);
        for (var cx = start; cx < end; cx++)
        {
            Set(cx, y, value);
        }
    }

    public void DrawVerticalLine(int x, int y, int height, char value = '│')
    {
        if (height <= 0 || x < 0 || x >= Width)
        {
            return;
        }

        var start = Math.Max(0, y);
        var end = Math.Min(Height, y + height);
        for (var cy = start; cy < end; cy++)
        {
            Set(x, cy, value);
        }
    }

    public void DrawBox(Rect rect, string? title = null)
    {
        var clipped = Rect.Intersect(rect, Bounds);
        if (clipped.IsEmpty || clipped.Width < 2 || clipped.Height < 2)
        {
            return;
        }

        DrawHorizontalLine(clipped.X + 1, clipped.Y, clipped.Width - 2);
        DrawHorizontalLine(clipped.X + 1, clipped.Bottom - 1, clipped.Width - 2);
        DrawVerticalLine(clipped.X, clipped.Y + 1, clipped.Height - 2);
        DrawVerticalLine(clipped.Right - 1, clipped.Y + 1, clipped.Height - 2);

        Set(clipped.X, clipped.Y, '┌');
        Set(clipped.Right - 1, clipped.Y, '┐');
        Set(clipped.X, clipped.Bottom - 1, '└');
        Set(clipped.Right - 1, clipped.Bottom - 1, '┘');

        if (!string.IsNullOrWhiteSpace(title))
        {
            var safeTitle = $" {title.Trim()} ";
            WriteText(clipped.X + 2, clipped.Y, safeTitle, clipped.Width - 4);
        }
    }

    public string Render()
    {
        if (TextMode == CanvasTextMode.Fast)
        {
            return RenderFast();
        }

        return RenderGrapheme();
    }

    private string RenderFast()
    {
        var sb = new StringBuilder((Width + 1) * Height);
        for (var y = 0; y < Height; y++)
        {
            if (y > 0)
            {
                sb.Append('\n');
            }

            sb.Append(_cells!, y * Width, Width);
        }

        return sb.ToString();
    }

    private string RenderGrapheme()
    {
        var sb = new StringBuilder((Width + 1) * Height);
        for (var y = 0; y < Height; y++)
        {
            if (y > 0)
            {
                sb.Append('\n');
            }

            var rowStart = y * Width;
            for (var x = 0; x < Width; x++)
            {
                var index = rowStart + x;
                if (_graphemeContinuation![index])
                {
                    continue;
                }

                var cell = _graphemeCells![index];
                if (cell is null)
                {
                    sb.Append(' ');
                }
                else
                {
                    sb.Append(cell);
                }
            }
        }

        return sb.ToString();
    }

    private void SetGraphemeCell(int x, int y, string value, int width)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height || width <= 0)
        {
            return;
        }

        var index = (y * Width) + x;
        ClearGraphemeCell(index);
        _graphemeCells![index] = value;
        _graphemeContinuation![index] = false;

        if (width > 1)
        {
            var nextColumn = x + 1;
            if (nextColumn >= Width)
            {
                return;
            }

            var nextIndex = (y * Width) + nextColumn;
            ClearGraphemeCell(nextIndex);
            _graphemeCells[nextIndex] = null;
            _graphemeContinuation[nextIndex] = true;
        }
    }

    private void ClearGraphemeCell(int index)
    {
        if (_graphemeCells is null || _graphemeContinuation is null)
        {
            return;
        }

        if (_graphemeContinuation[index])
        {
            var previous = index - 1;
            if (previous >= 0 && (previous / Width) == (index / Width))
            {
                _graphemeCells[previous] = null;
                _graphemeContinuation[previous] = false;
            }

            _graphemeContinuation[index] = false;
            _graphemeCells[index] = null;
            return;
        }

        if (_graphemeCells[index] is string existing && TextElementWidth.Measure(existing) > 1)
        {
            var next = index + 1;
            if (next < _graphemeCells.Length && (next / Width) == (index / Width) && _graphemeContinuation[next])
            {
                _graphemeContinuation[next] = false;
                _graphemeCells[next] = null;
            }
        }

        _graphemeCells[index] = null;
        _graphemeContinuation[index] = false;
    }
}
