using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.GitConsole;

internal sealed class GitRepoHeaderControl : Control
{
    public string Title { get; set; } = "Flight Deck";
    public string RepositoryName { get; set; } = "tessera";
    public string RepositoryPath { get; set; } = string.Empty;
    public string BranchName { get; set; } = "main";
    public string RemoteName { get; set; } = "origin";
    public string PulseText { get; set; } = "steady";
    public string LastAction { get; set; } = string.Empty;
    public string LastActionDetail { get; set; } = string.Empty;
    public int Ahead { get; set; }
    public int Behind { get; set; }
    public BorderStyle Border { get; set; } = BorderStyle.Rounded;
    public Thickness Padding { get; set; } = Thickness.All(1);
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;
    public TesseraStyle NameStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle BranchStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle PathStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle PulseStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle ActionStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle DetailStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle MetaStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle HighlightStyle { get; set; } = TesseraStyle.Empty;

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

        if (content.Height >= 6 && content.Width >= 32)
        {
            var summaryRect = new Rect(content.X, content.Y + 2, content.Width, Math.Min(4, content.Height - 2));
            canvas.DrawBox(summaryRect, null, BorderStyle.Rounded, BorderStyleText);
            var summaryContent = summaryRect.Inset(1, 1);

            WriteLine(
                canvas,
                content,
                0,
                $"{Render(NameStyle, RepositoryName)}  {Render(BranchStyle, BranchName)}  {Render(PulseStyle, PulseText)}");
            WriteLine(
                canvas,
                content,
                1,
                $"{Render(PathStyle, RepositoryPath)}  {Render(MetaStyle, $"{RemoteName}  ahead {Ahead:00}  behind {Behind:00}")}");
            WriteLine(canvas, summaryContent, 0,
                $"{Render(HighlightStyle, "Last move")}  {Render(ActionStyle, LastAction)}");
            if (summaryContent.Height > 1)
            {
                WriteLine(canvas, summaryContent, 1, Render(DetailStyle, LastActionDetail));
            }

            return;
        }

        WriteLine(
            canvas,
            content,
            0,
            $"{Render(NameStyle, RepositoryName)}  {Render(BranchStyle, BranchName)}  {Render(PulseStyle, PulseText)}");
        if (content.Height > 1)
        {
            WriteLine(
                canvas,
                content,
                1,
                $"{Render(ActionStyle, LastAction)}  {Render(MetaStyle, $"{RemoteName} +{Ahead}/-{Behind}")}");
        }

        if (content.Height > 2)
        {
            WriteLine(canvas, content, 2, Render(DetailStyle, LastActionDetail));
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

    private static string Render(TesseraStyle style, string text)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
