using System.Text;

namespace TeaSharp.Components;

public sealed class Canvas
{
    private readonly char[] _cells;

    public Canvas(int width, int height)
    {
        Width = Math.Max(1, width);
        Height = Math.Max(1, height);
        _cells = new char[Width * Height];
        Clear();
    }

    public int Width { get; }

    public int Height { get; }

    public Rect Bounds => new(0, 0, Width, Height);

    public void Clear(char fill = ' ')
    {
        Array.Fill(_cells, fill);
    }

    public void Set(int x, int y, char value)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return;
        }

        _cells[(y * Width) + x] = value;
    }

    public char Get(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return '\0';
        }

        return _cells[(y * Width) + x];
    }

    public void WriteText(int x, int y, string text, int maxWidth = int.MaxValue)
    {
        if (string.IsNullOrEmpty(text) || y < 0 || y >= Height || maxWidth <= 0)
        {
            return;
        }

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
        var sb = new StringBuilder((Width + 1) * Height);
        for (var y = 0; y < Height; y++)
        {
            if (y > 0)
            {
                sb.Append('\n');
            }

            sb.Append(_cells, y * Width, Width);
        }

        return sb.ToString();
    }
}
