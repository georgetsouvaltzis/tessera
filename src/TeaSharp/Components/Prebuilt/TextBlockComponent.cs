using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Layout;
using TeaSharp.Styles;
using System.Globalization;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class TextBlockComponent : ICanvasComponent
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
        TextStyle = options.TextStyle;
        HorizontalAlignment = options.HorizontalAlignment;
        VerticalAlignment = options.VerticalAlignment;
    }

    public string Text { get; set; } = string.Empty;

    public string? Title { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public TeaStyle TextStyle { get; set; } = TeaStyle.Empty;

    public HorizontalAlignment HorizontalAlignment { get; set; }

    public VerticalAlignment VerticalAlignment { get; set; }

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
        var startY = rect.Y + ResolveVerticalOffset(rect.Height, lines.Length);
        for (var row = 0; row < rows; row++)
        {
            var line = lines[row];
            var rendered = TextStyle.IsEmpty ? line : TextStyle.Render(line);
            var x = rect.X + ResolveHorizontalOffset(rect.Width, line);
            canvas.WriteText(x, startY + row, rendered, Math.Max(0, rect.Right - x));
        }
    }

    private int ResolveHorizontalOffset(int availableWidth, string line)
    {
        return HorizontalAlignment switch
        {
            HorizontalAlignment.Center => Math.Max(0, (availableWidth - MeasureDisplayWidth(line)) / 2),
            HorizontalAlignment.Right => Math.Max(0, availableWidth - MeasureDisplayWidth(line)),
            _ => 0,
        };
    }

    private int ResolveVerticalOffset(int availableHeight, int lineCount)
    {
        return VerticalAlignment switch
        {
            VerticalAlignment.Center => Math.Max(0, (availableHeight - Math.Min(availableHeight, lineCount)) / 2),
            VerticalAlignment.Bottom => Math.Max(0, availableHeight - Math.Min(availableHeight, lineCount)),
            _ => 0,
        };
    }

    private static int MeasureDisplayWidth(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        var width = 0;
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();
            if (CanvasAnsiScanner.TryReadEscape(element, 0, out _, out _))
            {
                continue;
            }

            width += TextElementWidth.Measure(element);
        }

        return width;
    }
}
