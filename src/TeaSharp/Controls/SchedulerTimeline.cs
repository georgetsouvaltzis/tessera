using System.Globalization;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a selectable scheduler timeline for planning and time-slot workflows.
/// </summary>
public sealed class SchedulerTimeline : Control
{
    private readonly List<SchedulerEntry> _entries = [];
    private int _selectedIndex;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>
    /// Occurs when scheduler selection changes.
    /// </summary>
    public event EventHandler<SchedulerSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets control title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Schedule";

    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    public bool ShowFocusMarker { get; set; } = true;

    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle TimeTextStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle EntryTextStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle MetaTextStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle MutedRowStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle ConflictRowStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    public Thickness Padding { get; set; }

    public int PageSize { get; set; } = 8;

    public bool ShowDuration { get; set; } = true;

    public string TimeFormat
    {
        get;
        set => field = string.IsNullOrWhiteSpace(value) ? "HH:mm" : value;
    } = "HH:mm";

    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = ">";

    public string UnselectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = " ";

    public string ConflictMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "!";

    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no entries)";

    public TeaStyle EmptyTextStyle { get; set; } = TeaStyle.Empty;

    public IReadOnlyList<SchedulerEntry> Entries => _entries;
    public int SelectedIndex => _entries.Count == 0 ? -1 : _selectedIndex;
    public SchedulerEntry? SelectedEntry => _entries.Count == 0 ? null : _entries[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces timeline entries and keeps deterministic sort by start/end/title.
    /// </summary>
    /// <param name="entries">Entries to show.</param>
    public void SetEntries(IEnumerable<SchedulerEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var previousIndex = SelectedIndex;
        var previousEntry = SelectedEntry;

        _entries.Clear();
        foreach (var entry in entries)
        {
            if (entry is null)
            {
                continue;
            }

            _entries.Add(CloneEntry(entry));
        }

        SortEntries();
        _selectedIndex = _entries.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _entries.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - 1));
        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
    }

    public void AddEntry(SchedulerEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        _entries.Add(CloneEntry(entry));
        SortEntries();
        _selectedIndex = _entries.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _entries.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - 1));
    }

    public void Clear()
    {
        var previousIndex = SelectedIndex;
        var previousEntry = SelectedEntry;
        _entries.Clear();
        _selectedIndex = 0;
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
    }

    /// <summary>
    /// Selects an entry by index.
    /// </summary>
    /// <param name="index">Requested entry index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise, <see langword="false" />.</returns>
    public bool Select(int index)
    {
        return SetSelectedIndex(index);
    }

    /// <summary>
    /// Sets the selected entry index using bounds clamping.
    /// </summary>
    /// <param name="index">Requested entry index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise, <see langword="false" />.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_entries.Count == 0)
        {
            return false;
        }

        var next = Math.Clamp(index, 0, _entries.Count - 1);
        if (next == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousEntry = _entries[previousIndex];
        _selectedIndex = next;
        EnsureSelectionVisible(_lastViewportRows);
        SelectionChanged?.Invoke(this, new SchedulerSelectionChangedEventArgs(previousIndex, _selectedIndex, previousEntry, _entries[_selectedIndex]));
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _entries.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_entries.Count - 1);
        }

        var page = Math.Max(1, _lastViewportRows > 0 ? _lastViewportRows : PageSize);
        if (key.Is(Key.PageUp))
        {
            return SetSelectedIndex(_selectedIndex - page);
        }

        if (key.Is(Key.PageDown))
        {
            return SetSelectedIndex(_selectedIndex + page);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = bounds.Inset(Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var headerRows = HasTitleRow() ? 1 : 0;
        var rowsY = content.Y + headerRows;
        var rowsHeight = Math.Max(0, content.Height - headerRows);
        _lastViewportRows = Math.Max(1, rowsHeight);

        if (pointer.Kind == PointerEventKind.Wheel && _entries.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1);
            }
        }

        if (pointer.Kind != PointerEventKind.Press
            || pointer.Button != PointerButton.Left
            || !content.Contains(pointer.X, pointer.Y)
            || pointer.Y < rowsY
            || _entries.Count == 0)
        {
            return Handle(message);
        }

        RequestFocus();
        EnsureSelectionVisible(_lastViewportRows);
        var row = pointer.Y - rowsY;
        var target = _scrollOffset + row;
        if (target < 0 || target >= _entries.Count)
        {
            return true;
        }

        return SetSelectedIndex(target);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = clipped.Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        var y = content.Y;
        if (HasTitleRow())
        {
            canvas.WriteText(content.X, y, ApplyStyle(FormatTitle(), ResolveStyle(IsFocused ? FocusedTitleStyle : TitleStyle)), content.Width);
            y++;
        }

        var rowsHeight = Math.Max(0, content.Bottom - y);
        _lastViewportRows = Math.Max(1, rowsHeight);
        if (_entries.Count == 0 || rowsHeight <= 0)
        {
            if (rowsHeight > 0)
            {
                canvas.WriteText(content.X, y, ApplyStyle(EmptyText, ResolveStyle(EmptyTextStyle)), content.Width);
            }

            return;
        }

        EnsureSelectionVisible(_lastViewportRows);
        var rowCount = Math.Min(_lastViewportRows, _entries.Count - _scrollOffset);
        for (var row = 0; row < rowCount; row++)
        {
            var entryIndex = _scrollOffset + row;
            var selected = entryIndex == _selectedIndex;
            var conflict = entryIndex > 0 && _entries[entryIndex].Start < _entries[entryIndex - 1].End;
            var line = RenderEntryLine(_entries[entryIndex], selected, conflict);
            canvas.WriteText(content.X, y + row, line, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 28;
        for (var index = 0; index < _entries.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(BuildPlainEntryLine(_entries[index], selected: false, conflict: false)));
        }

        width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitle()) + 2);
        var height = Math.Max(6, (HasTitleRow() ? 1 : 0) + Math.Min(10, Math.Max(_entries.Count, 1)));
        width += Padding.Horizontal;
        height += Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string RenderEntryLine(SchedulerEntry entry, bool selected, bool conflict)
    {
        var prefix = selected ? SelectedMarker : UnselectedMarker;
        var conflictToken = conflict ? ConflictMarker : " ";
        var timeText = BuildTimeRange(entry);
        var bodyText = BuildBody(entry);
        var durationText = ShowDuration ? $" ({Math.Max(0, (int)Math.Round((entry.End - entry.Start).TotalMinutes, MidpointRounding.AwayFromZero))}m)" : string.Empty;

        var rowStyle = ResolveRowStateStyle(entry, selected, conflict);
        var prefixStyled = ApplyStyle($"{prefix}{conflictToken} ", rowStyle);
        var timeStyled = ApplyStyle(timeText, rowStyle.Merge(TimeTextStyle));
        var bodyStyled = ApplyStyle(bodyText, rowStyle.Merge(EntryTextStyle));
        var durationStyled = ApplyStyle(durationText, rowStyle.Merge(MetaTextStyle));
        return $"{prefixStyled}{timeStyled} {bodyStyled}{durationStyled}";
    }

    private string BuildPlainEntryLine(SchedulerEntry entry, bool selected, bool conflict)
    {
        var prefix = selected ? SelectedMarker : UnselectedMarker;
        var conflictToken = conflict ? ConflictMarker : " ";
        return $"{prefix}{conflictToken} {BuildTimeRange(entry)} {BuildBody(entry)}";
    }

    private string BuildTimeRange(SchedulerEntry entry)
    {
        return $"{entry.Start.ToString(TimeFormat, CultureInfo.InvariantCulture)}-{entry.End.ToString(TimeFormat, CultureInfo.InvariantCulture)}";
    }

    private static string BuildBody(SchedulerEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Details))
        {
            return entry.Title;
        }

        return $"{entry.Title} · {entry.Details.Trim()}";
    }

    private TeaStyle ResolveRowStateStyle(SchedulerEntry entry, bool selected, bool conflict)
    {
        var style = TeaStyle.Empty;
        if (entry.IsMuted)
        {
            style = style.Merge(MutedRowStyle);
        }

        if (conflict)
        {
            style = style.Merge(ConflictRowStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedRowStyle);
        }

        return ResolveStyle(style);
    }

    private TeaStyle ResolveStyle(TeaStyle style)
    {
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, SchedulerEntry? previousEntry)
    {
        var selectedIndex = SelectedIndex;
        var selectedEntry = SelectedEntry;
        var changed = previousIndex != selectedIndex
            || !ReferenceEquals(previousEntry, selectedEntry);
        if (changed)
        {
            SelectionChanged?.Invoke(this, new SchedulerSelectionChangedEventArgs(previousIndex, selectedIndex, previousEntry, selectedEntry));
        }
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_entries.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        var rows = Math.Max(1, viewportRows);
        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
            return;
        }

        if (_selectedIndex >= _scrollOffset + rows)
        {
            _scrollOffset = _selectedIndex - rows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - rows));
    }

    private void SortEntries()
    {
        _entries.Sort(static (left, right) =>
        {
            var byStart = left.Start.CompareTo(right.Start);
            if (byStart != 0)
            {
                return byStart;
            }

            var byEnd = left.End.CompareTo(right.End);
            if (byEnd != 0)
            {
                return byEnd;
            }

            return string.Compare(left.Title, right.Title, StringComparison.Ordinal);
        });
    }

    private bool HasTitleRow()
    {
        return !string.IsNullOrEmpty(Title);
    }

    private string FormatTitle()
    {
        if (!IsFocused || !ShowFocusMarker || string.IsNullOrWhiteSpace(FocusMarker))
        {
            return Title;
        }

        return string.IsNullOrEmpty(Title) ? string.Empty : $"{Title} {FocusMarker}";
    }

    private static SchedulerEntry CloneEntry(SchedulerEntry entry)
    {
        return new SchedulerEntry(entry.Id, entry.Title, entry.Start, entry.End, entry.Details, entry.IsMuted);
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
    }
}
