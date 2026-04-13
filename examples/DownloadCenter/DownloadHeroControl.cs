using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.DownloadCenter;

internal sealed class DownloadHeroControl : Control
{
    public string Title { get; set; } = "Download Center";
    public string ClockText { get; set; } = string.Empty;
    public string SummaryText { get; set; } = string.Empty;
    public string PressureText { get; set; } = string.Empty;
    public string ThroughputText { get; set; } = string.Empty;
    public string CommandText { get; set; } = string.Empty;
    public BorderStyle Border { get; set; } = BorderStyle.Rounded;
    public Thickness Padding { get; set; } = Thickness.All(1);
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle ClockStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle BadgeStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle MetaStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle CommandStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, null, Border, BorderStyleText);
        var content = clipped.Inset(1, 1).Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        WriteLine(canvas, content, 0,
            $"{ApplyStyle(Title.ToUpperInvariant(), TitleStyle)}  {ApplyStyle(ClockText, ClockStyle)}");
        WriteLine(canvas, content, 1,
            $"{ApplyStyle($"[{SummaryText}]", BadgeStyle)} {ApplyStyle($"[{ThroughputText}]", BadgeStyle)} {ApplyStyle($"[{PressureText}]", BadgeStyle)}");
        WriteLine(canvas, content, 2,
            ApplyStyle("transfer lanes, retry choreography, and orbit cache pressure aligned", MetaStyle));
        WriteLine(canvas, content, 3, ApplyStyle(CommandText, CommandStyle));
    }

    private static void WriteLine(Canvas canvas, Rect content, int row, string text)
    {
        if (row >= content.Height)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y + row, text, content.Width);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty ? text : style.Render(text);
    }
}
