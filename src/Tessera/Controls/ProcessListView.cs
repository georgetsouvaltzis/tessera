using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a selectable process table with status/CPU/memory columns.
/// </summary>
public sealed class ProcessListView : Control
{
    private readonly List<ProcessListEntry> _entries = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>Raised when selection changes.</summary>
    public event EventHandler<ProcessListSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>Gets or sets control title text.</summary>
    public string Title { get; set; } = "Processes";

    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>Gets or sets whether <see cref="FocusMarker"/> is rendered while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets text shown when there are no process rows.</summary>
    public string EmptyText { get; set; } = "(no processes)";

    /// <summary>Gets or sets whether column header is rendered.</summary>
    public bool ShowHeader { get; set; } = true;

    /// <summary>Gets or sets selected-row marker text.</summary>
    public string SelectedMarker { get; set; } = ">";

    /// <summary>Gets or sets non-selected-row marker text.</summary>
    public string UnselectedMarker { get; set; } = " ";

    /// <summary>Gets or sets status header text.</summary>
    public string StatusHeaderText { get; set; } = "STATUS";

    /// <summary>Gets or sets process-id header text.</summary>
    public string PidHeaderText { get; set; } = "PID";

    /// <summary>Gets or sets process-name header text.</summary>
    public string NameHeaderText { get; set; } = "NAME";

    /// <summary>Gets or sets CPU header text.</summary>
    public string CpuHeaderText { get; set; } = "CPU%";

    /// <summary>Gets or sets memory header text.</summary>
    public string MemoryHeaderText { get; set; } = "MEM";

    /// <summary>Gets or sets column-separator text.</summary>
    public string ColumnSeparatorText { get; set; } = " | ";

    /// <summary>Gets or sets CPU format string.</summary>
    public string CpuFormat
    {
        get;
        set => field = string.IsNullOrWhiteSpace(value) ? "0.0" : value;
    } = "0.0";

    /// <summary>Gets or sets memory format string.</summary>
    public string MemoryFormat
    {
        get;
        set => field = string.IsNullOrWhiteSpace(value) ? "0.0" : value;
    } = "0.0";

    /// <summary>Gets or sets title style while unfocused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets title style while focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets border style while unfocused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets border style while focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets header row style.</summary>
    public TesseraStyle HeaderStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets base data-row style.</summary>
    public TesseraStyle RowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets hovered-row style.</summary>
    public TesseraStyle HoveredRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets selected-row style.</summary>
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets selected-row style while focused.</summary>
    public TesseraStyle FocusedSelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into the status column text.</summary>
    public TesseraStyle StatusStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into muted rows.</summary>
    public TesseraStyle MutedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged while disabled.</summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets empty-state text style.</summary>
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets current process rows.</summary>
    public IReadOnlyList<ProcessListEntry> Entries => _entries;

    /// <summary>Gets selected index, or <c>-1</c> when empty.</summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>Gets selected row, if any.</summary>
    public ProcessListEntry? SelectedEntry =>
        _selectedIndex >= 0 && _selectedIndex < _entries.Count ? _entries[_selectedIndex] : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Replaces all rows.</summary>
    /// <param name="entries">Rows to render.</param>
    public void SetEntries(IEnumerable<ProcessListEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var previousIndex = _selectedIndex;
        var previousEntry = SelectedEntry;
        _entries.Clear();
        foreach (var entry in entries)
        {
            if (entry is not null) _entries.Add(CloneEntry(entry));
        }

        if (_entries.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            _scrollOffset = 0;
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _entries.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _entries.Count - 1);
            EnsureSelectionVisible(_lastViewportRows);
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
    }

    /// <summary>Appends one row.</summary>
    /// <param name="entry">Entry to append.</param>
    public void Append(ProcessListEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var previousIndex = _selectedIndex;
        var previousEntry = SelectedEntry;
        _entries.Add(CloneEntry(entry));
        if (_selectedIndex < 0) _selectedIndex = 0;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
    }

    /// <summary>Clears rows and selection state.</summary>
    public void Clear()
    {
        var previousIndex = _selectedIndex;
        var previousEntry = SelectedEntry;
        _entries.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
    }

    /// <summary>Selects by index using bounds clamping.</summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool Select(int index) => SetSelectedIndex(index);

