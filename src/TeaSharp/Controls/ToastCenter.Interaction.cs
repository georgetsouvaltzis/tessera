using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

public sealed partial class ToastCenter
{
    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || message is not KeyPressed key)
        {
            return false;
        }

        if (!IsReadOnly && key.IsCharacter('c'))
        {
            if (_items.Count == 0)
            {
                return false;
            }

            Clear();
            return true;
        }

        if (_items.Count == 0)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return MoveSelection(+1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return MoveSelection(-1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_items.Count - 1);
        }

        if (!IsReadOnly && (key.Is(Key.Delete) || key.Is(Key.Backspace) || key.IsCharacter('d')))
        {
            return DismissSelected();
        }

        if (!IsReadOnly && key.IsCharacter('m'))
        {
            return ToggleMutedSelected();
        }

        return false;
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

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHoveredIndex(-1);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _items.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return MoveSelection(+1) || changed;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return MoveSelection(-1) || changed;
            }
        }

        if (!inside || _items.Count == 0)
        {
            return changed || Handle(message);
        }

        var hovered = ComputeWindowStart(ResolveRowCapacity(content.Height)) + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _items.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered) || changed;
        }

        if (pointer.Kind == PointerEventKind.Press && hovered >= 0)
        {
            changed |= SetHoveredIndex(hovered);
            changed |= SetSelectedIndex(hovered);
            RequestFocus();
            if (!IsReadOnly && pointer.Button == PointerButton.Right)
            {
                changed |= DismissSelected();
            }

            return changed;
        }

        return changed || Handle(message);
    }

    private bool MoveSelection(int delta)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var next = Math.Clamp(_selectedIndex + delta, 0, _items.Count - 1);
        return SetSelectedIndex(next);
    }

    private bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _items.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        _selectedIndex = clamped;
        return true;
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private int ComputeWindowStart(int rowCapacity)
    {
        return Math.Clamp(_selectedIndex - (rowCapacity / 2), 0, Math.Max(0, _items.Count - rowCapacity));
    }

    private int ResolveRowCapacity(int contentHeight)
    {
        return Math.Max(1, Math.Min(contentHeight, ResolveVisibleCapacity()));
    }

    private int ResolveVisibleCapacity()
    {
        return Math.Max(1, VisibleCapacity);
    }
}
