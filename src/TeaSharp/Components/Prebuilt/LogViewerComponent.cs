using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Provides a scrollable log surface with optional filtering, pause, and clear behavior.
/// </summary>
public sealed class LogViewerComponent : IStatefulComponent, IFocusableComponent
{
    private readonly ViewportModel _viewport = new();
    private readonly List<string> _entries = [];

    public LogViewerComponent()
    {
        _viewport.SetWrap(false);
    }

    public LogViewerComponent(LogViewerOptions options)
        : this()
    {
        Title = options.Title;
        Focused = options.Focused;
        Border = options.Border;
        Padding = options.Padding;
        AutoScroll = options.AutoScroll;
        ViewportKeyMap = options.ViewportKeyMap ?? ViewportKeyMap.Default;
        TogglePauseKey = options.TogglePauseKey ?? TogglePauseKey;
        ClearKey = options.ClearKey ?? ClearKey;
        if (options.InitialEntries is not null)
        {
            foreach (var entry in options.InitialEntries)
            {
                _entries.Add(entry);
            }
        }

        if (!string.IsNullOrEmpty(options.InitialFilter))
        {
            Filter = options.InitialFilter;
        }

        RefreshViewport();
        if (AutoScroll)
        {
            _viewport.ScrollToBottom();
        }
    }

    public string Title { get; set; } = "Logs";

    public bool Focused { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public bool AutoScroll { get; set; } = true;

    public bool Paused { get; private set; }

    public string Filter { get; private set; } = string.Empty;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
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

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : title,
            Border,
            Padding);
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
    private void RefreshViewport()
    {
        _viewport.SetLines(LogViewerContent.Filter(_entries, Filter));
    }
}
