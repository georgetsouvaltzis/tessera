using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents an inspector-like key/value list with selectable rows.
/// </summary>
public sealed class KeyValueList : Control
{
    private readonly List<KeyValueListEntry> _entries = [];
    private int _selectedIndex = -1;
    private int _scrollOffset;

    /// <summary>
    /// Occurs when <see cref="SelectedIndex"/> changes.
    /// </summary>
    public event EventHandler<KeyValueListSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets list title.
    /// </summary>
    public string Title { get; set; } = "Key/Value";

    /// <summary>
    /// Gets or sets marker shown in title when focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    /// Gets or sets whether focused title marker is rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets title style when unfocused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets preferred key-column width.
    /// </summary>
    public int PreferredKeyColumnWidth { get; set; } = 20;

    /// <summary>
    /// Gets or sets separator text between key and value.
    /// </summary>
    public string Separator { get; set; } = ":";

    /// <summary>
    /// Gets or sets style for key text.
    /// </summary>
    public TeaStyle KeyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for value text.
    /// </summary>
    public TeaStyle ValueStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for separator text.
    /// </summary>
    public TeaStyle SeparatorStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into border glyphs while the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets current entries.
    /// </summary>
    public IReadOnlyList<KeyValueListEntry> Entries => _entries;

    /// <summary>
    /// Gets selected index or <c>-1</c> when empty.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets selected entry.
    /// </summary>
    public KeyValueListEntry? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _entries.Count
        ? _entries[_selectedIndex]
        : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces all entries.
    /// </summary>
    /// <param name="entries">Entries to render.</param>
    public void SetEntries(IEnumerable<KeyValueListEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;

        _entries.Clear();
        foreach (var entry in entries)
        {
            if (entry is not null)
            {
                _entries.Add(entry);
            }
        }

        _selectedIndex = _entries.Count == 0
            ? -1
            : Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _entries.Count - 1);
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Sets selected index with bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed; otherwise <see langword="false"/>.</returns>
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
        var previousItem = SelectedItem;
        _selectedIndex = clamped;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _entries.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_entries.Count - 1);
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty || !content.Contains(pointer.X, pointer.Y))
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1);
            }

            return false;
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left)
        {
            return Handle(message);
        }

        RequestFocus();
        var row = pointer.Y - content.Y;
        if (row < 0)
        {
            return true;
        }

        EnsureSelectionVisible(content.Height);
        var index = _scrollOffset + row;
        return index >= 0 && index < _entries.Count && SetSelectedIndex(index);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped, title, Border, Padding, ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        if (_entries.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, "(empty)", content.Width);
            return;
        }

        var keyWidth = ResolveKeyWidth(content.Width);
        EnsureSelectionVisible(content.Height);
        var rows = Math.Min(content.Height, _entries.Count - _scrollOffset);
        for (var row = 0; row < rows; row++)
        {
            var index = _scrollOffset + row;
            var entry = _entries[index];
            var selected = index == _selectedIndex;
            var keyStyle = selected ? KeyStyle.Merge(SelectedRowStyle) : KeyStyle;
            var valueStyle = selected ? ValueStyle.Merge(SelectedRowStyle) : ValueStyle;
            var separatorStyle = selected ? SeparatorStyle.Merge(SelectedRowStyle) : SeparatorStyle;
            var marker = selected ? ">" : " ";

            var key = ApplyStyle(PadRight(entry.Key, keyWidth), keyStyle);
            var separator = ApplyStyle(Separator, separatorStyle);
            var value = ApplyStyle(entry.Value, valueStyle);
            canvas.WriteText(content.X, content.Y + row, $"{marker} {key} {separator} {value}", content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var keyWidth = Math.Max(8, PreferredKeyColumnWidth);
        var width = 2 + keyWidth + 1 + ControlTextLayout.MeasureDisplayWidth(Separator) + 1 + 16 + Padding.Horizontal;
        var height = Math.Max(1, _entries.Count) + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            width = Math.Max(width, (Title?.Length ?? 0) + 4);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private int ResolveKeyWidth(int contentWidth)
    {
        var maxWidth = Math.Max(8, contentWidth - 6 - ControlTextLayout.MeasureDisplayWidth(Separator));
        return Math.Clamp(Math.Max(8, PreferredKeyColumnWidth), 8, maxWidth);
    }

    private string RenderTitle()
    {
        var text = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(text ?? string.Empty, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private void EnsureSelectionVisible(int viewportHeight)
    {
        if (viewportHeight <= 0 || _entries.Count == 0 || _selectedIndex < 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
        }
        else if (_selectedIndex >= _scrollOffset + viewportHeight)
        {
            _scrollOffset = _selectedIndex - viewportHeight + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _entries.Count - viewportHeight));
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, KeyValueListEntry? previousItem)
    {
        if (previousIndex == _selectedIndex && ReferenceEquals(previousItem, SelectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new KeyValueListSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, SelectedItem));
    }

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }

    private static string PadRight(string text, int width)
    {
        var safe = text ?? string.Empty;
        var measured = ControlTextLayout.MeasureDisplayWidth(safe);
        return measured >= width
            ? safe
            : safe + new string(' ', width - measured);
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }
}