    /// <summary>Sets the selected row index using bounds clamping.</summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_entries.Count == 0) return false;
        var clamped = Math.Clamp(index, 0, _entries.Count - 1);
        if (clamped == _selectedIndex) return false;
        var previousIndex = _selectedIndex;
        var previousEntry = SelectedEntry;
        _selectedIndex = clamped;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _entries.Count == 0 || message is not KeyPressed key) return false;
        var page = Math.Max(1, _lastViewportRows);
        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.Home)) return SetSelectedIndex(0);
        if (key.Is(Key.End)) return SetSelectedIndex(_entries.Count - 1);
        if (key.Is(Key.PageDown)) return SetSelectedIndex(_selectedIndex + page);
        if (key.Is(Key.PageUp)) return SetSelectedIndex(_selectedIndex - page);
        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty) return Handle(message);
        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty) return Handle(message);

        var rowY = content.Y + (HasTitle() ? 1 : 0) + (ShowHeader ? 1 : 0);
        var rowsHeight = Math.Max(0, content.Bottom - rowY);
        _lastViewportRows = Math.Max(1, rowsHeight);

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press) changed |= SetHoveredIndex(-1);

        if (pointer.Kind == PointerEventKind.Wheel && _entries.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedIndex(_selectedIndex + 1) || changed;
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedIndex(_selectedIndex - 1) || changed;
        }

        if (!inside || _entries.Count == 0 || pointer.Y < rowY || rowsHeight <= 0) return changed || Handle(message);
        EnsureSelectionVisible(rowsHeight);
        var hovered = _scrollOffset + (pointer.Y - rowY);
        if (hovered < 0 || hovered >= _entries.Count) hovered = -1;

        if (pointer.Kind == PointerEventKind.Motion) return SetHoveredIndex(hovered);
        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            changed |= SetHoveredIndex(hovered);
            changed |= SetSelectedIndex(hovered);
        }

        return changed;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty) return;

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty) return;

        var y = content.Y;
        if (ShowHeader)
        {
            WriteStyledText(canvas, content.X, y, BuildHeader(), ResolveHeaderStyle(), content.Width);
            y++;
        }

        var rowsHeight = Math.Max(0, content.Bottom - y);
        _lastViewportRows = Math.Max(1, rowsHeight);
        if (_entries.Count == 0 || rowsHeight <= 0)
        {
            if (rowsHeight > 0) WriteStyledText(canvas, content.X, y, EmptyText, ResolveEmptyStyle(), content.Width);
            return;
        }

        EnsureSelectionVisible(rowsHeight);
        var visible = Math.Min(rowsHeight, _entries.Count - _scrollOffset);
        for (var row = 0; row < visible; row++)
        {
            var index = _scrollOffset + row;
            WriteStyledText(canvas, content.X, y + row, BuildRow(_entries[index], index == _selectedIndex), ResolveRowStyle(index, _entries[index]), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(BuildHeader()) + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2));
        for (var index = 0; index < _entries.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(BuildRow(_entries[index], selected: index == _selectedIndex)) + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2));
        }

        var headerRows = ShowHeader ? 1 : 0;
        var rowCount = Math.Max(1, _entries.Count);
        var height = rowCount + headerRows + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index) return false;
        _hoveredIndex = index;
        return true;
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_entries.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedIndex < 0) _selectedIndex = 0;
        if (_selectedIndex < _scrollOffset) _scrollOffset = _selectedIndex;
        else if (_selectedIndex >= _scrollOffset + viewportRows) _scrollOffset = _selectedIndex - viewportRows + 1;
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - viewportRows));
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, ProcessListEntry? previousEntry)
    {
        if (previousIndex == _selectedIndex && ReferenceEquals(previousEntry, SelectedEntry)) return;
        SelectionChanged?.Invoke(this, new ProcessListSelectionChangedEventArgs(previousIndex, _selectedIndex, previousEntry, SelectedEntry));
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TesseraStyle ResolveHeaderStyle()
    {
        var style = HeaderStyle;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TesseraStyle ResolveEmptyStyle()
    {
        var style = EmptyStyle;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TesseraStyle ResolveRowStyle(int index, ProcessListEntry entry)
    {
        var style = RowStyle.Merge(entry.Style).Merge(StatusStyle);
        if (entry.IsMuted) style = style.Merge(MutedRowStyle);
        if (index == _hoveredIndex) style = style.Merge(HoveredRowStyle);
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedRowStyle);
            if (IsFocused) style = style.Merge(FocusedSelectedRowStyle);
        }

        if (IsDisabled) style = style.Merge(DisabledStyle);
        return style;
    }

    private string RenderTitle()
    {
        var title = MeasureTitle();
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return style.IsEmpty ? title : style.Render(title);
    }

    private string MeasureTitle()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return string.Concat(Title, " ", FocusMarker);
        }

        return Title;
    }

    private bool HasTitle() => !string.IsNullOrWhiteSpace(Title);

    private string BuildHeader()
    {
        return string.Concat(
            " ",
            StatusHeaderText.PadRight(6),
            ColumnSeparatorText,
            PidHeaderText.PadLeft(6),
            ColumnSeparatorText,
            NameHeaderText,
            ColumnSeparatorText,
            CpuHeaderText.PadLeft(6),
            ColumnSeparatorText,
            MemoryHeaderText.PadLeft(7));
    }

    private string BuildRow(ProcessListEntry entry, bool selected)
    {
        var marker = selected ? SelectedMarker : UnselectedMarker;
        var status = ResolveStatusText(entry.Status);
        var pid = entry.Pid.ToString(CultureInfo.InvariantCulture);
        var name = NormalizeSingleLine(entry.Name);
        var cpu = string.Concat(entry.CpuPercent.ToString(CpuFormat, CultureInfo.InvariantCulture), "%");
        var mem = string.Concat(entry.MemoryMb.ToString(MemoryFormat, CultureInfo.InvariantCulture), "M");
        return string.Concat(
            marker,
            " ",
            status.PadRight(6),
            ColumnSeparatorText,
            pid.PadLeft(6),
            ColumnSeparatorText,
            name,
            ColumnSeparatorText,
            cpu.PadLeft(6),
            ColumnSeparatorText,
            mem.PadLeft(7));
    }

    private string ResolveStatusText(ProcessListStatus status)
    {
        return status switch
        {
            ProcessListStatus.Running => "RUN",
            ProcessListStatus.Sleeping => "SLP",
            ProcessListStatus.Stopped => "STP",
            ProcessListStatus.Zombie => "ZMB",
            _ => "UNK",
        };
    }

    private static string NormalizeSingleLine(string? value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Replace('\r', ' ').Replace('\n', ' ');
    }

    private static ProcessListEntry CloneEntry(ProcessListEntry entry)
    {
        return new ProcessListEntry(entry.Pid, entry.Name, entry.Status, entry.CpuPercent, entry.MemoryMb)
        {
            IsMuted = entry.IsMuted,
            Style = entry.Style,
        };
    }

    private static void WriteStyledText(Canvas canvas, int x, int y, string text, TesseraStyle style, int width)
    {
        if (width <= 0) return;
        canvas.WriteText(x, y, style.IsEmpty ? text : style.Render(text), width);
    }
}
