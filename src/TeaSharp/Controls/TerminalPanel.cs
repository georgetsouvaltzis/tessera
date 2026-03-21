using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a terminal-style output panel with selectable rows.
/// </summary>
public sealed class TerminalPanel : Control
{
    private readonly List<TerminalPanelLine> _lines = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>
    /// Occurs when selected row changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<TerminalPanelLine>>? SelectionChanged;

    /// <summary>
    /// Gets or sets maximum retained rows. Values below 1 are clamped to 1.
    /// </summary>
    public int MaxLines { get; set => field = Math.Max(1, value); } = 2000;

    /// <summary>
    /// Gets or sets whether appending keeps selection pinned to the latest row.
    /// </summary>
    public bool FollowTail { get; set; } = true;

    /// <summary>
    /// Gets or sets whether line numbers are rendered.
    /// </summary>
    public bool ShowLineNumbers { get; set; } = true;

    /// <summary>
    /// Gets or sets text shown when there are no rows.
    /// </summary>
    public string EmptyText { get; set => field = value ?? string.Empty; } = "(no output)";

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets base style for standard output rows.
    /// </summary>
    public TeaStyle StandardOutputStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets base style for standard error rows.
    /// </summary>
    public TeaStyle StandardErrorStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets base style for command/input rows.
    /// </summary>
    public TeaStyle CommandStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets base style for system/meta rows.
    /// </summary>
    public TeaStyle SystemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into marker text.
    /// </summary>
    public TeaStyle MarkerStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rows.
    /// </summary>
    public TeaStyle HoveredLineStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TeaStyle SelectedLineStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows while focused.
    /// </summary>
    public TeaStyle FocusedSelectedLineStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged while control is disabled.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets empty-state text style.
    /// </summary>
    public TeaStyle EmptyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets rows in render order.
    /// </summary>
    public IReadOnlyList<TerminalPanelLine> Lines => _lines;

