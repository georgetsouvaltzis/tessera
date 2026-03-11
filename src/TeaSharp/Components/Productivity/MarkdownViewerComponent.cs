using System.ComponentModel;
using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Renders markdown into a scrollable viewport for help panes, docs, or inline references.
/// </summary>
public sealed class MarkdownViewerComponent : IStatefulComponent, IFocusableComponent
{
    private readonly ViewportModel _viewport = new();
    private string _markdown = string.Empty;

    public MarkdownViewerComponent()
    {
    }

    public MarkdownViewerComponent(MarkdownViewerOptions options)
    {
        Title = options.Title;
        Focused = options.Focused;
        ShowBorder = options.ShowBorder;
        Wrap = options.Wrap;
        ShowLineNumbers = options.ShowLineNumbers;
        ViewportKeyMap = options.ViewportKeyMap ?? ViewportKeyMap.Default;
        if (!string.IsNullOrEmpty(options.InitialMarkdown))
        {
            SetMarkdown(options.InitialMarkdown);
        }
    }

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

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public void SetMarkdown(string markdown)
    {
        _markdown = markdown ?? string.Empty;
        _viewport.SetLines(MarkdownLineRenderer.Render(_markdown));
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
}
