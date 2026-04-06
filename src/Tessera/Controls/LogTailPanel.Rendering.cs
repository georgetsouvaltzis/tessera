using System.Globalization;
using System.Text;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class LogTailPanel
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        if (_entries.Count == 0)
        {
            var emptyStyle = ResolveEntryStyle(-1, null);
            WriteStyledLine(canvas, content.X, content.Y, content.Width, EmptyText, emptyStyle);
            return;
        }

        EnsureEntryBodyCache();
        var visibleRows = Math.Min(_entries.Count, content.Height);
        var start = ComputeWindowStart(content.Height);
        for (var row = 0; row < visibleRows; row++)
        {
            var index = start + row;
            var entry = _entries[index];
            var marker = index == _selectedIndex ? SelectedMarker : UnselectedMarker;
            var body = _entryBodyCache[index];
            var style = ResolveEntryStyle(index, entry);
            var y = content.Y + row;
            if (style.IsEmpty)
            {
                canvas.WriteText(content.X, y, marker, content.Width);
                if (content.Width > 2)
                {
                    canvas.WriteText(content.X + 1, y, " ", 1);
                    canvas.WriteText(content.X + 2, y, body, content.Width - 2);
                }

                continue;
            }

            var line = string.Concat(marker, " ", body);
            canvas.WriteText(content.X, y, style.Render(line), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var markerWidth = Math.Max(ControlTextLayout.MeasureDisplayWidth(SelectedMarker), ControlTextLayout.MeasureDisplayWidth(UnselectedMarker));
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(Title) + 4);
        EnsureEntryBodyCache();
        for (var index = 0; index < _entryBodyCache.Count; index++)
        {
            var rowWidth = markerWidth + 1 + ControlTextLayout.MeasureDisplayWidth(_entryBodyCache[index]);
            width = Math.Max(width, rowWidth + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2));
        }

        var height = Math.Max(4, Math.Min(MaxEntries, Math.Max(1, _entries.Count)) + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2));
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private string RenderTitle()
    {
        var title = Title;
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            title = string.Concat(title, " ", FocusMarker);
        }

        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return style.IsEmpty ? title : style.Render(title);
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledEntryStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorEntryStyle);
        }

        return style;
    }

    private TesseraStyle ResolveEntryStyle(int index, LogEntry? entry)
    {
        var style = EntryStyle;
        if (index >= 0 && index == _hoveredIndex)
        {
            style = style.Merge(HoveredEntryStyle);
        }

        if (index >= 0 && index == _selectedIndex)
        {
            style = style.Merge(SelectedEntryStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedEntryStyle);
            }
        }

        if (entry is not null)
        {
            style = style.Merge(entry.Level switch
            {
                LogLevel.Trace => TraceEntryStyle,
                LogLevel.Debug => DebugEntryStyle,
                LogLevel.Warning => WarningEntryStyle,
                LogLevel.Error => ErrorEntryStyle,
                LogLevel.Critical => CriticalEntryStyle,
                _ => InfoEntryStyle,
            });

            if (entry.IsMuted)
            {
                style = style.Merge(MutedEntryStyle);
            }

            if (entry.HasError)
            {
                style = style.Merge(ErrorEntryStyle);
            }
        }

        if (HasError)
        {
            style = style.Merge(ErrorEntryStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledEntryStyle);
        }

        return style;
    }

    private void EnsureEntryBodyCache()
    {
        if (_entryBodyCache.Count != _entries.Count)
        {
            _entryBodyCache.Clear();
            _entryBodyCache.Capacity = _entries.Count;
            for (var index = 0; index < _entries.Count; index++)
            {
                _entryBodyCache.Add(string.Empty);
            }

            _entryCacheDirty = true;
        }

        if (!_entryCacheDirty)
        {
            return;
        }

        for (var index = 0; index < _entries.Count; index++)
        {
            _entryBodyCache[index] = BuildEntryBody(_entries[index]);
        }

        _entryCacheDirty = false;
    }

    private string BuildEntryBody(LogEntry entry)
    {
        var normalizedMessage = NormalizeSingleLine(entry.Message);
        if (!ShowTimestamp && !ShowLevel && (!ShowSource || string.IsNullOrWhiteSpace(entry.Source)))
        {
            return normalizedMessage;
        }

        var capacity = normalizedMessage.Length + 24 + entry.Source.Length;
        var builder = new StringBuilder(capacity);
        if (ShowTimestamp)
        {
            Span<char> timestampBuffer = stackalloc char[8];
            if (entry.Timestamp.TryFormat(timestampBuffer, out var written, "HH:mm:ss".AsSpan(), CultureInfo.InvariantCulture))
            {
                builder.Append('[');
                builder.Append(timestampBuffer[..written]);
                builder.Append("] ");
            }
        }

        if (ShowLevel)
        {
            builder.Append(LevelTag(entry.Level));
            builder.Append(' ');
        }

        if (ShowSource && !string.IsNullOrWhiteSpace(entry.Source))
        {
            builder.Append(entry.Source);
            builder.Append(": ");
        }

        builder.Append(normalizedMessage);
        return builder.ToString();
    }

    private static ReadOnlySpan<char> LevelTag(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => "INF",
        };
    }

    private static string NormalizeSingleLine(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var newlineIndex = value.AsSpan().IndexOfAny('\r', '\n');
        if (newlineIndex < 0)
        {
            return value;
        }

        var builder = new StringBuilder(value.Length);
        for (var index = 0; index < value.Length; index++)
        {
            var ch = value[index];
            builder.Append(ch is '\r' or '\n' ? ' ' : ch);
        }

        return builder.ToString();
    }

    private void WriteStyledLine(Canvas canvas, int x, int y, int width, string line, TesseraStyle style)
    {
        canvas.WriteText(x, y, style.IsEmpty ? line : style.Render(line), width);
    }
}
