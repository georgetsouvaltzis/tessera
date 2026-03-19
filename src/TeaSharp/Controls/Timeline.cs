using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a selectable timeline of temporal entries.
/// </summary>
public sealed class Timeline : Control
{
    private readonly List<TimelineEntry> _entries = [];
    private int _selectedIndex;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>
    /// Occurs when the selected timeline entry changes.
    /// </summary>
    public event EventHandler<TimelineSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets timeline title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Timeline";

    /// <summary>
    /// Gets or sets marker appended to title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether focus marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets title style when not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for timestamp text.
    /// </summary>
    public TeaStyle TimestampStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for entry label text.
    /// </summary>
    public TeaStyle LabelStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for additional content text.
    /// </summary>
    public TeaStyle ContentStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for separators and status text.
    /// </summary>
    public TeaStyle SeparatorStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into muted rows.
    /// </summary>
    public TeaStyle MutedStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged when control is disabled.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets fallback page size used by PageUp/PageDown navigation.
    /// </summary>
    public int PageSize { get; set; } = 8;

    /// <summary>
    /// Gets or sets separator shown between timestamp and label.
    /// </summary>
    public string Separator
    {
        get;
        set => field = value ?? string.Empty;
    } = " | ";

    /// <summary>
    /// Gets configured entries.
    /// </summary>
    public IReadOnlyList<TimelineEntry> Entries => _entries;

    /// <summary>
    /// Gets selected index.
    /// Returns <c>-1</c> when there are no entries.
    /// </summary>
    public int SelectedIndex => _entries.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    /// Gets selected entry.
    /// </summary>
    public TimelineEntry? SelectedItem => _entries.Count == 0 ? null : _entries[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces timeline entries.
    /// </summary>
    /// <param name="entries">Entries in display order.</param>
    public void SetEntries(IEnumerable<TimelineEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;

        _entries.Clear();
        foreach (var entry in entries)
        {
            if (entry is not null)
            {
                _entries.Add(entry);
            }
        }

        _selectedIndex = _entries.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _entries.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - 1));
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Selects an entry by index.
    /// </summary>
    /// <param name="index">The requested selected index.</param>
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

        _lastViewportRows = Math.Max(1, content.Height);
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
            || _entries.Count == 0)
        {
            return Handle(message);
        }

        RequestFocus();
        EnsureSelectionVisible(_lastViewportRows);
        var row = pointer.Y - content.Y;
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

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped, title, Border, Padding);
        if (content.IsEmpty)
        {
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        if (_entries.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(empty)", MutedStyle), content.Width);
            return;
        }

        EnsureSelectionVisible(_lastViewportRows);
        var rowCount = Math.Min(_lastViewportRows, _entries.Count - _scrollOffset);
        for (var row = 0; row < rowCount; row++)
        {
            var entryIndex = _scrollOffset + row;
            var entry = _entries[entryIndex];
            var selected = entryIndex == _selectedIndex;
            var line = FormatEntryLine(entry, selected);
            canvas.WriteText(content.X, content.Y + row, line, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 14;
        for (var index = 0; index < _entries.Count; index++)
        {
            var entry = _entries[index];
            var line = BuildPlainEntryLine(entry, selected: false);
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(line));
        }

        if (Border != BorderStyle.None)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleText()) + 4);
            width += 2;
        }

        width += Padding.Horizontal;
        var height = Math.Max(1, Math.Min(Math.Max(1, PageSize), Math.Max(1, _entries.Count))) + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string FormatEntryLine(TimelineEntry entry, bool selected)
    {
        var rowStyle = ResolveRowStyle(entry, selected);
        var marker = ApplyStyle(selected ? ">" : " ", rowStyle);
        var timestamp = ApplyStyle(entry.TimestampText, rowStyle.Merge(TimestampStyle));
        var separator = ApplyStyle(Separator, rowStyle.Merge(SeparatorStyle));
        var label = ApplyStyle(entry.Label, rowStyle.Merge(LabelStyle));
        var content = string.IsNullOrEmpty(entry.Content)
            ? string.Empty
            : ApplyStyle($" - {entry.Content}", rowStyle.Merge(ContentStyle));
        var status = string.IsNullOrEmpty(entry.Status)
            ? string.Empty
            : ApplyStyle($" [{entry.Status}]", rowStyle.Merge(SeparatorStyle));
        return $"{marker} {timestamp}{separator}{label}{content}{status}";
    }

    private string BuildPlainEntryLine(TimelineEntry entry, bool selected)
    {
        var marker = selected ? ">" : " ";
        var content = string.IsNullOrEmpty(entry.Content) ? string.Empty : $" - {entry.Content}";
        var status = string.IsNullOrEmpty(entry.Status) ? string.Empty : $" [{entry.Status}]";
        return $"{marker} {entry.TimestampText}{Separator}{entry.Label}{content}{status}";
    }

    private TeaStyle ResolveRowStyle(TimelineEntry entry, bool selected)
    {
        var style = TeaStyle.Empty;
        if (entry.IsMuted)
        {
            style = style.Merge(MutedStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedRowStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private bool SetSelectedIndex(int index)
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
        var previousItem = _entries[previousIndex];
        _selectedIndex = clamped;
        EnsureSelectionVisible(_lastViewportRows);
        SelectionChanged?.Invoke(
            this,
            new TimelineSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, _entries[_selectedIndex]));
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

    private void RaiseSelectionChangedIfNeeded(int previousIndex, TimelineEntry? previousItem)
    {
        if (previousIndex == SelectedIndex && ReferenceEquals(previousItem, SelectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new TimelineSelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }
}
