using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a scrollable single-selection list.
/// </summary>
/// <typeparam name="T">The item type shown by the list.</typeparam>
public sealed class ListView<T> : Control
{
    private readonly ListModel<T> _model;
    private int? _hoveredFilteredIndex;

    /// <summary>
    /// Initializes a new list view.
    /// </summary>
    /// <param name="textSelector">Optional item-to-text projection.</param>
    public ListView(Func<T, string>? textSelector = null)
    {
        _model = new ListModel<T>(Array.Empty<T>(), textSelector ?? DefaultText);
    }

    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<T>>? SelectionChanged;

    /// <summary>
    /// Gets or sets the list title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "List";

    /// <summary>
    /// Gets or sets the list border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets the inner padding applied to the list body.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets how many items fit in a page-sized view.
    /// </summary>
    public int PageSize
    {
        get => _model.PageSize;
        set => _model.PageSize = value;
    }

    /// <summary>
    /// Gets the number of currently visible items after filtering.
    /// </summary>
    public int Count => _model.Count;

    /// <summary>
    /// Gets the current selected index.
    /// </summary>
    public int SelectedIndex => _model.SelectedIndex;

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public T? SelectedItem => _model.SelectedItem;

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
    /// Replaces the items shown by the list.
    /// </summary>
    /// <param name="items">The items to display.</param>
    public void SetItems(IEnumerable<T> items)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _model.SetItems(items);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Applies a filter string to the list items.
    /// </summary>
    /// <param name="filter">The filter string.</param>
    public void SetFilter(string filter)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _model.SetFilter(filter ?? string.Empty);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    public override bool Handle(Message message)
    {
        if (IsDisabled)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = _model.Update(message);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return changed;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        if (!content.Contains(pointer.X, pointer.Y) && pointer.Kind != PointerEventKind.Wheel)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredFilteredIndex(null);
            }

            return false;
        }

        _model.PageSize = Math.Max(1, content.Height);
        var hoverChanged = pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press
            ? SetHoveredByPointer(pointer.X, pointer.Y, content)
            : false;

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return hoverChanged;
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            return hoverChanged | Handle(message);
        }

        if (pointer is not { Kind: PointerEventKind.Press, Button: PointerButton.Left } click || !content.Contains(click.X, click.Y))
        {
            return false;
        }

        var row = click.Y - content.Y;
        if (row < 0 || row >= content.Height)
        {
            return false;
        }

        var visibleRows = _model.VisibleRows();
        if (row >= visibleRows.Count)
        {
            return false;
        }

        if (!IsPointerWithinRowLabel(click.X, content.X, content.Width, _model.LabelFor(visibleRows[row].Item)))
        {
            return hoverChanged;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = _model.SelectFilteredIndex(visibleRows[row].Index);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return hoverChanged | changed;
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

        _model.PageSize = Math.Max(1, content.Height);
        var rows = _model.VisibleRows();
        if (rows.Count == 0 && content.Height > 0)
        {
            canvas.WriteText(content.X, content.Y, "(empty)", content.Width);
            return;
        }

        for (var row = 0; row < rows.Count && row < content.Height; row++)
        {
            var visible = rows[row];
            var marker = visible.Selected
                ? "›"
                : _hoveredFilteredIndex == visible.Index ? "▸" : " ";
            var text = $"{marker} {_model.LabelFor(visible.Item)}";
            canvas.WriteText(content.X, content.Y + row, text, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var rows = _model.VisibleRows();
        var width = 0;
        for (var index = 0; index < rows.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(_model.LabelFor(rows[index].Item)) + 2);
        }

        width += Padding.Horizontal;
        var height = Math.Max(1, rows.Count) + Padding.Vertical;
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

    private void RaiseSelectionChangedIfNeeded(int previousIndex, T? previousItem)
    {
        if (previousIndex == SelectedIndex
            && EqualityComparer<T?>.Default.Equals(previousItem, SelectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new ListSelectionChangedEventArgs<T>(previousIndex, SelectedIndex, previousItem, SelectedItem));
    }

    private bool SetHoveredByPointer(int x, int y, Rect content)
    {
        if (!content.Contains(x, y))
        {
            return SetHoveredFilteredIndex(null);
        }

        var row = y - content.Y;
        if (row < 0 || row >= content.Height)
        {
            return SetHoveredFilteredIndex(null);
        }

        var rows = _model.VisibleRows();
        if (row >= rows.Count)
        {
            return SetHoveredFilteredIndex(null);
        }

        if (!IsPointerWithinRowLabel(x, content.X, content.Width, _model.LabelFor(rows[row].Item)))
        {
            return SetHoveredFilteredIndex(null);
        }

        return SetHoveredFilteredIndex(rows[row].Index);
    }

    private bool SetHoveredFilteredIndex(int? filteredIndex)
    {
        if (_hoveredFilteredIndex == filteredIndex)
        {
            return false;
        }

        _hoveredFilteredIndex = filteredIndex;
        return true;
    }

    private static bool IsPointerWithinRowLabel(int pointerX, int contentX, int contentWidth, string label)
    {
        var hitWidth = Math.Min(contentWidth, 2 + ControlTextLayout.MeasureDisplayWidth(label));
        return pointerX >= contentX && pointerX < contentX + hitWidth;
    }

    private static string DefaultText(T item) => item?.ToString() ?? string.Empty;
}
