using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.TransitBoard;

internal sealed class TransitHeroControl : Control
{
    public string Title { get; set; } = "Central Exchange";
    public string ClockText { get; set; } = string.Empty;
    public string SummaryText { get; set; } = string.Empty;
    public string AdvisoryText { get; set; } = string.Empty;
    public string NoticeText { get; set; } = string.Empty;
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle ClockStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle SummaryStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle AdvisoryStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle NoticeStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle DividerStyle { get; set; } = TesseraStyle.Empty;

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        WriteLine(canvas, clipped, 0, $"{Render(TitleStyle, Title.ToUpperInvariant())}  {Render(ClockStyle, ClockText)}");
        WriteLine(canvas, clipped, 1, Render(SummaryStyle, SummaryText));
        WriteLine(canvas, clipped, 2, Render(AdvisoryStyle, AdvisoryText));
        WriteLine(canvas, clipped, 3, Render(NoticeStyle, NoticeText));

        if (clipped.Height > 4)
        {
            canvas.WriteText(clipped.X, clipped.Bottom - 1, Render(DividerStyle, new string('─', clipped.Width)), clipped.Width);
        }
    }

    private static void WriteLine(Canvas canvas, Rect rect, int row, string text)
    {
        if (row >= rect.Height)
        {
            return;
        }

        canvas.WriteText(rect.X, rect.Y + row, text, rect.Width);
    }

    private static string Render(TesseraStyle style, string text) => style.IsEmpty ? text : style.Render(text);
}
