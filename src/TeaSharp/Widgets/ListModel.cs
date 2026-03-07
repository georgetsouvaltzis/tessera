using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Widgets;

public readonly record struct ListRow<T>(T Item, int Index, bool Selected);

public sealed class ListModel<T>
{
    private readonly Func<T, string> _toText;
    private readonly List<T> _allItems = [];
    private readonly List<int> _filteredIndexes = [];
    private readonly object _loadGate = new();
    private int _offset;
    private int _loadVersion;
    private CancellationTokenSource? _activeLoadCts;

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

    public T? SelectedItem => HasSelection
        ? _allItems[_filteredIndexes[SelectedIndex]]
        : default;

    public int Count => _filteredIndexes.Count;

    public void SetItems(IEnumerable<T> items)
    {
        _allItems.Clear();
        _allItems.AddRange(items);
        ApplyFilter();
    }

    public async ValueTask SetItemsAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        _allItems.Clear();
        await AppendItemsCoreAsync(items, cancellationToken).ConfigureAwait(false);
        ApplyFilter();
    }

    public async ValueTask<int> AppendItemsAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        var before = _allItems.Count;
        await AppendItemsCoreAsync(items, cancellationToken).ConfigureAwait(false);
        ApplyFilter();
        return _allItems.Count - before;
    }

    public async ValueTask<int> ReloadAsync(
        Func<CancellationToken, IAsyncEnumerable<T>> loader,
        CancellationToken cancellationToken = default)
    {
        var (version, linkedToken, disposer) = BeginTrackedLoad(cancellationToken);
        try
        {
            var loaded = await MaterializeAsync(loader(linkedToken), linkedToken).ConfigureAwait(false);
            if (!IsCurrentLoad(version))
            {
                return 0;
            }

            SetItems(loaded);
            return _allItems.Count;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && !IsCurrentLoad(version))
        {
            return 0;
        }
        finally
        {
            disposer();
        }
    }

    public async ValueTask<int> AppendAsync(
        Func<CancellationToken, IAsyncEnumerable<T>> loader,
        CancellationToken cancellationToken = default)
    {
        var (version, linkedToken, disposer) = BeginTrackedLoad(cancellationToken);
        try
        {
            var loaded = await MaterializeAsync(loader(linkedToken), linkedToken).ConfigureAwait(false);
            if (!IsCurrentLoad(version))
            {
                return 0;
            }

            _allItems.AddRange(loaded);
            ApplyFilter();
            return loaded.Count;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && !IsCurrentLoad(version))
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

    public bool Update(IMessage message, ListKeyMap? keyMap = null)
    {
        keyMap ??= ListKeyMap.Default;
        var beforeSelection = SelectedIndex;
        var beforeOffset = _offset;

        if (message is KeyPressMsg key)
        {
            if (keyMap.Up.Matches(key))
            {
                MoveSelection(-1);
            }
            else if (keyMap.Down.Matches(key))
            {
                MoveSelection(1);
            }
            else if (keyMap.PageUp.Matches(key))
            {
                MoveSelection(-Math.Max(1, PageSize));
            }
            else if (keyMap.PageDown.Matches(key))
            {
                MoveSelection(Math.Max(1, PageSize));
            }
            else if (keyMap.Home.Matches(key))
            {
                Select(0);
            }
            else if (keyMap.End.Matches(key))
            {
                Select(Math.Max(0, _filteredIndexes.Count - 1));
            }
        }
        else if (message is MouseWheelMsg wheel)
        {
            if (wheel.Button == MouseButton.WheelUp)
            {
                MoveSelection(-1);
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                MoveSelection(1);
            }
        }

        return beforeSelection != SelectedIndex || beforeOffset != _offset;
    }

    public IReadOnlyList<ListRow<T>> VisibleRows()
    {
        var rows = new List<ListRow<T>>(Math.Max(1, PageSize));
        if (_filteredIndexes.Count == 0 || PageSize <= 0)
        {
            return rows;
        }

        var start = Math.Clamp(_offset, 0, Math.Max(0, _filteredIndexes.Count - 1));
        var max = Math.Min(PageSize, _filteredIndexes.Count - start);
        for (var i = 0; i < max; i++)
        {
            var filteredIndex = start + i;
            var sourceIndex = _filteredIndexes[filteredIndex];
            rows.Add(new ListRow<T>(_allItems[sourceIndex], filteredIndex, filteredIndex == SelectedIndex));
        }

        return rows;
    }

    public string LabelFor(T item) => _toText(item);

    private void MoveSelection(int delta)
    {
        if (_filteredIndexes.Count == 0)
        {
            SelectedIndex = -1;
            _offset = 0;
            return;
        }

        if (SelectedIndex < 0)
        {
            Select(0);
            return;
        }

        Select(SelectedIndex + delta);
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
        EnsureSelectionVisible();
    }

    private void EnsureSelectionVisible()
    {
        if (!HasSelection)
        {
            _offset = 0;
            return;
        }

        var page = Math.Max(1, PageSize);
        if (SelectedIndex < _offset)
        {
            _offset = SelectedIndex;
        }
        else if (SelectedIndex >= _offset + page)
        {
            _offset = SelectedIndex - page + 1;
        }

        var maxOffset = Math.Max(0, _filteredIndexes.Count - page);
        _offset = Math.Clamp(_offset, 0, maxOffset);
    }

    private void ApplyFilter()
    {
        _filteredIndexes.Clear();
        for (var i = 0; i < _allItems.Count; i++)
        {
            var label = _toText(_allItems[i]);
            if (Filter.Length == 0 || label.Contains(Filter, FilterComparison))
            {
                _filteredIndexes.Add(i);
            }
        }

        if (SortComparison is not null && _filteredIndexes.Count > 1)
        {
            _filteredIndexes.Sort((left, right) => SortComparison(_allItems[left], _allItems[right]));
        }

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
        EnsureSelectionVisible();
    }

    private async ValueTask AppendItemsCoreAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken)
    {
        await foreach (var item in items.ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            _allItems.Add(item);
        }
    }

    private static async ValueTask<List<T>> MaterializeAsync(IAsyncEnumerable<T> items, CancellationToken cancellationToken)
    {
        var result = new List<T>();
        await foreach (var item in items.ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            result.Add(item);
        }

        return result;
    }

    private (int Version, CancellationToken Token, Action Dispose) BeginTrackedLoad(CancellationToken cancellationToken)
    {
        CancellationTokenSource linkedCts;
        int version;
        lock (_loadGate)
        {
            _activeLoadCts?.Cancel();
            _activeLoadCts?.Dispose();
            _activeLoadCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts = _activeLoadCts;
            version = ++_loadVersion;
        }

        return (version, linkedCts.Token, () =>
        {
            lock (_loadGate)
            {
                if (ReferenceEquals(_activeLoadCts, linkedCts))
                {
                    _activeLoadCts.Dispose();
                    _activeLoadCts = null;
                }
                else
                {
                    linkedCts.Dispose();
                }
            }
        });
    }

    private bool IsCurrentLoad(int version)
    {
        lock (_loadGate)
        {
            return version == _loadVersion;
        }
    }
}
