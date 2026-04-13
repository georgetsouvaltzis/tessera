using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;

namespace Tessera.Controls;

public sealed partial class SideNavRail
{
    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
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
            return SelectBoundary(true);
        }

        if (key.Is(Key.End))
        {
            return SelectBoundary(false);
        }

        if (key.Is(Key.Left))
        {
            return SetCollapsed(true);
        }

        if (key.Is(Key.Right))
        {
            return SetCollapsed(false);
        }

        if (!IsReadOnly && (key.Is(Key.Enter) || key.IsCharacter(' ')))
        {
            return ActivateSelection();
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer || IsDisabled)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredIndex(-1);
            }

            return false;
        }

        if (pointer.Kind == PointerEventKind.Wheel)
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

        if (pointer.Y == content.Y)
        {
            if (pointer.Kind == PointerEventKind.Motion)
            {
                return SetHoveredIndex(-1);
            }

            if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
            {
                return SetCollapsed(!IsCollapsed);
            }

            return false;
        }

        var hovered = ResolveItemIndexByPointer(content, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            var changed = SetHoveredIndex(hovered);
            if (hovered < 0 || _items[hovered].IsDisabled)
            {
                return changed;
            }

            changed |= TrySetSelectedIndex(hovered, true);
            if (!IsReadOnly)
            {
                changed |= ActivateSelection();
            }

            return changed;
        }

        return false;
    }
}