    /// <summary>
    /// Gets selected row index, or <c>-1</c> when empty.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets selected row, if any.
    /// </summary>
    public TerminalPanelLine? SelectedLine =>
        _selectedIndex >= 0 && _selectedIndex < _lines.Count
            ? _lines[_selectedIndex]
            : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces all rows.
    /// </summary>
    /// <param name="lines">Rows to render.</param>
    public void SetLines(IEnumerable<TerminalPanelLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        _lines.Clear();
        foreach (var line in lines)
            if (line is not null) _lines.Add(CloneLine(line));

        TrimToMaxLines();
        if (_lines.Count == 0) { _selectedIndex = -1; _hoveredIndex = -1; _scrollOffset = 0; return; }

        _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _lines.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _lines.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, _lines.Count - 1);
    }

    /// <summary>
    /// Appends one row.
    /// </summary>
    /// <param name="line">Row to append.</param>
    public void Append(TerminalPanelLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(CloneLine(line));
        TrimToMaxLines();

        if (_lines.Count == 0) { _selectedIndex = -1; _hoveredIndex = -1; _scrollOffset = 0; return; }

        if (FollowTail)
        {
            _ = SetSelectedIndex(_lines.Count - 1);
            EnsureSelectionVisible();
        }
        else if (_selectedIndex < 0)
        {
            _selectedIndex = 0;
        }
    }

    /// <summary>
    /// Appends one row using text and channel values.
    /// </summary>
    /// <param name="text">Row text.</param>
    /// <param name="channel">Row channel.</param>
    /// <param name="marker">Optional marker override.</param>
    public void Append(string text, TerminalPanelChannel channel = TerminalPanelChannel.StandardOutput, string? marker = null)
    {
        Append(new TerminalPanelLine(text, channel, marker));
    }

    /// <summary>
    /// Appends many rows.
    /// </summary>
    /// <param name="lines">Rows to append.</param>
    public void AppendRange(IEnumerable<TerminalPanelLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        foreach (var line in lines)
            if (line is not null) _lines.Add(CloneLine(line));

        TrimToMaxLines();
        if (_lines.Count == 0) { _selectedIndex = -1; _hoveredIndex = -1; _scrollOffset = 0; return; }

        if (FollowTail)
        {
            _ = SetSelectedIndex(_lines.Count - 1);
            EnsureSelectionVisible();
        }
        else if (_selectedIndex < 0)
        {
            _selectedIndex = 0;
        }
    }

    /// <summary>
    /// Clears rows and selection state.
    /// </summary>
    public void Clear()
    {
        _lines.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _scrollOffset = 0;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _lines.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.PageDown)) return SetSelectedIndex(_selectedIndex + Math.Max(1, _lastViewportRows - 1));
        if (key.Is(Key.PageUp)) return SetSelectedIndex(_selectedIndex - Math.Max(1, _lastViewportRows - 1));
        return key.Is(Key.Home) ? SetSelectedIndex(0)
            : key.Is(Key.End) && SetSelectedIndex(_lines.Count - 1);
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty) return Handle(message);

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedIndex(_selectedIndex + 1);
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedIndex(_selectedIndex - 1);
            return false;
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredIndex(-1) || Handle(message);
            }

            return Handle(message);
        }

        var rowIndex = _scrollOffset + (pointer.Y - content.Y);
        if (rowIndex < 0 || rowIndex >= _lines.Count)
        {
            return SetHoveredIndex(-1) || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion) return SetHoveredIndex(rowIndex);

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            return SetSelectedIndex(rowIndex);
        }

        return false;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var content = ResolveContentRect(Rect.Intersect(rect, canvas.Bounds));
        if (content.IsEmpty) return;

        if (_lines.Count == 0)
        {
            var style = IsDisabled ? EmptyStyle.Merge(DisabledStyle) : EmptyStyle;
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, style), content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible();
        var visibleRows = Math.Min(content.Height, _lines.Count - _scrollOffset);
        for (var row = 0; row < visibleRows; row++)
        {
            var lineIndex = _scrollOffset + row;
            var text = FormatLine(lineIndex, _lines[lineIndex]);
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(text, ResolveLineStyle(lineIndex)), content.Width);
        }
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 24;
        for (var index = 0; index < _lines.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatLine(index, _lines[index])) + Padding.Horizontal);
        }

        var height = Math.Max(1, Math.Min(12, _lines.Count));
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height + Padding.Vertical, 0, availableBounds.Height));
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return bounds.IsEmpty ? bounds : bounds.Inset(Padding);
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index) return false;

        _hoveredIndex = index;
        return true;
    }

    private bool SetSelectedIndex(int index)
    {
        if (_lines.Count == 0) return false;

        var clamped = Math.Clamp(index, 0, _lines.Count - 1);
        if (_selectedIndex == clamped) return false;

        var previousIndex = _selectedIndex;
        var previousLine = previousIndex >= 0 && previousIndex < _lines.Count ? _lines[previousIndex] : null;
        _selectedIndex = clamped;
        EnsureSelectionVisible();
        SelectionChanged?.Invoke(
            this,
            new ListSelectionChangedEventArgs<TerminalPanelLine>(
                previousIndex,
                _selectedIndex,
                previousLine,
                _lines[_selectedIndex]));
        return true;
    }

    private void EnsureSelectionVisible()
    {
        if (_selectedIndex < 0 || _lines.Count == 0) return;

        if (_selectedIndex < _scrollOffset) { _scrollOffset = _selectedIndex; return; }

        var viewport = Math.Max(1, _lastViewportRows);
        if (_selectedIndex >= _scrollOffset + viewport) _scrollOffset = _selectedIndex - viewport + 1;
    }

    private void TrimToMaxLines()
    {
        var overflow = _lines.Count - MaxLines;
        if (overflow <= 0) return;

        _lines.RemoveRange(0, overflow);
        if (_selectedIndex >= 0) _selectedIndex = Math.Max(-1, _selectedIndex - overflow);

        if (_hoveredIndex >= 0) _hoveredIndex = Math.Max(-1, _hoveredIndex - overflow);

        _scrollOffset = Math.Max(0, _scrollOffset - overflow);
    }

    private TeaStyle ResolveLineStyle(int lineIndex)
    {
        var style = ResolveChannelStyle(_lines[lineIndex].Channel);
        if (!MarkerStyle.IsEmpty) style = style.Merge(MarkerStyle);

        if (lineIndex == _hoveredIndex) style = style.Merge(HoveredLineStyle);

        if (lineIndex == _selectedIndex)
        {
            style = style.Merge(SelectedLineStyle);
            if (IsFocused) style = style.Merge(FocusedSelectedLineStyle);
        }

        if (IsDisabled) style = style.Merge(DisabledStyle);

        return style;
    }

    private TeaStyle ResolveChannelStyle(TerminalPanelChannel channel)
    {
        return channel switch
        {
            TerminalPanelChannel.StandardError => StandardErrorStyle,
            TerminalPanelChannel.Command => CommandStyle,
            TerminalPanelChannel.System => SystemStyle,
            _ => StandardOutputStyle,
        };
    }

    private string FormatLine(int lineIndex, TerminalPanelLine line)
    {
        var marker = ResolveMarker(line);
        return ShowLineNumbers
            ? $"{lineIndex + 1:D4} {marker} {line.Text}"
            : $"{marker} {line.Text}";
    }

    private static string ResolveMarker(TerminalPanelLine line)
    {
        if (!string.IsNullOrWhiteSpace(line.Marker)) return line.Marker!;

        return line.Channel switch
        {
            TerminalPanelChannel.StandardError => "ERR",
            TerminalPanelChannel.Command => "CMD",
            TerminalPanelChannel.System => "SYS",
            _ => "OUT",
        };
    }

    private static TerminalPanelLine CloneLine(TerminalPanelLine line) => new(line.Text, line.Channel, line.Marker);

    private static string ApplyStyle(string value, TeaStyle style) => style.IsEmpty ? value : style.Render(value);
}
