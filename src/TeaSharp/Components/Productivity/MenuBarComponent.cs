using System.ComponentModel;
using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

/// <summary>
/// Renders and routes a single-row menu surface with keyboard shortcuts and mouse activation.
/// </summary>
public sealed class MenuBarComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<MenuBarItem> _items = [];
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private int _hoveredIndex = -1;

    public MenuBarComponent()
    {
    }

    public MenuBarComponent(MenuBarOptions options)
    {
        Focused = options.Focused;
        Disabled = options.Disabled;
        ReadOnly = options.ReadOnly;
        NextItemKey = options.NextItemKey ?? NextItemKey;
        PreviousItemKey = options.PreviousItemKey ?? PreviousItemKey;
        ActivateKey = options.ActivateKey ?? ActivateKey;
        InteractionProfile = options.InteractionProfile ?? WidgetInteractionProfile.Default;
        if (options.Items is not null)
        {
            SetItems(options.Items);
        }
    }

    public int SelectedIndex { get; private set; }

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public string? LastActivatedItemId { get; private set; }

    public long ActivationVersion { get; private set; }

    public KeyBinding NextItemKey { get; set; } = new("right/l", "next item", "right", "l");

    public KeyBinding PreviousItemKey { get; set; } = new("left/h", "previous item", "left", "h");

    public KeyBinding ActivateKey { get; set; } = new("enter/space", "activate", "enter", "space");

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public IReadOnlyList<MenuBarItem> Items => _items;

    public void SetItems(params MenuBarItem[] items)
    {
        SetItems((IEnumerable<MenuBarItem>)items);
    }

    public void SetItems(IEnumerable<MenuBarItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (_items.Count == 0)
        {
            SelectedIndex = 0;
            return;
        }

        SelectedIndex = Math.Clamp(SelectedIndex, 0, _items.Count - 1);
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || message is not KeyPressMsg key || _items.Count == 0)
        {
            return false;
        }

        if (!ReadOnly && key.Code == KeyCode.Character && key.Text.Length == 1 && key.Modifiers == KeyModifiers.None)
        {
            var c = char.ToLowerInvariant(key.Text[0]);
            for (var i = 0; i < _items.Count; i++)
            {
                if (char.ToLowerInvariant(_items[i].Shortcut) != c)
                {
                    continue;
                }

                SelectedIndex = i;
                LastActivatedItemId = _items[i].Id;
                ActivationVersion++;
                return true;
            }
        }

        if (NextItemKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + 1) % _items.Count;
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + _items.Count - 1) % _items.Count;
            return true;
        }

        if (!ReadOnly && ActivateKey.Matches(key))
        {
            LastActivatedItemId = _items[SelectedIndex].Id;
            ActivationVersion++;
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || _items.Count == 0 || bounds.IsEmpty)
        {
            return false;
        }

        var inRow = bounds.Contains(message.X, message.Y) && message.Y == bounds.Y;
        var changed = false;
        if (!inRow)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredIndex(-1);
            }

            return changed;
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                SelectedIndex = (SelectedIndex + 1) % _items.Count;
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                SelectedIndex = (SelectedIndex + _items.Count - 1) % _items.Count;
                changed = true;
            }
        }

        var hovered = HitTestItemIndex(message.X, bounds);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredIndex(hovered);
            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredIndex(hovered);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && hovered >= 0)
            {
                if (SelectedIndex != hovered)
                {
                    SelectedIndex = hovered;
                    changed = true;
                }

                if (!ReadOnly)
                {
                    LastActivatedItemId = _items[SelectedIndex].Id;
                    ActivationVersion++;
                    changed = true;
                }
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1 || _items.Count == 0)
        {
            return;
        }

        var x = clipped.X;
        for (var i = 0; i < _items.Count && x < clipped.Right; i++)
        {
            var label = _items[i].Shortcut == '\0'
                ? $" {_items[i].Title} "
                : $" {_items[i].Title}({_items[i].Shortcut}) ";

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

            if (i == SelectedIndex)
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

            var rendered = ItemStatePalette.Render(label, states);
            canvas.WriteText(x, clipped.Y, rendered, clipped.Right - x);
            x += label.Length + 1;
        }
    }

    private int HitTestItemIndex(int x, Rect bounds)
    {
        var cursor = bounds.X;
        for (var i = 0; i < _items.Count && cursor < bounds.Right; i++)
        {
            var label = _items[i].Shortcut == '\0'
                ? $" {_items[i].Title} "
                : $" {_items[i].Title}({_items[i].Shortcut}) ";
            var end = cursor + label.Length;
            if (x >= cursor && x < end)
            {
                return i;
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
}
