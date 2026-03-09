namespace TeaSharp.Components;

internal sealed class CommandPaletteController
{
    private readonly List<CommandPaletteItem> _items = [];
    private readonly List<int> _filteredIndices = [];

    public IReadOnlyList<int> FilteredIndices => _filteredIndices;

    public int SelectedFilteredIndex { get; private set; }

    public int HoveredFilteredIndex { get; private set; } = -1;

    public int FilteredCount => _filteredIndices.Count;

    public void SetItems(IEnumerable<CommandPaletteItem> items, string query)
    {
        _items.Clear();
        _items.AddRange(items);
        Refresh(query);
    }

    public void Refresh(string query)
    {
        _filteredIndices.Clear();
        var filter = query.Trim();
        for (var i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            var include = filter.Length == 0
                || item.Title.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || item.Description.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || item.Id.Contains(filter, StringComparison.OrdinalIgnoreCase);
            if (include)
            {
                _filteredIndices.Add(i);
            }
        }

        if (_filteredIndices.Count == 0)
        {
            SelectedFilteredIndex = 0;
            HoveredFilteredIndex = -1;
            return;
        }

        SelectedFilteredIndex = Math.Clamp(SelectedFilteredIndex, 0, _filteredIndices.Count - 1);
        if (HoveredFilteredIndex >= _filteredIndices.Count)
        {
            HoveredFilteredIndex = _filteredIndices.Count - 1;
        }
    }

    public void MoveNext()
    {
        if (_filteredIndices.Count > 0)
        {
            SelectedFilteredIndex = (SelectedFilteredIndex + 1) % _filteredIndices.Count;
        }
    }

    public void MovePrevious()
    {
        if (_filteredIndices.Count > 0)
        {
            SelectedFilteredIndex = (SelectedFilteredIndex + _filteredIndices.Count - 1) % _filteredIndices.Count;
        }
    }

    public bool SetHovered(int index)
    {
        if (HoveredFilteredIndex == index)
        {
            return false;
        }

        HoveredFilteredIndex = index;
        return true;
    }

    public bool SetSelectedFilteredIndex(int index)
    {
        if (SelectedFilteredIndex == index)
        {
            return false;
        }

        SelectedFilteredIndex = index;
        return true;
    }

    public CommandPaletteItem? GetSelectedItem()
    {
        if (_filteredIndices.Count == 0)
        {
            return null;
        }

        var selected = Math.Clamp(SelectedFilteredIndex, 0, _filteredIndices.Count - 1);
        return _items[_filteredIndices[selected]];
    }

    public CommandPaletteItem GetFilteredItem(int filteredIndex)
    {
        return _items[_filteredIndices[filteredIndex]];
    }
}
