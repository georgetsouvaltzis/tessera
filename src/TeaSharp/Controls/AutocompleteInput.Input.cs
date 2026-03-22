using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;

namespace TeaSharp.Controls;

public sealed partial class AutocompleteInput
{
    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused)
        {
            return false;
        }

        if (message is KeyPressed key)
        {
            if (key.Is(Key.Down) || key.IsCharacter('j'))
            {
                return MoveSelection(1);
            }

            if (key.Is(Key.Up) || key.IsCharacter('k'))
            {
                return MoveSelection(-1);
            }

            if (!IsReadOnly && (key.Is(Key.Enter) || key.Is(Key.Tab)))
            {
                return CommitSelection();
            }
        }

        if (IsReadOnly)
        {
            return false;
        }

        var update = _input.Update(message);
        if (!update.Changed)
        {
            return false;
        }

        RefreshFilteredSuggestions();
        return true;
    }

    /// <inheritdoc />
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

        if (pointer.Kind == PointerEventKind.Wheel && content.Contains(pointer.X, pointer.Y))
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

        if (pointer.Kind == PointerEventKind.Press
            && pointer.Button == PointerButton.Left
            && content.Contains(pointer.X, pointer.Y))
        {
            RequestFocus();
        }

        var popupStartY = content.Y + 1;
        var visibleSuggestions = ResolveVisibleSuggestionCount(content.Height);
        var withinPopup = IsPopupVisible
            && pointer.X >= content.X
            && pointer.X <= content.Right
            && pointer.Y >= popupStartY
            && pointer.Y < popupStartY + visibleSuggestions;

        if (!withinPopup)
        {
            if (pointer.Kind == PointerEventKind.Motion)
            {
                return SetHoveredSuggestionIndex(-1);
            }

            return Handle(message);
        }

        var row = pointer.Y - popupStartY;
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredSuggestionIndex(row);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            var changed = SetHoveredSuggestionIndex(row);
            changed |= SetSelectedSuggestionIndex(row);
            if (!IsReadOnly)
            {
                changed |= CommitSelection();
            }

            return changed;
        }

        return false;
    }

    private bool SetHoveredSuggestionIndex(int index)
    {
        var normalized = index >= 0 && index < _filteredSuggestionIndices.Count ? index : -1;
        if (_hoveredSuggestionIndex == normalized)
        {
            return false;
        }

        _hoveredSuggestionIndex = normalized;
        return true;
    }
}
