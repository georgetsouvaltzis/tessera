using TeaSharp.Controls.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a single-choice selector.
/// </summary>
public sealed class Choice : Control
{
    private readonly SelectionListState _options = new();
    private bool _fieldHovered;

    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets the selector title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Choice";

    /// <summary>
    /// Gets or sets the field border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets the inner padding applied to the selector.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the maximum number of visible items while open.
    /// </summary>
    public int MaxVisibleItems
    {
        get;
        set;
    } = 6;

    /// <summary>
    /// Gets a value indicating whether the selector is currently open.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// Gets the current selected index.
    /// </summary>
    public int SelectedIndex => _options.SelectedIndex;

    /// <summary>
    /// Gets the current selected item.
    /// </summary>
    public string SelectedItem => _options.SelectedItem;

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
    /// Replaces the available choice items.
    /// </summary>
    /// <param name="items">The items to display.</param>
    public void SetItems(IEnumerable<string> items)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _options.SetItems(items, selectFirstItemWhenUnset: true);
        _fieldHovered = false;
        if (_options.Count == 0)
        {
            IsOpen = false;
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || _options.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (!IsOpen)
        {
            if (key.Is(Key.Enter) || key.Is(Key.Down) || key.IsCharacter(' '))
            {
                IsOpen = true;
                _options.AlignHighlightToSelectionOrStart();
                return true;
            }

            return false;
        }

        if (key.Is(Key.Escape))
        {
            IsOpen = false;
            return true;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            _options.MoveNextVisible();
            return true;
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            _options.MovePreviousVisible();
            return true;
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            var changed = SelectHighlighted();
            IsOpen = false;
            return changed || true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || _options.Count == 0 || message is not PointerInput pointer)
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

        if (pointer.Kind == PointerEventKind.Wheel && IsOpen)
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
        var hoveredOptionIndex = RowToVisibleIndex(content, pointer.Y);
        switch (pointer.Kind)
        {
            case PointerEventKind.Motion:
                changed |= SetFieldHovered(hoveredField);
                changed |= _options.SetHoveredVisibleIndex(hoveredOptionIndex);
                if (hoveredOptionIndex >= 0)
                {
                    changed |= SetHighlightedVisibleIndex(hoveredOptionIndex);
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

                if (IsOpen && hoveredOptionIndex >= 0)
                {
                    changed |= SelectVisible(hoveredOptionIndex);
                    IsOpen = false;
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
            Border == BorderStyle.None ? null : IsFocused ? $"{Title} *" : Title,
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
        var selected = _options.Count == 0 ? "(empty)" : SelectedItem;
        var width = ControlTextLayout.MeasureDisplayWidth($"v {selected}") + Padding.Horizontal;
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
        var indicator = IsOpen ? "^" : "v";
        var selected = _options.Count == 0 ? "(empty)" : SelectedItem;
        canvas.WriteText(content.X, content.Y, $"{indicator} {selected}", content.Width);
    }

    private void RenderOpenOptions(Canvas canvas, Rect content)
    {
        if (!IsOpen || content.Height <= 1 || _options.VisibleCount == 0)
        {
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
            canvas.WriteText(content.X, content.Y + 1 + row, text, content.Width);
        }
    }

    private int RowToVisibleIndex(Rect content, int y)
    {
        return IsOpen
            ? OptionListViewport.RowToVisibleIndex(content, y, MaxVisibleItems, _options.VisibleCount, _options.HighlightedVisibleIndex)
            : -1;
    }

    private bool SelectVisible(int visibleIndex)
    {
        if (visibleIndex < 0 || visibleIndex >= _options.VisibleCount)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = _options.SetSelectedIndex(_options.VisibleItemIndexAt(visibleIndex));
        if (changed)
        {
            RaiseSelectionChanged(previousIndex, previousItem);
        }

        return changed;
    }

    private bool SelectHighlighted()
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = _options.TrySelectHighlighted(out _);
        if (changed)
        {
            RaiseSelectionChanged(previousIndex, previousItem);
        }

        return changed;
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

    private bool SetFieldHovered(bool hovered)
    {
        if (_fieldHovered == hovered)
        {
            return false;
        }

        _fieldHovered = hovered;
        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, string previousItem)
    {
        if (previousIndex == SelectedIndex && string.Equals(previousItem, SelectedItem, StringComparison.Ordinal))
        {
            return;
        }

        RaiseSelectionChanged(previousIndex, previousItem);
    }

    private void RaiseSelectionChanged(int previousIndex, string previousItem)
    {
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
    }
}
