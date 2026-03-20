namespace TeaSharp.Controls;

public sealed partial class FuzzyFinder
{
    private void RefreshResults()
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var previousId = previousItem?.Id;

        _results.Clear();
        var query = _query.Value.Trim();
        for (var index = 0; index < _items.Count; index++)
        {
            var display = BuildDisplayLabel(_items[index]);
            if (!TryScore(query, display, out var score, out var matchIndices))
            {
                continue;
            }

            _results.Add(new ResultRow(index, score, matchIndices));
        }

        _results.Sort(static (left, right) =>
        {
            var scoreCompare = right.Score.CompareTo(left.Score);
            return scoreCompare != 0 ? scoreCompare : left.ItemIndex.CompareTo(right.ItemIndex);
        });

        if (_results.Count == 0)
        {
            _selectedIndex = 0;
            _scrollOffset = 0;
            _hoveredIndex = -1;
            RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
            return;
        }

        if (!string.IsNullOrEmpty(previousId))
        {
            for (var index = 0; index < _results.Count; index++)
            {
                if (_items[_results[index].ItemIndex].Id == previousId)
                {
                    _selectedIndex = index;
                    _scrollOffset = 0;
                    _hoveredIndex = _hoveredIndex >= _results.Count ? -1 : _hoveredIndex;
                    RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
                    return;
                }
            }
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, _results.Count - 1);
        _scrollOffset = 0;
        _hoveredIndex = _hoveredIndex >= _results.Count ? -1 : _hoveredIndex;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, FuzzyFinderItem? previousItem)
    {
        var selectedIndex = SelectedIndex;
        var selectedItem = SelectedItem;
        if (previousIndex == selectedIndex
            && EqualityComparer<FuzzyFinderItem?>.Default.Equals(previousItem, selectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new FuzzyFinderSelectionChangedEventArgs(previousIndex, selectedIndex, previousItem, selectedItem));
    }

    private static bool TryScore(string query, string candidate, out int score, out int[] matchIndices)
    {
        var trimmedQuery = query.AsSpan().Trim();
        if (trimmedQuery.IsEmpty)
        {
            score = 0;
            matchIndices = [];
            return true;
        }

        matchIndices = new int[trimmedQuery.Length];
        score = 0;
        var searchStart = 0;
        var previous = -2;
        for (var index = 0; index < trimmedQuery.Length; index++)
        {
            var foundAt = IndexOfOrdinalIgnoreCase(candidate, trimmedQuery[index], searchStart);
            if (foundAt < 0)
            {
                score = int.MinValue;
                matchIndices = [];
                return false;
            }

            matchIndices[index] = foundAt;
            score += 10;
            if (foundAt == 0 || IsWordBoundary(candidate[foundAt - 1]))
            {
                score += 6;
            }

            if (foundAt == previous + 1)
            {
                score += 4;
            }

            score -= foundAt / 32;
            previous = foundAt;
            searchStart = foundAt + 1;
        }

        score -= candidate.Length - trimmedQuery.Length;
        return true;
    }

    private static int IndexOfOrdinalIgnoreCase(string candidate, char value, int start)
    {
        var needle = char.ToLowerInvariant(value);
        for (var index = Math.Max(0, start); index < candidate.Length; index++)
        {
            if (char.ToLowerInvariant(candidate[index]) == needle)
            {
                return index;
            }
        }

        return -1;
    }

    private static bool IsWordBoundary(char value)
    {
        return value is ' ' or '-' or '_' or '/' or '\\' or '.';
    }

    private static string BuildDisplayLabel(FuzzyFinderItem item)
    {
        return string.IsNullOrWhiteSpace(item.Description)
            ? item.Label ?? string.Empty
            : $"{item.Label} - {item.Description}";
    }

    private readonly record struct ResultRow(int ItemIndex, int Score, int[] MatchIndices);
}
