using System.Globalization;
using System.Text;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a selectable task-run dashboard for build/test/deploy workflows.
/// </summary>
public sealed class TaskRunnerPanel : Control
{
    private readonly List<TaskRunItem> _items = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>Occurs when selected row changes.</summary>
    public event EventHandler<TaskRunnerSelectionChangedEventArgs>? SelectionChanged;
    /// <summary>Gets or sets control title.</summary>
    public string Title { get; set; } = "Task Runner";
    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";
    /// <summary>Gets or sets whether <see cref="FocusMarker"/> is rendered while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;
    /// <summary>Gets or sets text rendered when there are no task rows.</summary>
    public string EmptyText { get; set; } = "(no tasks)";
    /// <summary>Gets or sets whether row timestamps are rendered.</summary>
    public bool ShowTimestamp { get; set; } = true;
    /// <summary>Gets or sets timestamp format used by row rendering.</summary>
    public string TimestampFormat { get; set; } = "HH:mm:ss";
    /// <summary>Gets or sets whether appended rows are auto-selected.</summary>
    public bool AutoFollow { get; set; } = true;
    /// <summary>Gets or sets border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }
    /// <summary>Gets or sets title style while not focused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets title style while focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets border glyph style while not focused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets border glyph style merged while focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets base row style.</summary>
    public TesseraStyle RowStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style merged into hovered rows.</summary>
    public TesseraStyle HoveredRowStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style merged into selected rows.</summary>
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style merged into selected rows while focused.</summary>
    public TesseraStyle FocusedSelectedRowStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style merged while the control is disabled.</summary>
    public TesseraStyle DisabledRowStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets base style for status markers.</summary>
    public TesseraStyle StatusMarkerStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for running marker state.</summary>
    public TesseraStyle RunningStatusStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for succeeded marker state.</summary>
    public TesseraStyle SucceededStatusStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for failed marker state.</summary>
    public TesseraStyle FailedStatusStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for empty-state text.</summary>
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets current task rows.</summary>
    public IReadOnlyList<TaskRunItem> Items => _items;
    /// <summary>Gets selected row index, or <c>-1</c> when empty.</summary>
    public int SelectedIndex => _selectedIndex;
    /// <summary>Gets selected row, if any.</summary>
    public TaskRunItem? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;
    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Replaces all task rows.</summary>
    /// <param name="items">Rows to render.</param>
    public void SetItems(IEnumerable<TaskRunItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _items.Clear();
        foreach (var item in items.Where(static item => item is not null))
        {
            _items.Add(new TaskRunItem(item.Id, item.Name, item.Status, item.Description, item.UpdatedAt));
        }

        if (_items.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            _scrollOffset = 0;
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _items.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
            _scrollOffset = Math.Clamp(_scrollOffset, 0, _items.Count - 1);
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>Appends one task row.</summary>
    /// <param name="item">Row to append.</param>
    public void Append(TaskRunItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _items.Add(new TaskRunItem(item.Id, item.Name, item.Status, item.Description, item.UpdatedAt));
        if (AutoFollow)
        {
            _selectedIndex = _items.Count - 1;
            EnsureSelectionVisible(_lastViewportRows);
        }
        else if (_selectedIndex < 0)
        {
            _selectedIndex = 0;
        }

        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>Clears all task rows and selection state.</summary>
    public void Clear()
    {
        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _items.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>Sets selected row index using bounds clamping.</summary>
    /// <param name="index">Requested row index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _items.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _selectedIndex = clamped;
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        var page = Math.Max(1, _lastViewportRows);
        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.Home)) return SetSelectedIndex(0);
        if (key.Is(Key.End)) return SetSelectedIndex(_items.Count - 1);
        if (key.Is(Key.PageDown)) return SetSelectedIndex(_selectedIndex + page);
        if (key.Is(Key.PageUp)) return SetSelectedIndex(_selectedIndex - page);
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

        if (pointer.Kind == PointerEventKind.Wheel && _items.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedIndex(_selectedIndex + 1) || changed;
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedIndex(_selectedIndex - 1) || changed;
        }

        if (!inside)
        {
            return changed;
        }

        EnsureSelectionVisible(content.Height);
        var hovered = _scrollOffset + (pointer.Y - content.Y);
        hovered = hovered < 0 || hovered >= _items.Count ? -1 : hovered;
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

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

        if (_items.Count == 0)
        {
            var style = IsDisabled ? EmptyStyle.Merge(DisabledRowStyle) : EmptyStyle;
            WriteStyledText(canvas, content.X, content.Y, EmptyText ?? string.Empty, style, content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(_lastViewportRows);
        var visible = Math.Min(content.Height, _items.Count - _scrollOffset);
        for (var row = 0; row < visible; row++)
        {
            var itemIndex = _scrollOffset + row;
            var item = _items[itemIndex];
            var y = content.Y + row;
            var marker = ResolveStatusMarker(item.Status);
            var markerStyle = ResolveMarkerStyle(itemIndex, item.Status);
            var rowStyle = ResolveRowStyle(itemIndex);
            WriteStyledText(canvas, content.X, y, marker, markerStyle, content.Width);
            if (content.Width > 1)
            {
                WriteStyledText(canvas, content.X + 1, y, " ", rowStyle, 1);
            }

            if (content.Width > 2)
            {
                WriteStyledText(canvas, content.X + 2, y, BuildRowBody(item), rowStyle, content.Width - 2);
            }
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 6);
        for (var index = 0; index < _items.Count; index++)
        {
            var markerWidth = Math.Max(1, ControlTextLayout.MeasureDisplayWidth(ResolveStatusMarker(_items[index].Status)));
            var bodyWidth = ControlTextLayout.MeasureDisplayWidth(BuildRowBody(_items[index]));
            width = Math.Max(width, markerWidth + 1 + bodyWidth + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2));
        }

        var rowCount = Math.Max(1, _items.Count);
        var height = Math.Max(4, rowCount + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2));
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
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

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_items.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedIndex < 0)
        {
            _selectedIndex = 0;
        }

        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
        }
        else if (_selectedIndex >= _scrollOffset + viewportRows)
        {
            _scrollOffset = _selectedIndex - viewportRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _items.Count - viewportRows));
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, TaskRunItem? previousItem)
    {
        if (previousIndex == _selectedIndex && ReferenceEquals(previousItem, SelectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new TaskRunnerSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, SelectedItem));
    }

    private TesseraStyle ResolveRowStyle(int index)
    {
        var style = RowStyle;
        if (index == _hoveredIndex) style = style.Merge(HoveredRowStyle);
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedRowStyle);
            if (IsFocused) style = style.Merge(FocusedSelectedRowStyle);
        }

        if (IsDisabled) style = style.Merge(DisabledRowStyle);
        return style;
    }

    private TesseraStyle ResolveMarkerStyle(int index, TaskRunStatus status)
    {
        var style = StatusMarkerStyle.Merge(ResolveStatusStyle(status));
        if (index == _hoveredIndex) style = style.Merge(HoveredRowStyle);
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedRowStyle);
            if (IsFocused) style = style.Merge(FocusedSelectedRowStyle);
        }

        if (IsDisabled) style = style.Merge(DisabledRowStyle);
        return style;
    }

    private TesseraStyle ResolveStatusStyle(TaskRunStatus status)
    {
        return status switch
        {
            TaskRunStatus.Running => RunningStatusStyle,
            TaskRunStatus.Succeeded => SucceededStatusStyle,
            TaskRunStatus.Failed => FailedStatusStyle,
            _ => TesseraStyle.Empty,
        };
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledRowStyle) : style;
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
            return string.Concat(Title ?? string.Empty, " ", FocusMarker);
        }

        return Title ?? string.Empty;
    }

    private string BuildRowBody(TaskRunItem item)
    {
        var name = NormalizeSingleLine(item.Name);
        var description = NormalizeSingleLine(item.Description);
        var builder = new StringBuilder(name.Length + description.Length + 24);
        if (ShowTimestamp)
        {
            var format = string.IsNullOrWhiteSpace(TimestampFormat) ? "HH:mm:ss" : TimestampFormat;
            builder.Append(item.UpdatedAt.ToString(format, CultureInfo.InvariantCulture));
            builder.Append(' ');
        }

        builder.Append(ResolveStatusLabel(item.Status));
        builder.Append(' ');
        builder.Append(name);
        if (!string.IsNullOrWhiteSpace(description))
        {
            builder.Append(" - ");
            builder.Append(description);
        }

        return builder.ToString();
    }

    private static string ResolveStatusMarker(TaskRunStatus status)
    {
        return status switch
        {
            TaskRunStatus.Running => "▶",
            TaskRunStatus.Succeeded => "✓",
            TaskRunStatus.Failed => "✕",
            TaskRunStatus.Skipped => "∘",
            TaskRunStatus.Canceled => "!",
            _ => "·",
        };
    }

    private static string ResolveStatusLabel(TaskRunStatus status)
    {
        return status switch
        {
            TaskRunStatus.Running => "RUN",
            TaskRunStatus.Succeeded => "OK",
            TaskRunStatus.Failed => "FAIL",
            TaskRunStatus.Skipped => "SKIP",
            TaskRunStatus.Canceled => "CXL",
            _ => "QUE",
        };
    }

    private static string NormalizeSingleLine(string? value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Replace('\r', ' ').Replace('\n', ' ');
    }

    private static void WriteStyledText(Canvas canvas, int x, int y, string text, TesseraStyle style, int width)
    {
        canvas.WriteText(x, y, style.IsEmpty ? text : style.Render(text), width);
    }
}
