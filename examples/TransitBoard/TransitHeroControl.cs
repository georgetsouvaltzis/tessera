using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Examples.TransitBoard;

internal sealed class TransitHeroControl : Control
{
    public string Title { get; set; } = "Central Exchange";
    public string ClockText { get; set; } = string.Empty;
    public string SummaryText { get; set; } = string.Empty;
    public string AdvisoryText { get; set; } = string.Empty;
    public string NoticeText { get; set; } = string.Empty;
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ClockStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SummaryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle AdvisoryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle NoticeStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DividerStyle { get; set; } = TeaStyle.Empty;

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

    private static string Render(TeaStyle style, string text) => style.IsEmpty ? text : style.Render(text);
}
