namespace TeaSharp.Components;

internal sealed class OptionListController
{
    private readonly List<string> _items = [];
    private readonly List<int> _visibleIndices = [];

    public IReadOnlyList<string> Items => _items;

    public IReadOnlyList<int> VisibleIndices => _visibleIndices;

    public int SelectedIndex { get; private set; } = -1;

    public int HighlightedVisibleIndex { get; private set; }

    public int HoveredVisibleIndex { get; private set; } = -1;

    public int Count => _items.Count;

    public int VisibleCount => _visibleIndices.Count;

    public string SelectedItem => SelectedIndex >= 0 && SelectedIndex < _items.Count
        ? _items[SelectedIndex]
        : string.Empty;

    public void SetItems(IEnumerable<string> items, bool selectFirstItemWhenUnset)
    {
        _items.Clear();
        _items.AddRange(items);

        if (_items.Count == 0)
        {
            SelectedIndex = -1;
            HighlightedVisibleIndex = 0;
            HoveredVisibleIndex = -1;
            _visibleIndices.Clear();
            return;
        }

        if (SelectedIndex < 0 || SelectedIndex >= _items.Count)
        {
            SelectedIndex = selectFirstItemWhenUnset ? 0 : -1;
        }

        ApplyFilter(string.Empty);
    }

    public void ApplyFilter(string? filter)
    {
        _visibleIndices.Clear();
        var normalized = filter?.Trim() ?? string.Empty;
        for (var i = 0; i < _items.Count; i++)
        {
            if (normalized.Length == 0 || _items[i].Contains(normalized, StringComparison.OrdinalIgnoreCase))
            {
                _visibleIndices.Add(i);
            }
        }

        if (_visibleIndices.Count == 0)
        {
            HighlightedVisibleIndex = 0;
            HoveredVisibleIndex = -1;
            return;
        }

        if (SelectedIndex >= 0)
        {
            var selectedVisibleIndex = _visibleIndices.IndexOf(SelectedIndex);
            if (selectedVisibleIndex >= 0)
            {
                HighlightedVisibleIndex = selectedVisibleIndex;
                ClampHovered();
                return;
            }
        }

        HighlightedVisibleIndex = Math.Clamp(HighlightedVisibleIndex, 0, _visibleIndices.Count - 1);
        ClampHovered();
    }

    public void AlignHighlightToSelectionOrStart()
    {
        if (_visibleIndices.Count == 0)
        {
            HighlightedVisibleIndex = 0;
            return;
        }

        if (SelectedIndex >= 0)
        {
            var selectedVisibleIndex = _visibleIndices.IndexOf(SelectedIndex);
            if (selectedVisibleIndex >= 0)
            {
                HighlightedVisibleIndex = selectedVisibleIndex;
                return;
            }
        }

        HighlightedVisibleIndex = Math.Clamp(HighlightedVisibleIndex, 0, _visibleIndices.Count - 1);
    }

    public void MoveNextVisible()
    {
        if (_visibleIndices.Count == 0)
        {
            return;
        }

        HighlightedVisibleIndex = (HighlightedVisibleIndex + 1) % _visibleIndices.Count;
    }

    public void MovePreviousVisible()
    {
        if (_visibleIndices.Count == 0)
        {
            return;
        }

        HighlightedVisibleIndex = (HighlightedVisibleIndex + _visibleIndices.Count - 1) % _visibleIndices.Count;
    }

    public bool TrySelectHighlighted(out int selectedIndex)
    {
        selectedIndex = -1;
        if (_visibleIndices.Count == 0)
        {
            return false;
        }

        var selectedVisible = Math.Clamp(HighlightedVisibleIndex, 0, _visibleIndices.Count - 1);
        SelectedIndex = _visibleIndices[selectedVisible];
        selectedIndex = SelectedIndex;
        return true;
    }

    public bool SetHoveredVisibleIndex(int index)
    {
        if (HoveredVisibleIndex == index)
        {
            return false;
        }

        HoveredVisibleIndex = index;
        return true;
    }

    public bool SetSelectedIndex(int index)
    {
        if (SelectedIndex == index)
        {
            return false;
        }

        SelectedIndex = index;
        return true;
    }

    public int VisibleItemIndexAt(int visibleIndex)
    {
        return visibleIndex >= 0 && visibleIndex < _visibleIndices.Count
            ? _visibleIndices[visibleIndex]
            : -1;
    }

    private void ClampHovered()
    {
        if (HoveredVisibleIndex >= _visibleIndices.Count)
        {
            HoveredVisibleIndex = _visibleIndices.Count - 1;
        }
    }
}
