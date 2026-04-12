using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;
using Tessera.Widgets;

namespace Tessera.Controls;

/// <summary>
/// Represents a scrolling log viewer.
/// </summary>
public sealed class LogView : Control
{
    private readonly ViewportModel _viewport = new();
    private readonly List<string> _entries = [];
    private string _filter = string.Empty;

    /// <summary>
    /// Executes log view.
    /// </summary>
    /// <returns>The result of log view.</returns>
    public LogView()
    {
        _viewport.SetWrap(false);
    }

    /// <summary>
    /// Represents title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Logs";

    /// <summary>
    /// Represents focus marker.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Represents show focus marker.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Represents title style.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents entry style.
    /// </summary>
    public TesseraStyle EntryStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents paused title style.
    /// </summary>
    public TesseraStyle PausedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into border glyphs while the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents border.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Represents padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Represents auto scroll.
    /// </summary>
    public bool AutoScroll
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Represents is paused.
    /// </summary>
    public bool IsPaused
    {
        get;
        private set;
    }

    /// <summary>
    /// Represents count.
    /// </summary>
    public int Count => _entries.Count;

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <summary>
    /// Executes append.
    /// </summary>
    /// <param name="line">The line value.</param>
    public void Append(string line)
    {
        if (IsPaused)
        {
            return;
        }

        var value = line ?? string.Empty;
        _entries.Add(value);
        if (!HasActiveFilter() || value.Contains(_filter, StringComparison.OrdinalIgnoreCase))
        {
            _viewport.AppendRawLine(value);
        }

        if (AutoScroll)
        {
            _viewport.ScrollToBottom();
        }
    }

    /// <summary>
    /// Executes clear.
    /// </summary>
    public void Clear()
    {
        _entries.Clear();
        _viewport.Clear();
    }

    /// <summary>
    /// Executes set filter.
    /// </summary>
    /// <param name="filter">The filter value.</param>
    public void SetFilter(string filter)
    {
        var normalized = filter ?? string.Empty;
        if (string.Equals(_filter, normalized, StringComparison.Ordinal))
        {
            return;
        }

        _filter = normalized;
        RefreshViewport();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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
            Padding,
            ResolveBorderStyleText());
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
        if (!HasActiveFilter())
        {
            _viewport.SetLines(_entries);
            return;
        }

        var filtered = new List<string>();
        for (var index = 0; index < _entries.Count; index++)
        {
            var entry = _entries[index];
            if (entry.Contains(_filter, StringComparison.OrdinalIgnoreCase))
            {
                filtered.Add(entry);
            }
        }

        _viewport.SetLines(filtered);
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

    private bool HasActiveFilter()
    {
        return !string.IsNullOrWhiteSpace(_filter);
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }
}
