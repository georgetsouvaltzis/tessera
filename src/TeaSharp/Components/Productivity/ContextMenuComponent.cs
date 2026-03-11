using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Renders and routes a contextual action menu anchored to a screen position.
/// </summary>
public sealed partial class ContextMenuComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<ContextMenuItem> _items = [];
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    public ContextMenuComponent()
    {
    }

    public ContextMenuComponent(ContextMenuOptions options)
    {
        Title = options.Title;
        Focused = options.Focused;
        Disabled = options.Disabled;
        ReadOnly = options.ReadOnly;
        Border = options.Border;
        Padding = options.Padding;
        NextItemKey = options.NextItemKey ?? NextItemKey;
        PreviousItemKey = options.PreviousItemKey ?? PreviousItemKey;
        ExecuteKey = options.ExecuteKey ?? ExecuteKey;
        CloseKey = options.CloseKey ?? CloseKey;
        InteractionProfile = options.InteractionProfile ?? WidgetInteractionProfile.Default;
        if (options.Items is not null)
        {
            SetItems(options.Items);
        }
    }

    public string Title { get; set; } = "Context";

    public bool Visible { get; private set; }

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.Rounded;

    public Thickness Padding { get; set; }

    public int AnchorX { get; private set; }

    public int AnchorY { get; private set; }

    public string? LastExecutedItemId { get; private set; }

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ExecuteKey { get; set; } = new("enter/space", "execute", "enter", "space");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public IReadOnlyList<ContextMenuItem> Items => _items;

    public void SetItems(params ContextMenuItem[] items)
    {
        SetItems((IEnumerable<ContextMenuItem>)items);
    }

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

        if (!TryResolveMenuBounds(clipped, out var menuBounds, out var content))
        {
            return;
        }

        RenderMenu(canvas, menuBounds, content);
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
