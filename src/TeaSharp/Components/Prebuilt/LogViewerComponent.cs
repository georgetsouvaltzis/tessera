using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class LogViewerComponent : IStatefulComponent, IFocusableComponent
{
    private readonly ViewportModel _viewport = new();
    private readonly List<string> _entries = [];

    public LogViewerComponent()
    {
        _viewport.SetWrap(false);
    }

    public string Title { get; set; } = "Logs";

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool AutoScroll { get; set; } = true;

    public bool Paused { get; private set; }

    public string Filter { get; private set; } = string.Empty;

    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public KeyBinding TogglePauseKey { get; set; } = new("p", "toggle pause", "p");

    public KeyBinding ClearKey { get; set; } = new("c", "clear", "c");

    public int Count => _entries.Count;

    public void Append(string line)
    {
        if (Paused)
        {
            return;
        }

        _entries.Add(line);
        RefreshViewport();
        if (AutoScroll)
        {
            _viewport.ScrollToBottom();
        }
    }

    public void Clear()
    {
        _entries.Clear();
        RefreshViewport();
    }

    public void SetFilter(string filter)
    {
        Filter = filter ?? string.Empty;
        RefreshViewport();
    }

    public bool Update(IMessage message)
    {
        if (message is KeyPressMsg key)
        {
            if (TogglePauseKey.Matches(key))
            {
                Paused = !Paused;
                return true;
            }

            if (ClearKey.Matches(key))
            {
                Clear();
                return true;
            }
        }

        return _viewport.Update(message, ViewportKeyMap);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Focused ? $"{Title} *" : Title;
        if (Paused)
        {
            title += " [paused]";
        }

        var content = ShowBorder
            ? DrawBorderAndResolveContent(canvas, clipped, title)
            : clipped;
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

    private static Rect DrawBorderAndResolveContent(Canvas canvas, Rect clipped, string title)
    {
        canvas.DrawBox(clipped, title);
        return clipped.Inset(1, 1);
    }

    private void RefreshViewport()
    {
        _viewport.SetLines(LogViewerContent.Filter(_entries, Filter));
    }
}
