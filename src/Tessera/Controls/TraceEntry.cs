using System.Globalization;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents one operation/request trace row.
/// </summary>
public sealed class TraceEntry
{
    /// <summary>
    /// Initializes a trace entry.
    /// </summary>
    /// <param name="id">Stable trace identifier.</param>
    /// <param name="timestamp">Trace timestamp.</param>
    /// <param name="operation">Operation/request name.</param>
    /// <param name="message">Trace message text.</param>
    /// <param name="severity">Trace severity.</param>
    /// <param name="durationMs">Optional duration in milliseconds.</param>
    /// <param name="metadata">Optional metadata text.</param>
    /// <param name="isMuted"><see langword="true" /> when row should render muted.</param>
    public TraceEntry(
        string id,
        DateTimeOffset timestamp,
        string operation,
        string message,
        TraceSeverity severity = TraceSeverity.Info,
        double? durationMs = null,
        string? metadata = null,
        bool isMuted = false)
    {
        Id = id ?? string.Empty;
        Timestamp = timestamp;
        Operation = operation ?? string.Empty;
        Message = message ?? string.Empty;
        Severity = severity;
        DurationMs = durationMs;
        Metadata = metadata ?? string.Empty;
        IsMuted = isMuted;
    }

    /// <summary>
    /// Gets stable trace identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets or sets trace timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets operation/request name.
    /// </summary>
    public string Operation
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets trace message text.
    /// </summary>
    public string Message
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets severity.
    /// </summary>
    public TraceSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets optional duration in milliseconds.
    /// </summary>
    public double? DurationMs { get; set; }

    /// <summary>
    /// Gets or sets optional metadata.
    /// </summary>
    public string Metadata
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets whether row should render muted.
    /// </summary>
    public bool IsMuted { get; set; }
}

public sealed partial class TraceViewer
{
    /// <summary>
    /// Sets the selected entry index using bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_entries.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _entries.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousEntry = SelectedEntry;
        _selectedIndex = clamped;
        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
        EnsureSelectionVisible(_lastViewportRows);
        return true;
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_entries.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
        }
        else if (_selectedIndex >= _scrollOffset + viewportRows)
        {
            _scrollOffset = _selectedIndex - viewportRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - viewportRows));
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private void SortEntries()
    {
        _entries.Sort(static (left, right) =>
        {
            var compare = left.Timestamp.CompareTo(right.Timestamp);
            if (compare != 0) return compare;
            compare = string.Compare(left.Operation, right.Operation, StringComparison.Ordinal);
            if (compare != 0) return compare;
            return string.Compare(left.Id, right.Id, StringComparison.Ordinal);
        });
    }

    private static TraceEntry CloneEntry(TraceEntry entry)
    {
        return new TraceEntry(
            entry.Id,
            entry.Timestamp,
            entry.Operation,
            entry.Message,
            entry.Severity,
            entry.DurationMs,
            entry.Metadata,
            entry.IsMuted);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, TraceEntry? previousEntry)
    {
        var selectedIndex = SelectedIndex;
        var selectedEntry = SelectedEntry;
        if (previousIndex == selectedIndex && ReferenceEquals(previousEntry, selectedEntry))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new TraceSelectionChangedEventArgs(previousIndex, selectedIndex, previousEntry, selectedEntry));
    }

    private string FormatLine(TraceEntry entry, bool selected)
    {
        var marker = selected ? SelectedMarker : UnselectedMarker;
        var timestamp = entry.Timestamp.ToString(TimeFormat, CultureInfo.InvariantCulture);
        var severity = ResolveSeverityToken(entry.Severity);
        var line = $"{marker} {timestamp} {severity} {entry.Operation}: {entry.Message}";
        if (ShowDuration && entry.DurationMs.HasValue)
        {
            line = $"{line} ({entry.DurationMs.Value:0.##}ms)";
        }

        if (!string.IsNullOrWhiteSpace(entry.Metadata))
        {
            line = $"{line} [{entry.Metadata}]";
        }

        return line;
    }

    private static string ResolveSeverityToken(TraceSeverity severity)
    {
        return severity switch
        {
            TraceSeverity.Verbose => "VRB",
            TraceSeverity.Warning => "WRN",
            TraceSeverity.Error => "ERR",
            TraceSeverity.Critical => "CRT",
            _ => "INF",
        };
    }

    private TesseraStyle ResolveRowStyle(TraceEntry entry, bool selected, bool hovered)
    {
        var style = EntryStyle.Merge(ResolveSeverityStyle(entry.Severity));
        if (entry.IsMuted)
        {
            style = style.Merge(MutedRowStyle);
        }

        if (hovered)
        {
            style = style.Merge(HoveredRowStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedRowStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedRowStyle);
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private TesseraStyle ResolveSeverityStyle(TraceSeverity severity)
    {
        return severity switch
        {
            TraceSeverity.Verbose => VerboseRowStyle,
            TraceSeverity.Warning => WarningRowStyle,
            TraceSeverity.Error => ErrorRowStyle,
            TraceSeverity.Critical => CriticalRowStyle,
            _ => InfoRowStyle,
        };
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string MeasureTitle()
    {
        return ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private static string ApplyStyle(string value, TesseraStyle style)
    {
        return style.IsEmpty ? value : style.Render(value);
    }
}
