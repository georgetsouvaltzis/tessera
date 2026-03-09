using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class ContextMenuComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<ContextMenuItem> _items = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    public string Title { get; set; } = "Context";

    public bool Visible { get; private set; }

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public int AnchorX { get; private set; }

    public int AnchorY { get; private set; }

    public string? LastExecutedItemId { get; private set; }

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ExecuteKey { get; set; } = new("enter/space", "execute", "enter", "space");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public IReadOnlyList<ContextMenuItem> Items => _items;

    public void SetItems(IEnumerable<ContextMenuItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
    }

    public void OpenAt(int x, int y)
    {
        Visible = true;
        AnchorX = Math.Max(0, x);
        AnchorY = Math.Max(0, y);
        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
    }

    public void Close()
    {
        Visible = false;
    }

    public bool Update(IMessage message)
    {
        if (!Visible || !Focused || Disabled || message is not KeyPressMsg key)
        {
            return false;
        }

        if (CloseKey.Matches(key))
        {
            Close();
            return true;
        }

        if (_items.Count == 0)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            _selectedIndex = (_selectedIndex + 1) % _items.Count;
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            _selectedIndex = (_selectedIndex + _items.Count - 1) % _items.Count;
            return true;
        }

        if (!ReadOnly && ExecuteKey.Matches(key))
        {
            LastExecutedItemId = _items[_selectedIndex].Id;
            Close();
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (!Visible || Disabled || !TryResolveMenuBounds(bounds, out var menuBounds, out var content))
        {
            return false;
        }

        var insideMenu = ContainsWithRightTolerance(menuBounds, message.X, message.Y);
        var changed = false;
        if (!insideMenu)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredIndex(-1);
            }

            if (message is MouseClickMsg or MouseReleaseMsg && InteractionProfile.ActivateOnClick)
            {
                Close();
                changed = true;
            }

            return changed;
        }

        if (_items.Count == 0)
        {
            return changed;
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                _selectedIndex = (_selectedIndex + 1) % _items.Count;
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                _selectedIndex = (_selectedIndex + _items.Count - 1) % _items.Count;
                changed = true;
            }
        }

        if (!ContainsWithRightTolerance(content, message.X, message.Y))
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredIndex(-1);
            }

            return changed;
        }

        var hovered = RowFromPointer(content, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredIndex(hovered);
            return changed;
        }

        if (message is MouseClickMsg or MouseReleaseMsg)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredIndex(hovered);
            }

            var leftActivate = message.Button == MouseButton.Left || message is MouseReleaseMsg;
            if (leftActivate && InteractionProfile.ActivateOnClick)
            {
                var target = hovered >= 0
                    ? hovered
                    : _selectedIndex;
                if (target < 0 || target >= _items.Count)
                {
                    return changed;
                }

                if (_selectedIndex != target)
                {
                    _selectedIndex = target;
                    changed = true;
                }

                if (!ReadOnly)
                {
                    LastExecutedItemId = _items[_selectedIndex].Id;
                    Close();
                    changed = true;
                }
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!Visible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var itemWidth = _items.Count == 0
            ? 12
            : Math.Max(12, _items.Max(item => item.Title.Length + 4));
        var width = Math.Min(itemWidth, clipped.Width);
        var height = Math.Min(Math.Max(3, _items.Count + 2), clipped.Height);

        var x = Math.Clamp(AnchorX, clipped.X, Math.Max(clipped.X, clipped.Right - width));
        var y = Math.Clamp(AnchorY, clipped.Y, Math.Max(clipped.Y, clipped.Bottom - height));
        var bounds = new Rect(x, y, width, height);

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(bounds, Title, BorderStyle.Rounded);
            content = bounds.Inset(1, 1);
        }
        else
        {
            content = bounds;
        }

        if (content.IsEmpty)
        {
            return;
        }

        if (_items.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ItemStatePalette.Render("(empty)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        for (var i = 0; i < rows; i++)
        {
            var states = new List<WidgetVisualState>(6);
            if (Focused)
            {
                states.Add(WidgetVisualState.Focused);
            }

            if (Disabled)
            {
                states.Add(WidgetVisualState.Disabled);
            }

            if (ReadOnly)
            {
                states.Add(WidgetVisualState.ReadOnly);
            }

            if (i == _selectedIndex)
            {
                states.Add(WidgetVisualState.Cursor);
                states.Add(WidgetVisualState.Selected);
            }

            if (i == _hoveredIndex)
            {
                states.Add(WidgetVisualState.Hovered);
            }

            var itemStates = _items[i].States;
            if (itemStates is not null)
            {
                states.AddRange(itemStates);
            }

            var cursor = i == _selectedIndex ? ">" : " ";
            canvas.WriteText(content.X, content.Y + i, ItemStatePalette.Render($"{cursor} {_items[i].Title}", states), content.Width);
        }
    }

    private bool TryResolveMenuBounds(Rect bounds, out Rect menuBounds, out Rect content)
    {
        menuBounds = default;
        content = default;

        var clipped = bounds;
        if (clipped.IsEmpty)
        {
            return false;
        }

        var itemWidth = _items.Count == 0
            ? 12
            : Math.Max(12, _items.Max(item => item.Title.Length + 4));
        var width = Math.Min(itemWidth, clipped.Width);
        var height = Math.Min(Math.Max(3, _items.Count + 2), clipped.Height);

        var x = Math.Clamp(AnchorX, clipped.X, Math.Max(clipped.X, clipped.Right - width));
        var y = Math.Clamp(AnchorY, clipped.Y, Math.Max(clipped.Y, clipped.Bottom - height));
        menuBounds = new Rect(x, y, width, height);
        content = ShowBorder
            ? menuBounds.Inset(1, 1)
            : menuBounds;
        return !content.IsEmpty;
    }

    private int RowFromPointer(Rect content, int y)
    {
        var row = y - content.Y;
        if (row < 0 || row >= Math.Min(content.Height, _items.Count))
        {
            return -1;
        }

        return row;
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

    private static bool ContainsWithRightTolerance(Rect rect, int x, int y)
    {
        return y >= rect.Y
            && y < rect.Bottom
            && x >= rect.X
            && x <= rect.Right;
    }
}

