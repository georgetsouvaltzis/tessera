using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a scrolling log viewer.
/// </summary>
public sealed class LogView : Control
{
    private readonly ViewportModel _viewport = new();
    private readonly List<string> _entries = [];
    private string _filter = string.Empty;

    public LogView()
    {
        _viewport.SetWrap(false);
    }

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Logs";

    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle EntryStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle PausedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    public Thickness Padding
    {
        get;
        set;
    }

    public bool AutoScroll
    {
        get;
        set;
    } = true;

    public bool IsPaused
    {
        get;
        private set;
    }

    public int Count => _entries.Count;

    public override bool IsFocused
    {
        get;
        set;
    }

    public void Append(string line)
    {
        if (IsPaused)
        {
            return;
        }

        _entries.Add(line ?? string.Empty);
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
        _filter = filter ?? string.Empty;
        RefreshViewport();
    }

    public override bool Handle(Message message)
    {
        if (message is KeyPressed key)
        {
            if (key.IsCharacter('p'))
            {
                IsPaused = !IsPaused;
                return true;
            }

            if (key.IsCharacter('c'))
            {
                Clear();
                return true;
            }
        }

        return _viewport.Update(message, ViewportKeyMap.Default);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = FormatTitle();
        if (IsPaused)
        {
            title += " [paused]";
        }

        if (!string.IsNullOrEmpty(title))
        {
            var titleStyle = IsFocused ? FocusedTitleStyle : TitleStyle;
            if (IsPaused && !PausedTitleStyle.IsEmpty)
            {
                titleStyle = titleStyle.IsEmpty ? PausedTitleStyle : titleStyle.Merge(PausedTitleStyle);
            }

            if (!titleStyle.IsEmpty)
            {
                title = titleStyle.Render(title);
            }
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
            var line = lines[row];
            if (!EntryStyle.IsEmpty)
            {
                line = EntryStyle.Render(line);
            }

            canvas.WriteText(content.X, content.Y + row, line, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(18, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4) + Padding.Horizontal;
        var height = Math.Max(4, Padding.Vertical + 4);
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RefreshViewport()
    {
        _viewport.SetLines(FilterEntries(_entries, _filter));
    }

    private string FormatTitle()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string FormatTitleForMeasure()
    {
        if (ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private static IEnumerable<string> FilterEntries(IReadOnlyList<string> entries, string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return entries;
        }

        return entries.Where(line => line.Contains(filter, StringComparison.OrdinalIgnoreCase));
    }
}
