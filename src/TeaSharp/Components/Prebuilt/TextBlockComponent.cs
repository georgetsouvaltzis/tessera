using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class TextBlockComponent : ICanvasComponent
{
    public TextBlockComponent()
    {
    }

    public TextBlockComponent(TextBlockOptions options)
    {
        Text = options.Text;
        Title = options.Title;
        Border = options.Border;
        Padding = options.Padding;
    }

    public string Text { get; set; } = string.Empty;

    public string? Title { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : Title ?? "Text",
            Border,
            Padding);
        if (content.IsEmpty)
        {
            return;
        }

        DrawLines(canvas, content);
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
