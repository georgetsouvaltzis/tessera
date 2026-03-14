using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a horizontal menu bar that raises menu-item activation events.
/// </summary>
public sealed class MenuBar : Control
{
    private readonly List<MenuItem> _items = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;
    private long _activationVersion;
    private long _consumedActivationVersion;

    /// <summary>
    /// Occurs when a configured menu item is activated.
    /// </summary>
    public event EventHandler<MenuItemActivatedEventArgs>? ItemActivated;

    /// <summary>
    /// Gets the currently selected menu index.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets the configured menu items.
    /// </summary>
    public IReadOnlyList<MenuItem> Items => _items;

    public override bool IsFocused
    {
        get;
        set;
    }

    public override bool IsDisabled
    {
        get;
        set;
    }

    public override bool IsReadOnly
    {
        get;
        set;
    }

    public string? LastActivatedItemId { get; private set; }

    /// <summary>
    /// Replaces the configured menu items.
    /// </summary>
    /// <param name="items">The menu items to display. Item ids should remain stable and unique for activation handling.</param>
    public void SetItems(IEnumerable<MenuItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            _items.Add(item);
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
        _hoveredIndex = -1;
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Modifiers == ModifierKeys.None && key.Key == Key.Character && key.Text.Length == 1)
        {
            var shortcut = char.ToLowerInvariant(key.Text[0]);
            for (var index = 0; index < _items.Count; index++)
            {
                if (char.ToLowerInvariant(_items[index].Shortcut) != shortcut)
                {
                    continue;
                }

                _selectedIndex = index;
                if (!IsReadOnly)
                {
                    ActivateItem(_items[index]);
                }

                return true;
            }
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            _selectedIndex = (_selectedIndex + 1) % _items.Count;
            return true;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            _selectedIndex = (_selectedIndex + _items.Count - 1) % _items.Count;
            return true;
        }

        if (!IsReadOnly && (key.Is(Key.Enter) || key.IsCharacter(' ')))
        {
            ActivateItem(_items[_selectedIndex]);
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || _items.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var inRow = bounds.Contains(pointer.X, pointer.Y) && pointer.Y == bounds.Y;
        var changed = false;
        if (!inRow)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredIndex(-1);
            }

            return changed || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                _selectedIndex = (_selectedIndex + 1) % _items.Count;
                return true;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                _selectedIndex = (_selectedIndex + _items.Count - 1) % _items.Count;
                return true;
            }
        }

        var hovered = HitTestItemIndex(pointer.X, bounds);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            if (hovered >= 0 && _selectedIndex != hovered)
            {
                _selectedIndex = hovered;
                return SetHoveredIndex(hovered) || true;
            }

            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press)
        {
            changed |= SetHoveredIndex(hovered);
            if (pointer.Button == PointerButton.Left && hovered >= 0)
            {
                if (_selectedIndex != hovered)
                {
                    _selectedIndex = hovered;
                    changed = true;
                }

                if (!IsReadOnly)
                {
                    ActivateItem(_items[_selectedIndex]);
                    return true;
                }
            }
        }

        return changed || Handle(message);
    }

    /// <summary>
    /// Attempts to consume a pending menu activation from the wrapped legacy component.
    /// </summary>
    /// <param name="itemId">Receives the activated item id when available.</param>
    /// <returns><see langword="true"/> when an activation was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeActivation(out string itemId)
    {
        if (_activationVersion == _consumedActivationVersion || string.IsNullOrEmpty(LastActivatedItemId))
        {
            itemId = string.Empty;
            return false;
        }

        _consumedActivationVersion = _activationVersion;
        itemId = LastActivatedItemId;
        return true;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1 || _items.Count == 0)
        {
            return;
        }

        var x = clipped.X;
        for (var index = 0; index < _items.Count && x < clipped.Right; index++)
        {
            var label = FormatLabel(index, hovered: index == _hoveredIndex);
            canvas.WriteText(x, clipped.Y, label, clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(label) + 1;
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 0;
        for (var index = 0; index < _items.Count; index++)
        {
            width += ControlTextLayout.MeasureDisplayWidth(FormatLabel(index, hovered: false));
            if (index > 0)
            {
                width++;
            }
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(_items.Count == 0 ? 0 : 1, 0, availableBounds.Height));
    }

    private string FormatLabel(int index, bool hovered)
    {
        var item = _items[index];
        var core = item.Shortcut == '\0'
            ? item.Text
            : $"{item.Text}({item.Shortcut})";
        var label = index == _selectedIndex
            ? $"[{core}]"
            : $" {core} ";
        return hovered && index != _selectedIndex
            ? $">{label.Trim()}<"
            : label;
    }

    private int HitTestItemIndex(int x, Rect bounds)
    {
        var cursor = bounds.X;
        for (var index = 0; index < _items.Count && cursor < bounds.Right; index++)
        {
            var label = FormatLabel(index, hovered: false);
            var width = ControlTextLayout.MeasureDisplayWidth(label);
            var end = cursor + width;
            if (x >= cursor && x < end)
            {
                return index;
            }

            cursor = end + 1;
        }

        return -1;
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

    private void ActivateItem(MenuItem item)
    {
        LastActivatedItemId = item.Id;
        _activationVersion++;
        ItemActivated?.Invoke(this, new MenuItemActivatedEventArgs(item));
    }
}
