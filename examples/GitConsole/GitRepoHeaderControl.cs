using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Examples.GitConsole;

internal sealed class GitRepoHeaderControl : Control
{
    public string Title { get; set; } = "Repository";
    public string RepositoryName { get; set; } = "teasharp";
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
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    public TeaStyle NameStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle BranchStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle PathStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle PulseStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ActionStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DetailStyle { get; set; } = TeaStyle.Empty;

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

        WriteLine(canvas, content, 0, $"{Render(NameStyle, RepositoryName)}  {Render(BranchStyle, BranchName)}  {Render(PulseStyle, PulseText)}");
        WriteLine(canvas, content, 1, $"{Render(PathStyle, RepositoryPath)}  {Render(DetailStyle, $"{RemoteName} ⇅ +{Ahead}/-{Behind}")}");
        WriteLine(canvas, content, 2, Render(ActionStyle, LastAction));
        if (content.Height > 3)
        {
            WriteLine(canvas, content, 3, Render(DetailStyle, LastActionDetail));
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
