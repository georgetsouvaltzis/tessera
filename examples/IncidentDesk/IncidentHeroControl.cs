using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Examples.IncidentDesk;

internal sealed class IncidentHeroControl : Control
{
    public string Title { get; set; } = "Incident Desk";
    public string IncidentId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string Sla { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Phase { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public BorderStyle Border { get; set; } = BorderStyle.Rounded;
    public Thickness Padding { get; set; } = Thickness.All(1);
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    public TeaStyle SummaryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle MetaStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DetailStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle HighlightStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SeverityStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle StatusStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle PhaseStyle { get; set; } = TeaStyle.Empty;

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = TitleStyle.IsEmpty ? Title : TitleStyle.Render(Title);
        canvas.DrawBox(clipped, title, Border, BorderStyleText);

        var content = clipped.Inset(1, 1).Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        var summaryLine = $"{Render(SeverityStyle, $" {Severity} ")}  {Render(StatusStyle, $" {Status} ")}  {Render(HighlightStyle, IncidentId)}  {Render(SummaryStyle, Summary)}";
        WriteLine(canvas, content, 0, summaryLine);

        if (content.Height > 1)
        {
            var opsLine = $"{Render(MetaStyle, $"{Service} / {Environment} / {Region}")}  {Render(HighlightStyle, $"owner {Owner}")}  {Render(HighlightStyle, Sla)}";
            WriteLine(canvas, content, 1, opsLine);
        }

        if (content.Height > 2)
        {
            WriteLine(canvas, content, 2, $"{Render(DetailStyle, "Bridge")}  {Render(MetaStyle, Channel)}  {Render(DetailStyle, "Phase")}  {Render(PhaseStyle, Phase)}");
        }

        if (content.Height > 3)
        {
            WriteLine(canvas, content, 3, $"{Render(DetailStyle, "Impact")}  {Render(MetaStyle, Impact)}");
        }
    }

    private static void WriteLine(Canvas canvas, Rect content, int row, string text)
    {
        if (row >= content.Height)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y + row, text, content.Width);
    }

    private static string Render(TeaStyle style, string text) => style.IsEmpty ? text : style.Render(text);
}
