using TeaSharp.Components.Primitives;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a two-sided status strip.
/// </summary>
public sealed class StatusBar : Control
{
    public string LeftText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string RightText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public char Fill
    {
        get;
        set;
    } = ' ';

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var row = new string(Fill, Math.Max(0, clipped.Width)).ToCharArray();
        CopyToRow(row, 0, LeftText);
        var rightStart = Math.Max(0, clipped.Width - RightText.Length);
        CopyToRow(row, rightStart, RightText);
        canvas.WriteText(clipped.X, clipped.Y, new string(row), clipped.Width);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return new LayoutMeasurement(
            Math.Clamp(Math.Max(LeftText.Length + RightText.Length, 1), 0, availableBounds.Width),
            Math.Clamp(1, 0, availableBounds.Height));
    }

    private static void CopyToRow(char[] row, int start, string text)
    {
        if (row.Length == 0 || string.IsNullOrEmpty(text) || start >= row.Length)
        {
            return;
        }

        var index = Math.Max(0, start);
        for (var i = 0; i < text.Length && index < row.Length; i++, index++)
        {
            row[index] = text[i];
        }
    }
}
