using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a selectable command stream output viewer.
/// </summary>
public sealed partial class CommandOutput : Control
{
    private readonly List<CommandOutputLine> _lines = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>
    /// Occurs when selected line changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<CommandOutputLine>>? SelectionChanged;

    /// <summary>
    /// Gets or sets control title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Command Output";

    /// <summary>
    /// Gets or sets marker appended to title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether <see cref="FocusMarker" /> is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets title style when not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets title style when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border style when not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border style when focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style for standard output lines.
    /// </summary>
    public TesseraStyle StdOutStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style for standard error lines.
    /// </summary>
    public TesseraStyle StdErrStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style for system/meta lines.
    /// </summary>
    public TesseraStyle SystemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rows.
    /// </summary>
    public TesseraStyle HoveredLineStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TesseraStyle SelectedLineStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows while focused.
    /// </summary>
    public TesseraStyle FocusedSelectedLineStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rendered rows while disabled.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style used for timestamp text.
    /// </summary>
    public TesseraStyle TimestampStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style for empty-state text.
    /// </summary>
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether timestamps are rendered.
    /// </summary>
    public bool ShowTimestamp { get; set; } = true;

    /// <summary>
    /// Gets or sets timestamp format string.
    /// </summary>
    public string TimestampFormat { get; set; } = "HH:mm:ss";

    /// <summary>
    /// Gets or sets a value indicating whether appending auto-selects the latest row.
    /// </summary>
    public bool AutoFollow { get; set; } = true;

    /// <summary>
    /// Gets or sets maximum retained lines. Use <c>0</c> for unlimited.
    /// </summary>
    public int MaxLines { get; set; } = 2000;

    /// <summary>
    /// Gets or sets text rendered when there are no lines.
    /// </summary>
    public string EmptyText { get; set; } = "(no output)";

    /// <summary>
    /// Gets output lines.
    /// </summary>
    public IReadOnlyList<CommandOutputLine> Lines => _lines;

    /// <summary>
    /// Gets selected index, or <c>-1</c> when empty.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets selected line, if any.
    /// </summary>
    public CommandOutputLine? SelectedLine => _selectedIndex >= 0 && _selectedIndex < _lines.Count
        ? _lines[_selectedIndex]
        : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces all output lines.
    /// </summary>
    /// <param name="lines">Output lines.</param>
    public void SetLines(IEnumerable<CommandOutputLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        _lines.Clear();
        foreach (var line in lines)
        {
            if (line is not null)
            {
                _lines.Add(new CommandOutputLine(line.Text, line.Channel, line.Timestamp));
            }
        }

        if (_lines.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            _scrollOffset = 0;
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _lines.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _lines.Count - 1);
            _scrollOffset = Math.Clamp(_scrollOffset, 0, _lines.Count - 1);
        }
    }

    /// <summary>
    /// Appends one pre-built output line.
    /// </summary>
    /// <param name="line">Output line.</param>
    public void Append(CommandOutputLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(new CommandOutputLine(line.Text, line.Channel, line.Timestamp));
        TrimToMaxLines();
        if (AutoFollow)
        {
            _ = SetSelectedIndex(_lines.Count - 1);
            EnsureSelectionVisible(_lastViewportRows);
        }
        else if (_selectedIndex < 0 && _lines.Count > 0)
        {
            _selectedIndex = 0;
        }
    }

    /// <summary>
    /// Appends a standard output line.
    /// </summary>
    /// <param name="text">Line text.</param>
    /// <param name="timestamp">Optional timestamp.</param>
    public void AppendStdOut(string text, DateTimeOffset? timestamp = null)
    {
        Append(new CommandOutputLine(text, CommandOutputChannel.StdOut, timestamp ?? DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Appends a standard error line.
    /// </summary>
    /// <param name="text">Line text.</param>
    /// <param name="timestamp">Optional timestamp.</param>
    public void AppendStdErr(string text, DateTimeOffset? timestamp = null)
    {
        Append(new CommandOutputLine(text, CommandOutputChannel.StdErr, timestamp ?? DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Appends a system/meta line.
    /// </summary>
    /// <param name="text">Line text.</param>
    /// <param name="timestamp">Optional timestamp.</param>
    public void AppendSystem(string text, DateTimeOffset? timestamp = null)
    {
        Append(new CommandOutputLine(text, CommandOutputChannel.System, timestamp ?? DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Clears all lines.
    /// </summary>
    public void Clear()
    {
        _lines.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _scrollOffset = 0;
    }

    /// <summary>
    /// Sets selected row using bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed; otherwise <see langword="false"/>.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_lines.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _lines.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousLine = SelectedLine;
        _selectedIndex = clamped;
        SelectionChanged?.Invoke(this, new ListSelectionChangedEventArgs<CommandOutputLine>(previousIndex, _selectedIndex, previousLine, SelectedLine));
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _lines.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        var page = Math.Max(1, _lastViewportRows > 0 ? _lastViewportRows : 8);
        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.Home)) return SetSelectedIndex(0);
        if (key.Is(Key.End)) return SetSelectedIndex(_lines.Count - 1);
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

        if (pointer.Kind == PointerEventKind.Wheel && _lines.Count > 0)
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

        EnsureSelectionVisible(content.Height);
        var hovered = _scrollOffset + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _lines.Count)
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

    private void TrimToMaxLines()
    {
        if (MaxLines <= 0 || _lines.Count <= MaxLines)
        {
            return;
        }

        var remove = _lines.Count - MaxLines;
        _lines.RemoveRange(0, remove);
        _selectedIndex = _selectedIndex < 0 ? -1 : Math.Max(0, _selectedIndex - remove);
        _hoveredIndex = _hoveredIndex < 0 ? -1 : Math.Max(0, _hoveredIndex - remove);
        _scrollOffset = Math.Max(0, _scrollOffset - remove);
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
        if (_lines.Count == 0 || viewportRows <= 0)
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

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _lines.Count - viewportRows));
    }

}
