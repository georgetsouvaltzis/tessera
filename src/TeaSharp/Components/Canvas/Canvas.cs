using TeaSharp.Components.Primitives.Internal;
using System.Text;

namespace TeaSharp.Components.Primitives;

/// <summary>
/// Represents a mutable terminal drawing surface.
/// </summary>
/// <remarks>
/// This is the low-level drawing primitive behind custom rendering and advanced component interop. Prefer root
/// controls and layouts for normal application authoring.
/// </remarks>
public sealed class Canvas
{
    private readonly char[]? _cells;
    private readonly CanvasGraphemeBuffer? _graphemeBuffer;

    /// <summary>
    /// Initializes a new canvas.
    /// </summary>
    /// <param name="width">The canvas width in cells.</param>
    /// <param name="height">The canvas height in cells.</param>
    /// <param name="textMode">The text rendering mode.</param>
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

    /// <summary>
    /// Gets the canvas width in cells.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the canvas height in cells.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the text rendering mode used by the canvas.
    /// </summary>
    public CanvasTextMode TextMode { get; }

    /// <summary>
    /// Gets the full canvas bounds.
    /// </summary>
    public Rect Bounds => new(0, 0, Width, Height);

    /// <summary>
    /// Clears the canvas with the supplied fill character.
    /// </summary>
    /// <param name="fill">The fill character.</param>
    public void Clear(char fill = ' ')
    {
        if (TextMode == CanvasTextMode.Fast)
        {
            Array.Fill(_cells!, fill);
            return;
        }

        _graphemeBuffer!.Clear(fill);
    }

    /// <summary>
    /// Writes a single cell.
    /// </summary>
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

    /// <summary>
    /// Reads a single cell.
    /// </summary>
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

    /// <summary>
    /// Writes text starting at the supplied position.
    /// </summary>
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

    internal void WriteTextPadded(int x, int y, string? text, int width)
    {
        if (y < 0 || y >= Height || width <= 0)
        {
            return;
        }

        if (TextMode == CanvasTextMode.Fast)
        {
            WriteTextPaddedFast(x, y, text, width);
            return;
        }

        WriteTextPaddedPortable(x, y, text, width);
    }

    private void WriteTextPaddedFast(int x, int y, string? text, int width)
    {
        var cx = x;
        var written = 0;
        var cells = _cells!;
        var rowStart = y * Width;
        if (!string.IsNullOrEmpty(text))
        {
            foreach (var raw in text)
            {
                if (written >= width)
                {
                    break;
                }

                if (raw == '\r')
                {
                    continue;
                }

                var value = raw == '\n' ? ' ' : raw;
                if ((uint)cx < (uint)Width)
                {
                    cells[rowStart + cx] = value;
                }

                cx++;
                written++;
            }
        }

        for (; written < width; written++, cx++)
        {
            if ((uint)cx < (uint)Width)
            {
                cells[rowStart + cx] = ' ';
            }
        }
    }

    private void WriteTextPaddedPortable(int x, int y, string? text, int width)
    {
        var cx = x;
        var written = 0;
        if (!string.IsNullOrEmpty(text))
        {
            foreach (var raw in text)
            {
                if (written >= width)
                {
                    break;
                }

                if (raw == '\r')
                {
                    continue;
                }

                Set(cx, y, raw == '\n' ? ' ' : raw);
                cx++;
                written++;
            }
        }

        for (; written < width; written++, cx++)
        {
            Set(cx, y, ' ');
        }
    }

    private void WriteTextFast(int x, int y, string text, int maxWidth)
    {
        var lineEnd = text.AsSpan().IndexOfAny('\r', '\n');
        var sourceLength = lineEnd < 0 ? text.Length : lineEnd;
        if (sourceLength <= 0)
        {
            return;
        }

        var maxSourceLength = Math.Min(sourceLength, maxWidth);
        if (maxSourceLength <= 0 || x >= Width || x + maxSourceLength <= 0)
        {
            return;
        }

        var sourceStart = 0;
        var targetX = x;
        var copyLength = maxSourceLength;
        if (targetX < 0)
        {
            sourceStart = -targetX;
            targetX = 0;
            copyLength -= sourceStart;
            if (copyLength <= 0)
            {
                return;
            }
        }

        copyLength = Math.Min(copyLength, Width - targetX);
        if (copyLength <= 0)
        {
            return;
        }

        var rowStart = y * Width;
        text.AsSpan(sourceStart, copyLength).CopyTo(_cells!.AsSpan(rowStart + targetX, copyLength));
    }

    /// <summary>
    /// Draws a horizontal line.
    /// </summary>
    public void DrawHorizontalLine(int x, int y, int width, char value = '─')
    {
        FillRow(y, x, width, value);
    }

    /// <summary>
    /// Draws a vertical line.
    /// </summary>
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

    internal void FillRect(Rect rect, char fill)
    {
        var clipped = Rect.Intersect(rect, Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        for (var y = clipped.Y; y < clipped.Bottom; y++)
        {
            FillRow(y, clipped.X, clipped.Width, fill);
        }
    }

    internal void FillRow(int y, int x, int width, char fill)
    {
        if (width <= 0 || y < 0 || y >= Height)
        {
            return;
        }

        var start = Math.Max(0, x);
        var end = Math.Min(Width, x + width);
        if (start >= end)
        {
            return;
        }

        if (TextMode == CanvasTextMode.Fast)
        {
            Array.Fill(_cells!, fill, (y * Width) + start, end - start);
            return;
        }

        for (var cx = start; cx < end; cx++)
        {
            _graphemeBuffer!.Set(cx, y, fill);
        }
    }

    /// <summary>
    /// Draws a border box with an optional title.
    /// </summary>
    public void DrawBox(Rect rect, string? title = null, BorderStyle borderStyle = BorderStyle.SingleLine)
    {
        if (borderStyle == BorderStyle.None)
        {
            return;
        }

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

    /// <summary>
    /// Renders the canvas into a string frame.
    /// </summary>
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
