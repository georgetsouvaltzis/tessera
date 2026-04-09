using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;

namespace Tessera.Controls;

public sealed partial class FuzzyFinder
{
    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused)
        {
            return false;
        }

        if (message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Escape))
        {
            if (_query.Value.Length > 0)
            {
                ClearQuery();
                return true;
            }

            if (IsOpen)
            {
                Close();
                return true;
            }

            return false;
        }

        if (!IsOpen && key.Key == Key.Character && key.Text.Length > 0)
        {
            IsOpen = true;
        }

        if (IsOpen)
        {
            if (key.Is(Key.Up) || key.IsCharacter('k'))
            {
                return MoveSelection(-1);
            }

            if (key.Is(Key.Down) || key.IsCharacter('j'))
            {
                return MoveSelection(1);
            }

            if (key.Is(Key.Home))
            {
                return SetSelectedIndex(0);
            }

            if (key.Is(Key.End))
            {
                return SetSelectedIndex(_results.Count - 1);
            }

            if (key.Is(Key.Enter) && !IsReadOnly)
            {
                return ActivateSelected();
            }
        }

        if (IsReadOnly)
        {
            return false;
        }

        var result = _query.Update(message);
        if (!result.Changed)
        {
            return false;
        }

        IsOpen = true;
        RefreshResults();
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || message is not PointerInput pointer || bounds.IsEmpty)
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
                changed |= SetHoveredIndex(-1);
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
                return changed | MoveSelection(1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return changed | MoveSelection(-1);
            }
        }

        if (!inside)
        {
            return changed || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(ResolveVisibleIndexFromPointer(content, pointer.Y));
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left)
        {
            return Handle(message);
        }

        RequestFocus();
        IsOpen = true;
        var index = ResolveVisibleIndexFromPointer(content, pointer.Y);
        changed |= SetHoveredIndex(index);
        if (index < 0)
        {
            return changed || true;
        }

        changed |= SetSelectedIndex(index);
        if (!IsReadOnly)
        {
            changed |= ActivateSelected();
        }

        return changed;
    }

    private int ResolveVisibleIndexFromPointer(in Rect content, int y)
    {
        if (y == content.Y)
        {
            return -1;
        }

        var visibleCount = ResolveVisibleResultCount(content.Height);
        var row = y - content.Y - 1;
        if (row < 0 || row >= visibleCount)
        {
            return -1;
        }

        var index = _scrollOffset + row;
        return index >= 0 && index < _results.Count
            ? index
            : -1;
    }

    private bool SetHoveredIndex(int index)
    {
        var normalized = index < 0 || index >= _results.Count
            ? -1
            : index;
        if (_hoveredIndex == normalized)
        {
            return false;
        }

        _hoveredIndex = normalized;
        return true;
    }

    private bool MoveSelection(int delta)
    {
        if (_results.Count == 0 || delta == 0)
        {
            return false;
        }

        return SetSelectedIndex(Math.Clamp(_selectedIndex + delta, 0, _results.Count - 1));
    }

    private bool SetSelectedIndex(int index)
    {
        if (_results.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _results.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _selectedIndex = clamped;
        SelectionChanged?.Invoke(this, new FuzzyFinderSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, SelectedItem));
        return true;
    }

    private bool ActivateSelected()
    {
        var item = SelectedItem;
        if (item is null)
        {
            return false;
        }

        LastSelectedItemId = item.Id;
        ItemSelected?.Invoke(this, new FuzzyFinderItemSelectedEventArgs(item, _query.Value));
        return true;
    }

    private int ResolveVisibleResultCount(int contentHeight)
    {
        var maxRows = Math.Max(0, contentHeight - 1);
        if (maxRows == 0)
        {
            return 0;
        }

        return Math.Min(maxRows, Math.Max(1, MaxVisibleResults));
    }

    private void EnsureScrollVisible(int visibleRows)
    {
        if (visibleRows <= 0 || _results.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        var maxOffset = Math.Max(0, _results.Count - visibleRows);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, maxOffset);
        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
        }
        else if (_selectedIndex >= _scrollOffset + visibleRows)
        {
            _scrollOffset = _selectedIndex - visibleRows + 1;
        }
    }
}
