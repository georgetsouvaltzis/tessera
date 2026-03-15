using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a dismissible overlay panel.
/// </summary>
public sealed class Modal : Control
{
    private List<string> _bodyLines = ["(empty)"];

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Modal";

    public bool IsVisible
    {
        get;
        set;
    }

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.Rounded;

    public Thickness Padding
    {
        get;
        set;
    }

    public IReadOnlyList<string> BodyLines
    {
        get => _bodyLines;
        set => _bodyLines = [.. (value ?? ["(empty)"])];
    }

    public char BackdropFill
    {
        get;
        set;
    } = '·';

    public override void Render(Canvas canvas, Rect rect)
    {
        if (!IsVisible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        for (var y = clipped.Y; y < clipped.Bottom; y++)
        {
            for (var x = clipped.X; x < clipped.Right; x++)
            {
                canvas.Set(x, y, BackdropFill);
            }
        }

        if (clipped.Width < 4 || clipped.Height < 4)
        {
            return;
        }

        var modalWidth = Math.Clamp(clipped.Width * 3 / 5, 4, Math.Max(4, clipped.Width - 2));
        var modalHeight = Math.Clamp(clipped.Height / 2, 4, Math.Max(4, clipped.Height - 2));
        var modalX = clipped.X + (clipped.Width - modalWidth) / 2;
        var modalY = clipped.Y + (clipped.Height - modalHeight) / 2;
        var modal = new Rect(modalX, modalY, modalWidth, modalHeight);

        FillRect(canvas, modal, ' ');
        var body = FrameLayout.DrawFrameAndResolveContent(canvas, modal, Title, Border, Padding);
        if (body.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(body.Height, _bodyLines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(body.X, body.Y + row, _bodyLines[row], body.Width);
        }
    }

    public void SetBodyLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        _bodyLines = [.. lines];
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var longest = _bodyLines.Count == 0 ? 8 : _bodyLines.Max(ControlTextLayout.MeasureDisplayWidth);
        var width = Math.Max(Title.Length + 4, longest + Padding.Horizontal) + 2;
        var height = Math.Max(4, _bodyLines.Count + Padding.Vertical + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private static void FillRect(Canvas canvas, Rect rect, char fill)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        for (var y = clipped.Y; y < clipped.Bottom; y++)
        {
            for (var x = clipped.X; x < clipped.Right; x++)
            {
                canvas.Set(x, y, fill);
            }
        }
    }
}
