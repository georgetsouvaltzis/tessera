using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;
using Tessera.Widgets;

namespace Tessera.Controls;

/// <summary>
///     Represents a filterable single-selection control.
/// </summary>
/// <remarks>
///     Use this when the option list is too large for a simple choice control and users benefit from inline filtering.
/// </remarks>
public sealed class ComboBox : Control
{
    private readonly TextInputModel _input = new();
    private readonly SelectionListState _options = new();
    private bool _fieldHovered;

    /// <summary>
    ///     Gets or sets the field title.
    /// </summary>
    public string Title { get; set; } = "ComboBox";

    /// <summary>
    ///     Represents focus marker.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether show focus marker.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets the title style.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the value text style.
    /// </summary>
    public TesseraStyle ValueTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the placeholder text style.
    /// </summary>
    public TesseraStyle PlaceholderTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into the field value when the pointer hovers the field row.
    /// </summary>
    public TesseraStyle HoveredValueStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the option style.
    /// </summary>
    public TesseraStyle OptionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the selected option style.
    /// </summary>
    public TesseraStyle SelectedOptionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the hovered option style.
    /// </summary>
    public TesseraStyle HoveredOptionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the muted style.
    /// </summary>
    public TesseraStyle MutedStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the disabled style.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets glyphs used to render field indicators and option markers.
    /// </summary>
    public DropdownGlyphSet Glyphs { get; set; } = DropdownGlyphSet.Default;

    /// <summary>
    ///     Gets or sets the placeholder shown when no filter text is present.
    /// </summary>
    public string Placeholder
    {
        get => _input.Placeholder;
        set => _input.Placeholder = value;
    }

    /// <summary>
    ///     Gets the active filter text.
    /// </summary>
    public string FilterText => _input.Value;

    /// <summary>
    ///     Gets the currently selected item.
    /// </summary>
    public string SelectedItem => _options.SelectedItem;

    /// <summary>
    ///     Gets a value indicating whether the drop-down list is open.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    ///     Gets or sets the maximum number of visible items while the list is open.
    /// </summary>
    public int MaxVisibleItems
    {
        get;
        set;
    } = 6;

    /// <summary>
    ///     Gets or sets the field border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    ///     Gets or sets the inner padding applied to the field body.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsDisabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    ///     Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    ///     Replaces the available selection items.
    /// </summary>
    /// <param name="items">The items to display.</param>
    public void SetItems(IEnumerable<string> items)
    {
        var previousIndex = _options.SelectedIndex;
        var previousItem = _options.SelectedItem;
        _options.SetItems(items, false);
        _options.ApplyFilter(_input.Value);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    ///     Replaces the current filter text.
    /// </summary>
    /// <param name="value">The filter text to apply.</param>
    public void SetFilterText(string value)
    {
        _input.SetValue(value);
        _options.ApplyFilter(_input.Value);
    }

    /// <summary>
    ///     Sets the selected item index using bounds clamping.
    /// </summary>
    /// <param name="index">The requested index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_options.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _options.Count - 1);
        var previousIndex = _options.SelectedIndex;
        var previousItem = _options.SelectedItem;
        var changed = _options.SetSelectedIndex(clamped);
        if (!changed)
        {
            return false;
        }

        _input.SetValue(_options.Items[clamped]);
        _options.ApplyFilter(_input.Value);
        _options.AlignHighlightToSelectionOrStart();
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    /// <summary>
    ///     Attempts to select the first item matching <paramref name="item" /> using ordinal comparison.
    /// </summary>
    /// <param name="item">The item value to select.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool TrySetSelectedItem(string item)
    {
        if (item is null)
        {
            return false;
        }

        for (var index = 0; index < _options.Items.Count; index++)
        {
            if (string.Equals(_options.Items[index], item, StringComparison.Ordinal))
            {
                return SetSelectedIndex(index);
            }
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly)
        {
            return false;
        }

        if (message is KeyPressed key)
        {
            if (IsOpen && key.Is(Key.Escape))
            {
                IsOpen = false;
                return true;
            }

            if (IsOpen && (key.Is(Key.Down) || key.IsCharacter('j')) && _options.VisibleCount > 0)
            {
                _options.MoveNextVisible();
                return true;
            }

            if (IsOpen && (key.Is(Key.Up) || key.IsCharacter('k')) && _options.VisibleCount > 0)
            {
                _options.MovePreviousVisible();
                return true;
            }

            if (IsOpen && key.Is(Key.Enter))
            {
                return SelectHighlighted();
            }

            if (!IsOpen && key.Is(Key.Down))
            {
                IsOpen = true;
                _options.AlignHighlightToSelectionOrStart();
                return true;
            }
        }

        var inputResult = _input.Update(message);
        if (inputResult.Changed)
        {
            _options.ApplyFilter(_input.Value);
            IsOpen = true;
            return true;
        }

        if (inputResult.Submitted && IsOpen && _options.VisibleCount > 0)
        {
            return SelectHighlighted();
        }

        return inputResult.Submitted;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
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

        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetFieldHovered(false);
                changed |= _options.SetHoveredVisibleIndex(-1);
            }

            if (pointer.Kind is not PointerEventKind.Wheel)
            {
                return changed || Handle(message);
            }
        }

