using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class ListComponent<T> : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private int? _hoveredFilteredIndex;
    private readonly ListModel<T> _model;

    public ListComponent(IEnumerable<T> items, Func<T, string> toText)
    {
        _model = new ListModel<T>(items, toText);
    }

    public ListComponent(ListOptions<T> options)
        : this(options.Items, options.ToText)
    {
        Title = options.Title;
        IsFocused = options.IsFocused;
        IsDisabled = options.IsDisabled;
        IsReadOnly = options.IsReadOnly;
        Border = options.Border;
        Padding = options.Padding;
        KeyMap = options.KeyMap ?? ListKeyMap.Default;
    }

    public string Title { get; set; } = "List";

    public bool IsFocused { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsReadOnly { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public int SelectedIndex => _model.SelectedIndex;

    public bool HasSelection => _model.HasSelection;

    public T? SelectedItem => _model.SelectedItem;

    public int Count => _model.Count;

    public int ViewOffset => _model.ViewOffset;

    public int PageSize
    {
        get => _model.PageSize;
        set => _model.PageSize = value;
    }

    public string Filter => _model.Filter;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public Func<T, IReadOnlyCollection<WidgetVisualState>?>? ItemStateResolver { get; set; }

    /// <summary>
    /// Raised when the list selection changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<T>>? SelectionChanged;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ListKeyMap KeyMap { get; set; } = ListKeyMap.Default;

    public void SetItems(IEnumerable<T> items)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _model.SetItems(items);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    public async ValueTask SetItemsAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        await _model.SetItemsAsync(items, cancellationToken);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    public async ValueTask<int> AppendItemsAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var appended = await _model.AppendItemsAsync(items, cancellationToken);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return appended;
    }

    public async ValueTask<int> ReloadAsync(Func<CancellationToken, IAsyncEnumerable<T>> loader, CancellationToken cancellationToken = default)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var loaded = await _model.ReloadAsync(loader, cancellationToken);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return loaded;
    }

    public async ValueTask<int> AppendAsync(Func<CancellationToken, IAsyncEnumerable<T>> loader, CancellationToken cancellationToken = default)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var appended = await _model.AppendAsync(loader, cancellationToken);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return appended;
    }

    public void SetFilter(string filter)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _model.SetFilter(filter);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    public IReadOnlyList<ListRow<T>> VisibleRows()
    {
        return _model.VisibleRows();
    }

    public string LabelFor(T item) => _model.LabelFor(item);

    public bool Update(IMessage message)
    {
        if (IsDisabled)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = _model.Update(message, KeyMap);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return changed;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (IsDisabled)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = ListComponentMouseRouter.Update(message, bounds, Border, Padding, _model, SetHoveredFilteredIndex);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        ListComponentRenderer.Render(
            canvas,
            rect,
            _model,
            Title,
            IsFocused,
            IsDisabled,
            IsReadOnly,
            Border,
            Padding,
            _hoveredFilteredIndex,
            ItemStatePalette,
            ItemStateResolver);
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

    private void RaiseSelectionChangedIfNeeded(int previousIndex, T? previousItem)
    {
        if (previousIndex == SelectedIndex
            && EqualityComparer<T?>.Default.Equals(previousItem, SelectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new ListSelectionChangedEventArgs<T>(previousIndex, SelectedIndex, previousItem, SelectedItem));
    }
}
