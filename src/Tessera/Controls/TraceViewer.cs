using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a selectable operation trace viewer.
/// </summary>
public sealed partial class TraceViewer : Control
{
    private readonly List<TraceEntry> _entries = [];
    private int _hoveredIndex = -1;
    private int _lastViewportRows = 8;
    private int _scrollOffset;
    private int _selectedIndex;

    /// <summary>
    ///     Gets or sets title text.
    /// </summary>
    public string Title { get; set; } = "Trace";

    /// <summary>
    ///     Gets or sets marker appended to title while focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether the focus marker is shown.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets title style when unfocused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets title style when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets border style when unfocused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets border style when focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets base row style.
    /// </summary>
    public TesseraStyle EntryStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets verbose row style.
    /// </summary>
    public TesseraStyle VerboseRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets informational row style.
    /// </summary>
    public TesseraStyle InfoRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets warning row style.
    /// </summary>
    public TesseraStyle WarningRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets error row style.
    /// </summary>
    public TesseraStyle ErrorRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets critical row style.
    /// </summary>
    public TesseraStyle CriticalRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into selected rows.
    /// </summary>
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into selected rows while focused.
    /// </summary>
    public TesseraStyle FocusedSelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into hovered rows.
    /// </summary>
    public TesseraStyle HoveredRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into muted rows.
    /// </summary>
    public TesseraStyle MutedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into rows while disabled.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style for empty text.
    /// </summary>
    public TesseraStyle EmptyTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    ///     Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    ///     Gets or sets marker rendered for selected row.
    /// </summary>
    public string SelectedMarker { get; set; } = ">";

    /// <summary>
    ///     Gets or sets marker rendered for unselected row.
    /// </summary>
    public string UnselectedMarker { get; set; } = " ";

    /// <summary>
    ///     Gets or sets timestamp format.
    /// </summary>
    public string TimeFormat
    {
        get;
        set => field = string.IsNullOrWhiteSpace(value) ? "HH:mm:ss.fff" : value;
    } = "HH:mm:ss.fff";

    /// <summary>
    ///     Gets or sets whether duration text is rendered.
    /// </summary>
    public bool ShowDuration { get; set; } = true;

    /// <summary>
    ///     Gets or sets fallback page size for keyboard paging.
    /// </summary>
    public int PageSize { get; set; } = 8;

    /// <summary>
    ///     Gets or sets empty text.
    /// </summary>
    public string EmptyText { get; set; } = "(no traces)";

    /// <summary>
    ///     Gets trace entries.
    /// </summary>
    public IReadOnlyList<TraceEntry> Entries => _entries;

    /// <summary>
    ///     Gets selected index, or <c>-1</c> when there are no entries.
    /// </summary>
    public int SelectedIndex => _entries.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    ///     Gets selected entry, if any.
    /// </summary>
    public TraceEntry? SelectedEntry => _entries.Count == 0 ? null : _entries[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    ///     Occurs when selected trace row changes.
    /// </summary>
    public event EventHandler<TraceSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    ///     Replaces trace entries with deterministic ordering by timestamp/operation/id.
    /// </summary>
    /// <param name="entries">Trace entries.</param>
    public void SetEntries(IEnumerable<TraceEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var previousIndex = SelectedIndex;
        var previousEntry = SelectedEntry;
        _entries.Clear();
        foreach (var entry in entries)
        {
            _entries.Add(CloneEntry(entry));
        }

        SortEntries();
        _selectedIndex = _entries.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _entries.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _entries.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - 1));
        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
    }

    /// <summary>
    ///     Adds one trace entry.
    /// </summary>
    /// <param name="entry">Trace entry to add.</param>
    public void AddEntry(TraceEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        _entries.Add(CloneEntry(entry));
        SortEntries();
        _selectedIndex = _entries.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _entries.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _entries.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - 1));
    }

    /// <summary>
    ///     Clears all trace entries.
    /// </summary>
    public void Clear()
    {
        var previousIndex = SelectedIndex;
        var previousEntry = SelectedEntry;
        _entries.Clear();
        _selectedIndex = 0;
        _hoveredIndex = -1;
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(previousIndex, previousEntry);
    }

    /// <summary>
    ///     Selects an entry by index.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool Select(int index)
    {
        return SetSelectedIndex(index);
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
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHoveredIndex(-1);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _entries.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1) || changed;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1) || changed;
            }
        }

        if (!inside)
        {
            return changed;
        }

        EnsureSelectionVisible(Math.Max(1, content.Height));
        var hovered = _scrollOffset + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _entries.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            changed |= SetHoveredIndex(hovered);
            changed |= SetSelectedIndex(hovered);
            return changed;
        }

        return changed;
    }

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
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        if (_entries.Count == 0)
        {
            var emptyStyle = IsDisabled ? EmptyTextStyle.Merge(DisabledStyle) : EmptyTextStyle;
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, emptyStyle), content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(_lastViewportRows);
        var rows = Math.Min(content.Height, _entries.Count - _scrollOffset);
        for (var row = 0; row < rows; row++)
        {
            var index = _scrollOffset + row;
            var entry = _entries[index];
            var line = FormatLine(entry, index == _selectedIndex);
            var style = ResolveRowStyle(entry, index == _selectedIndex, index == _hoveredIndex);
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 6);
        for (var index = 0; index < _entries.Count; index++)
        {
            var rowWidth = ControlTextLayout.MeasureDisplayWidth(FormatLine(_entries[index], false));
            width = Math.Max(width, rowWidth + 2);
        }

        var height = Math.Max(4, Math.Min(12, _entries.Count + 2));
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