        if (pointer.Kind == PointerEventKind.Wheel && IsOpen && _options.VisibleCount > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                _options.MoveNextVisible();
                return true;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                _options.MovePreviousVisible();
                return true;
            }
        }

        if (!inside)
        {
            return changed || Handle(message);
        }

        var hoveredField = pointer.Y == content.Y;
        var hoveredOption = RowToVisibleIndex(content, pointer.Y);
        switch (pointer.Kind)
        {
            case PointerEventKind.Motion:
                changed |= SetFieldHovered(hoveredField);
                changed |= _options.SetHoveredVisibleIndex(hoveredOption);
                if (hoveredOption >= 0)
                {
                    changed |= SetHighlightedVisibleIndex(hoveredOption);
                }

                return changed;
            case PointerEventKind.Press when pointer.Button == PointerButton.Left:
                if (hoveredField)
                {
                    SetFieldHovered(true);
                    if (!IsOpen)
                    {
                        IsOpen = true;
                        _options.AlignHighlightToSelectionOrStart();
                    }
                    else
                    {
                        IsOpen = false;
                    }

                    return true;
                }

                if (IsOpen && hoveredOption >= 0)
                {
                    _ = SetHighlightedVisibleIndex(hoveredOption);
                    _ = SelectHighlighted();
                    return true;
                }

                break;
        }

        return changed || Handle(message);
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
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        RenderField(canvas, content);
        RenderOpenOptions(canvas, content);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var fieldText = _input.BuildFrame(Math.Max(1, availableBounds.Width)).Text;
        var width = ControlTextLayout.MeasureDisplayWidth($"{Glyphs.CollapsedIndicator} {fieldText}") +
                    Padding.Horizontal;
        var height = Padding.Vertical + 1;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            width = Math.Max(width, Title.Length + 4);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderField(Canvas canvas, Rect content)
    {
        var frameWidth = Math.Max(1, content.Width - 2);
        var frame = _input.BuildFrame(frameWidth);
        var indicator = IsOpen ? Glyphs.ExpandedIndicator : Glyphs.CollapsedIndicator;
        var valueStyle = ResolveFieldValueStyle(frame.PlaceholderVisible);
        var text = $"{ApplyStyle(indicator, valueStyle)} {ApplyStyle(frame.Text, valueStyle)}";
        canvas.WriteText(content.X, content.Y, text, content.Width);
    }

    private void RenderOpenOptions(Canvas canvas, Rect content)
    {
        if (!IsOpen || content.Height <= 1)
        {
            return;
        }

        if (_options.VisibleCount == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, ApplyStyle("(no matches)", MutedStyle), content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = OptionListViewport.ComputeWindowStart(_options.HighlightedVisibleIndex, visibleRows,
            _options.VisibleCount);
        var end = Math.Min(_options.VisibleCount, start + visibleRows);
        var row = 0;
        for (var visibleIndex = start; visibleIndex < end; visibleIndex++, row++)
        {
            var itemIndex = _options.VisibleItemIndexAt(visibleIndex);
            var highlight = visibleIndex == _options.HighlightedVisibleIndex ? Glyphs.HighlightedOptionMarker : " ";
            var selectedMarker = itemIndex == _options.SelectedIndex ? Glyphs.SelectedOptionMarker : " ";
            var text = $"{highlight}{selectedMarker} {_options.Items[itemIndex]}";
            canvas.WriteText(content.X, content.Y + 1 + row,
                ApplyStyle(text, ResolveOptionStyle(itemIndex, visibleIndex)), content.Width);
        }
    }

    private bool SelectHighlighted()
    {
        var previousIndex = _options.SelectedIndex;
        var previousItem = _options.SelectedItem;
        if (!_options.TrySelectHighlighted(out var selectedIndex))
        {
            IsOpen = false;
            return true;
        }

        _input.SetValue(_options.Items[selectedIndex]);
        _options.ApplyFilter(_input.Value);
        IsOpen = false;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    private int RowToVisibleIndex(Rect content, int y)
    {
        return IsOpen
            ? OptionListViewport.RowToVisibleIndex(content, y, MaxVisibleItems, _options.VisibleCount,
                _options.HighlightedVisibleIndex)
            : -1;
    }

    private bool SetFieldHovered(bool hovered)
    {
        if (_fieldHovered == hovered)
        {
            return false;
        }

        _fieldHovered = hovered;
        return true;
    }

    private bool SetHighlightedVisibleIndex(int index)
    {
        if (index < 0 || index >= _options.VisibleCount || index == _options.HighlightedVisibleIndex)
        {
            return false;
        }

        while (_options.HighlightedVisibleIndex != index)
        {
            if (_options.HighlightedVisibleIndex < index)
            {
                _options.MoveNextVisible();
            }
            else
            {
                _options.MovePreviousVisible();
            }
        }

        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, string previousItem)
    {
        if (previousIndex == _options.SelectedIndex &&
            string.Equals(previousItem, _options.SelectedItem, StringComparison.Ordinal))
        {
            return;
        }

        SelectionChanged?.Invoke(this,
            new SelectionChangedEventArgs(previousIndex, _options.SelectedIndex, previousItem, _options.SelectedItem));
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && FocusMarker.Length > 0
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private TesseraStyle ResolveFieldValueStyle(bool placeholderVisible)
    {
        var style = placeholderVisible ? PlaceholderTextStyle : ValueTextStyle;
        if (_fieldHovered)
        {
            style = style.Merge(HoveredValueStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private TesseraStyle ResolveOptionStyle(int itemIndex, int visibleIndex)
    {
        var style = OptionStyle;
        if (itemIndex == _options.SelectedIndex)
        {
            style = style.Merge(SelectedOptionStyle);
        }

        if (visibleIndex == _options.HighlightedVisibleIndex)
        {
            style = style.Merge(HoveredOptionStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
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
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
