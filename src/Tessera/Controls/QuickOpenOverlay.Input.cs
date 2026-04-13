using Tessera.Components.Primitives;

namespace Tessera.Controls;

public sealed partial class QuickOpenOverlay
{
    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsOpen || IsDisabled || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Escape))
        {
            Cancel();
            return true;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return MoveSelection(1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return MoveSelection(-1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End) && _filteredIndices.Count > 0)
        {
            return SetSelectedIndex(_filteredIndices.Count - 1);
        }

        if (!IsReadOnly && key.Is(Key.Enter))
        {
            return SubmitSelection();
        }

        if (key.Is(Key.Backspace))
        {
            if (Query.Length == 0)
            {
                return false;
            }

            Query = Query[..^1];
            RefreshFilter();
            return true;
        }

        if (key.Key == Key.Character
            && key.Modifiers == ModifierKeys.None
            && !string.IsNullOrEmpty(key.Text))
        {
            Query = string.Concat(Query, key.Text);
            RefreshFilter();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (!IsOpen || IsDisabled || message is not PointerInput pointer ||
            !TryResolveOverlay(bounds, out _, out var content))
        {
            return Handle(message);
        }

        if (!bounds.Contains(pointer.X, pointer.Y))
        {
            return false;
        }

        if (pointer.Kind == PointerEventKind.Wheel && _filteredIndices.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return MoveSelection(1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return MoveSelection(-1);
            }
        }

        var insideContent = content.Contains(pointer.X, pointer.Y);
        if (!insideContent)
        {
            if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
            {
                Cancel();
                return true;
            }

            if (pointer.Kind == PointerEventKind.Motion)
            {
                return SetHoveredFilteredIndex(-1);
            }

            return false;
        }

        var hovered = ResolveHoveredFilteredIndex(content, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredFilteredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            var changed = SetHoveredFilteredIndex(hovered);
            if (hovered >= 0)
            {
                changed |= SetSelectedIndex(hovered);
                if (!IsReadOnly)
                {
                    changed |= SubmitSelection();
                }
            }

            return changed;
        }

        return false;
    }

    private bool MoveSelection(int delta)
    {
        if (_filteredIndices.Count == 0)
        {
            return false;
        }

        var target = (_selectedFilteredIndex + delta) % _filteredIndices.Count;
        if (target < 0)
        {
            target += _filteredIndices.Count;
        }

        return SetSelectedIndex(target);
    }

    private bool SetHoveredFilteredIndex(int index)
    {
        if (_hoveredFilteredIndex == index)
        {
            return false;
        }

        _hoveredFilteredIndex = index;
        return true;
    }

    private bool SubmitSelection()
    {
        if (_filteredIndices.Count == 0)
        {
            return false;
        }

        var item = _items[_filteredIndices[Math.Clamp(_selectedFilteredIndex, 0, _filteredIndices.Count - 1)]];
        Submitted?.Invoke(this, new QuickOpenOverlaySubmittedEventArgs(item, Query));
        Close();
        return true;
    }

    private void Cancel()
    {
        Close();
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    private int ResolveHoveredFilteredIndex(Rect content, int pointerY)
    {
        if (content.Height <= 1 || _filteredIndices.Count == 0)
        {
            return -1;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var row = pointerY - (content.Y + 1);
        if (row < 0 || row >= visibleRows)
        {
            return -1;
        }

        var start = ComputeWindowStart(_selectedFilteredIndex, visibleRows, _filteredIndices.Count);
        var filteredIndex = start + row;
        return filteredIndex >= 0 && filteredIndex < _filteredIndices.Count
            ? filteredIndex
            : -1;
    }
}
