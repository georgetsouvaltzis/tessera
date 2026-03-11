using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class LabelComponent : ICanvasComponent
{
    public LabelComponent()
    {
    }

    public LabelComponent(LabelOptions options)
    {
        Text = options.Text;
        Title = options.Title;
        ShowBorder = options.ShowBorder;
    }

    public string Text { get; set; } = string.Empty;

    public string? Title { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool DrawBorder
    {
        get => ShowBorder;
        set => ShowBorder = value;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Title ?? "Label");
            var content = clipped.Inset(1, 1);
            if (content.IsEmpty)
            {
                return;
            }

            DrawLines(canvas, content);
            return;
        }

        DrawLines(canvas, clipped);
    }

    private void DrawLines(Canvas canvas, Rect rect)
    {
        var lines = Text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

        var rows = Math.Min(rect.Height, lines.Length);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(rect.X, rect.Y + row, lines[row], rect.Width);
        }
    }
}
