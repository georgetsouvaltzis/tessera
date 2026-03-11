using System.Text;

namespace TeaSharp.Components.Primitives;

public sealed class Canvas
{
    private readonly char[]? _cells;
    private readonly CanvasGraphemeBuffer? _graphemeBuffer;

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
            _graphemeBuffer = new CanvasGraphemeBuffer(Width, Height);
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

        _graphemeBuffer!.Clear(fill);
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

        _graphemeBuffer!.Set(x, y, value);
    }

    public char Get(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return '\0';
        }

        if (TextMode == CanvasTextMode.Fast)
        {
            return _cells![(y * Width) + x];
        }

        return _graphemeBuffer!.Get(x, y);
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

        _graphemeBuffer!.WriteText(x, y, text, maxWidth);
    }

    private void WriteTextFast(int x, int y, string text, int maxWidth)
    {
        var cx = x;
        var written = 0;
        foreach (var ch in text)
        {
            if (ch is '\r' or '\n' || written >= maxWidth)
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

    public void DrawBox(Rect rect, string? title = null, BorderStyle borderStyle = BorderStyle.SingleLine)
    {
        var clipped = Rect.Intersect(rect, Bounds);
        if (clipped.IsEmpty || clipped.Width < 2 || clipped.Height < 2)
        {
            return;
        }

        var (horizontal, vertical, topLeft, topRight, bottomLeft, bottomRight) = borderStyle switch
        {
            BorderStyle.Rounded => ('─', '│', '╭', '╮', '╰', '╯'),
            BorderStyle.Heavy => ('━', '┃', '┏', '┓', '┗', '┛'),
            BorderStyle.Ascii => ('-', '|', '+', '+', '+', '+'),
            _ => ('─', '│', '┌', '┐', '└', '┘'),
        };

        DrawHorizontalLine(clipped.X + 1, clipped.Y, clipped.Width - 2, horizontal);
        DrawHorizontalLine(clipped.X + 1, clipped.Bottom - 1, clipped.Width - 2, horizontal);
        DrawVerticalLine(clipped.X, clipped.Y + 1, clipped.Height - 2, vertical);
        DrawVerticalLine(clipped.Right - 1, clipped.Y + 1, clipped.Height - 2, vertical);

        Set(clipped.X, clipped.Y, topLeft);
        Set(clipped.Right - 1, clipped.Y, topRight);
        Set(clipped.X, clipped.Bottom - 1, bottomLeft);
        Set(clipped.Right - 1, clipped.Bottom - 1, bottomRight);

        if (!string.IsNullOrWhiteSpace(title))
        {
            WriteText(clipped.X + 2, clipped.Y, $" {title.Trim()} ", clipped.Width - 4);
        }
    }

    public string Render()
    {
        return TextMode == CanvasTextMode.Fast
            ? RenderFast()
            : _graphemeBuffer!.Render();
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
}
