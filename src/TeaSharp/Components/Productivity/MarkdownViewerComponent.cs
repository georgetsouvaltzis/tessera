using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class MarkdownViewerComponent : IStatefulComponent, IFocusableComponent
{
    private readonly ViewportModel _viewport = new();
    private string _markdown = string.Empty;

    public string Title { get; set; } = "Markdown";

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool Wrap
    {
        get => _viewport.Wrap;
        set => _viewport.SetWrap(value);
    }

    public bool ShowLineNumbers
    {
        get => _viewport.ShowLineNumbers;
        set => _viewport.ShowLineNumbers = value;
    }

    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public void SetMarkdown(string markdown)
    {
        _markdown = markdown ?? string.Empty;
        _viewport.SetContent(RenderMarkdown(_markdown));
    }

    public bool Update(IMessage message)
    {
        return _viewport.Update(message, ViewportKeyMap);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
            content = clipped.Inset(1, 1);
        }
        else
        {
            content = clipped;
        }

        if (content.IsEmpty)
        {
            return;
        }

        _viewport.Resize(content.Width, content.Height);
        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, lines[row], content.Width);
        }
    }

    private static string RenderMarkdown(string markdown)
    {
        var lines = markdown
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

        var output = new List<string>(lines.Length);
        var inCode = false;
        foreach (var raw in lines)
        {
            var line = raw ?? string.Empty;
            var trimmed = line.TrimStart();

            if (trimmed.StartsWith("```", StringComparison.Ordinal))
            {
                inCode = !inCode;
                output.Add(inCode ? "┌ code" : "└");
                continue;
            }

            if (inCode)
            {
                output.Add($"  {line}");
                continue;
            }

            if (trimmed.StartsWith("### ", StringComparison.Ordinal))
            {
                output.Add($"### {trimmed[4..]}");
                continue;
            }

            if (trimmed.StartsWith("## ", StringComparison.Ordinal))
            {
                output.Add($"## {trimmed[3..]}");
                continue;
            }

            if (trimmed.StartsWith("# ", StringComparison.Ordinal))
            {
                output.Add($"# {trimmed[2..].ToUpperInvariant()}");
                continue;
            }

            if (trimmed.StartsWith("- ", StringComparison.Ordinal) || trimmed.StartsWith("* ", StringComparison.Ordinal))
            {
                output.Add($"• {trimmed[2..]}");
                continue;
            }

            output.Add(line);
        }

        return string.Join('\n', output);
    }
}
