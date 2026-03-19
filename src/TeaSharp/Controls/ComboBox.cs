using TeaSharp.Controls.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a filterable single-selection control.
/// </summary>
/// <remarks>
/// Use this when the option list is too large for a simple choice control and users benefit from inline filtering.
/// </remarks>
public sealed class ComboBox : Control
{
    private readonly SelectionListState _options = new();
    private readonly TextInputModel _input = new();
    private bool _fieldHovered;

    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets the field title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "ComboBox";

    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    public bool ShowFocusMarker { get; set; } = true;

    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle ValueTextStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle PlaceholderTextStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle OptionStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle SelectedOptionStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle HoveredOptionStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle MutedStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the placeholder shown when no filter text is present.
    /// </summary>
    public string Placeholder
    {
        get => _input.Placeholder;
        set => _input.Placeholder = value ?? string.Empty;
    }

    /// <summary>
    /// Gets the active filter text.
    /// </summary>
    public string FilterText => _input.Value;

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public string SelectedItem => _options.SelectedItem;

    /// <summary>
    /// Gets a value indicating whether the drop-down list is open.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// Gets or sets the maximum number of visible items while the list is open.
    /// </summary>
    public int MaxVisibleItems
    {
        get;
        set;
    } = 6;

    /// <summary>
    /// Gets or sets the field border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets the inner padding applied to the field body.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    public override bool IsFocused
    {
        get;
        set;
    }

    public override bool IsDisabled
    {
        get;
        set;
    }

    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    /// Replaces the available selection items.
    /// </summary>
    /// <param name="items">The items to display.</param>
    public void SetItems(IEnumerable<string> items)
    {
        var previousIndex = _options.SelectedIndex;
        var previousItem = _options.SelectedItem;
        _options.SetItems(items, selectFirstItemWhenUnset: false);
        _options.ApplyFilter(_input.Value);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Replaces the current filter text.
    /// </summary>
    /// <param name="value">The filter text to apply.</param>
    public void SetFilterText(string value)
    {
        _input.SetValue(value ?? string.Empty);
        _options.ApplyFilter(_input.Value);
    }

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
                    changed |= SetHighlightedVisibleIndex(hoveredOption);
                    changed |= SelectHighlighted();
                    return changed || true;
                }

                break;
        }

        return changed || Handle(message);
    }

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
            Padding);
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
        var width = ControlTextLayout.MeasureDisplayWidth($"v {fieldText}") + Padding.Horizontal;
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
        var indicator = IsOpen ? "^" : "v";
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
        var start = OptionListViewport.ComputeWindowStart(_options.HighlightedVisibleIndex, visibleRows, _options.VisibleCount);
        var end = Math.Min(_options.VisibleCount, start + visibleRows);
        var row = 0;
        for (var visibleIndex = start; visibleIndex < end; visibleIndex++, row++)
        {
            var itemIndex = _options.VisibleItemIndexAt(visibleIndex);
            var highlight = visibleIndex == _options.HighlightedVisibleIndex ? ">" : " ";
            var selectedMarker = itemIndex == _options.SelectedIndex ? "*" : " ";
            var text = $"{highlight}{selectedMarker} {_options.Items[itemIndex]}";
            canvas.WriteText(content.X, content.Y + 1 + row, ApplyStyle(text, ResolveOptionStyle(itemIndex, visibleIndex)), content.Width);
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
            ? OptionListViewport.RowToVisibleIndex(content, y, MaxVisibleItems, _options.VisibleCount, _options.HighlightedVisibleIndex)
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
        if (previousIndex == _options.SelectedIndex && string.Equals(previousItem, _options.SelectedItem, StringComparison.Ordinal))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(previousIndex, _options.SelectedIndex, previousItem, _options.SelectedItem));
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && FocusMarker.Length > 0
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private TeaStyle ResolveFieldValueStyle(bool placeholderVisible)
    {
        var style = placeholderVisible ? PlaceholderTextStyle : ValueTextStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private TeaStyle ResolveOptionStyle(int itemIndex, int visibleIndex)
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

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
