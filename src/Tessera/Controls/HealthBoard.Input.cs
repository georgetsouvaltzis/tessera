using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;

namespace Tessera.Controls;

public sealed partial class HealthBoard
{
    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _services.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        var page = Math.Max(1, _lastViewportRows);
        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(SelectedIndex + 1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(SelectedIndex - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_services.Count - 1);
        }

        if (key.Is(Key.PageDown))
        {
            return SetSelectedIndex(SelectedIndex + page);
        }

        if (key.Is(Key.PageUp))
        {
            return SetSelectedIndex(SelectedIndex - page);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
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

        if (pointer.Kind == PointerEventKind.Wheel && _services.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(SelectedIndex + 1) || changed;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(SelectedIndex - 1) || changed;
            }
        }

        if (!inside)
        {
            return changed;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(_lastViewportRows);
        var hovered = _scrollOffset + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _services.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            changed |= SetHoveredIndex(hovered);
            changed |= SetSelectedIndex(hovered);
            return changed;
        }

        return changed;
    }
}
