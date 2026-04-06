using System.ComponentModel;
using Tessera.Internal;
using Tessera.Widgets.Internal;

namespace Tessera.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal readonly record struct ListRow<T>(T Item, int Index, bool Selected);

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ListModel<T>
{
    private readonly Func<T, string> _toText;
    private readonly List<T> _allItems = [];
    private readonly List<int> _filteredIndexes = [];
    private readonly ListModelLoadCoordinator _loadCoordinator = new();
    private int _offset;

    public ListModel(IEnumerable<T> items, Func<T, string> toText)
    {
        _toText = toText;
        SetItems(items);
    }

    public int SelectedIndex { get; private set; } = -1;

    public int PageSize { get; set; } = 8;

    public string Filter { get; private set; } = string.Empty;

    public StringComparison FilterComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    public Comparison<T>? SortComparison { get; set; }

    public bool HasSelection => SelectedIndex >= 0 && SelectedIndex < _filteredIndexes.Count;

    public T? SelectedItem => HasSelection ? _allItems[_filteredIndexes[SelectedIndex]] : default;

    public int Count => _filteredIndexes.Count;

    public int ViewOffset => _offset;

    public void SetItems(IEnumerable<T> items)
    {
        _allItems.Clear();
        _allItems.AddRange(items);
        ApplyFilter();
    }

    public async ValueTask SetItemsAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        _allItems.Clear();
        await ListModelAsyncLoader.AppendItemsAsync(_allItems, items, cancellationToken).ConfigureAwait(false);
        ApplyFilter();
    }

    public async ValueTask<int> AppendItemsAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        var before = _allItems.Count;
        await ListModelAsyncLoader.AppendItemsAsync(_allItems, items, cancellationToken).ConfigureAwait(false);
        ApplyFilter();
        return _allItems.Count - before;
    }

    public async ValueTask<int> ReloadAsync(Func<CancellationToken, IAsyncEnumerable<T>> loader, CancellationToken cancellationToken = default)
    {
        var (version, linkedToken, disposer) = _loadCoordinator.Begin(cancellationToken);
        try
        {
            var loaded = await ListModelAsyncLoader.MaterializeAsync(loader(linkedToken), linkedToken).ConfigureAwait(false);
            if (!_loadCoordinator.IsCurrent(version))
            {
                return 0;
            }

            SetItems(loaded);
            return _allItems.Count;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && !_loadCoordinator.IsCurrent(version))
        {
            return 0;
        }
        finally
        {
            disposer();
        }
    }

    public async ValueTask<int> AppendAsync(Func<CancellationToken, IAsyncEnumerable<T>> loader, CancellationToken cancellationToken = default)
    {
        var (version, linkedToken, disposer) = _loadCoordinator.Begin(cancellationToken);
        try
        {
            var loaded = await ListModelAsyncLoader.MaterializeAsync(loader(linkedToken), linkedToken).ConfigureAwait(false);
            if (!_loadCoordinator.IsCurrent(version))
            {
                return 0;
            }

            _allItems.AddRange(loaded);
            ApplyFilter();
            return loaded.Count;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && !_loadCoordinator.IsCurrent(version))
        {
            return 0;
        }
        finally
        {
            disposer();
        }
    }

    public void SetFilter(string filter)
    {
        Filter = filter ?? string.Empty;
        ApplyFilter();
    }

    public bool Update(global::Tessera.Core.Abstractions.IMessage message, ListKeyMap? keyMap = null)
    {
        return Update(TesseraMessageAdapter.ToPublic(message), keyMap);
    }

    public bool Update(Message message, ListKeyMap? keyMap = null)
    {
        keyMap ??= ListKeyMap.Default;
        var beforeSelection = SelectedIndex;
        var beforeOffset = _offset;

        if (message is KeyPressed key)
        {
            if (keyMap.Up.Matches(key)) MoveSelection(-1);
            else if (keyMap.Down.Matches(key)) MoveSelection(1);
            else if (keyMap.PageUp.Matches(key)) MoveSelection(-Math.Max(1, PageSize));
            else if (keyMap.PageDown.Matches(key)) MoveSelection(Math.Max(1, PageSize));
            else if (keyMap.Home.Matches(key)) Select(0);
            else if (keyMap.End.Matches(key)) Select(Math.Max(0, _filteredIndexes.Count - 1));
        }
        else if (message is PointerInput { Kind: PointerEventKind.Wheel } wheel)
        {
            if (wheel.Button == PointerButton.WheelUp) MoveSelection(-1);
            else if (wheel.Button == PointerButton.WheelDown) MoveSelection(1);
        }

        return beforeSelection != SelectedIndex || beforeOffset != _offset;
    }

    public IReadOnlyList<ListRow<T>> VisibleRows()
    {
        return ListModelWindowing.VisibleRows(_allItems, _filteredIndexes, _offset, PageSize, SelectedIndex);
    }

    public string LabelFor(T item) => _toText(item);

    public bool SelectFilteredIndex(int filteredIndex)
    {
        if (_filteredIndexes.Count == 0)
        {
            return false;
        }

        var beforeSelection = SelectedIndex;
        var beforeOffset = _offset;
        Select(filteredIndex);
        return beforeSelection != SelectedIndex || beforeOffset != _offset;
    }

    private void MoveSelection(int delta)
    {
        if (_filteredIndexes.Count == 0)
        {
            SelectedIndex = -1;
            _offset = 0;
            return;
        }

        Select(SelectedIndex < 0 ? 0 : SelectedIndex + delta);
    }

    private void Select(int index)
    {
        if (_filteredIndexes.Count == 0)
        {
            SelectedIndex = -1;
            _offset = 0;
            return;
        }

        SelectedIndex = Math.Clamp(index, 0, _filteredIndexes.Count - 1);
        ListModelWindowing.EnsureSelectionVisible(SelectedIndex, _filteredIndexes.Count, PageSize, ref _offset);
    }

    private void ApplyFilter()
    {
        ListModelFilter.Apply(_allItems, _toText, Filter, FilterComparison, SortComparison, _filteredIndexes);
        if (_filteredIndexes.Count == 0)
        {
            SelectedIndex = -1;
            _offset = 0;
            return;
        }

        if (SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }

        SelectedIndex = Math.Clamp(SelectedIndex, 0, _filteredIndexes.Count - 1);
        ListModelWindowing.EnsureSelectionVisible(SelectedIndex, _filteredIndexes.Count, PageSize, ref _offset);
    }

}
