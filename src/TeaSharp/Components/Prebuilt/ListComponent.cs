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
        Focused = options.Focused;
        Disabled = options.Disabled;
        ReadOnly = options.ReadOnly;
        ShowBorder = options.ShowBorder;
        KeyMap = options.KeyMap ?? ListKeyMap.Default;
    }

    public string Title { get; set; } = "List";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

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

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ListKeyMap KeyMap { get; set; } = ListKeyMap.Default;

    public void SetItems(IEnumerable<T> items)
    {
        _model.SetItems(items);
    }

    public ValueTask SetItemsAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        return _model.SetItemsAsync(items, cancellationToken);
    }

    public ValueTask<int> AppendItemsAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        return _model.AppendItemsAsync(items, cancellationToken);
    }

    public ValueTask<int> ReloadAsync(Func<CancellationToken, IAsyncEnumerable<T>> loader, CancellationToken cancellationToken = default)
    {
        return _model.ReloadAsync(loader, cancellationToken);
    }

    public ValueTask<int> AppendAsync(Func<CancellationToken, IAsyncEnumerable<T>> loader, CancellationToken cancellationToken = default)
    {
        return _model.AppendAsync(loader, cancellationToken);
    }

    public void SetFilter(string filter)
    {
        _model.SetFilter(filter);
    }

    public IReadOnlyList<ListRow<T>> VisibleRows()
    {
        return _model.VisibleRows();
    }

    public string LabelFor(T item) => _model.LabelFor(item);

    public bool Update(IMessage message)
    {
        if (Disabled)
        {
            return false;
        }

        return _model.Update(message, KeyMap);
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled)
        {
            return false;
        }
        return ListComponentMouseRouter.Update(message, bounds, ShowBorder, _model, SetHoveredFilteredIndex);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        ListComponentRenderer.Render(
            canvas,
            rect,
            _model,
            Title,
            Focused,
            Disabled,
            ReadOnly,
            ShowBorder,
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
}
