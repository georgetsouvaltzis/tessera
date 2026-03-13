using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Renders and routes a single-row menu surface with keyboard shortcuts and mouse activation.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class MenuBarComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<MenuBarItem> _items = [];
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private int _hoveredIndex = -1;
    private long _consumedActivationVersion;

    public MenuBarComponent()
    {
    }

    public MenuBarComponent(MenuBarOptions options)
    {
        IsFocused = options.IsFocused;
        IsDisabled = options.IsDisabled;
        IsReadOnly = options.IsReadOnly;
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

    public bool IsFocused { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsReadOnly { get; set; }

    public string? LastActivatedItemId { get; private set; }

    public long ActivationVersion { get; private set; }

    /// <summary>
    /// Raised when a menu item is activated by shortcut, keyboard selection, or mouse click.
    /// </summary>
    public event EventHandler<MenuBarItemActivatedEventArgs>? ItemActivated;

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

    /// <summary>
    /// Consumes the latest menu activation exactly once.
    /// </summary>
    public bool TryConsumeActivation(out string itemId)
    {
        if (ActivationVersion == _consumedActivationVersion || string.IsNullOrEmpty(LastActivatedItemId))
        {
            itemId = string.Empty;
            return false;
        }

        _consumedActivationVersion = ActivationVersion;
        itemId = LastActivatedItemId;
        return true;
    }

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
        if (!IsFocused || IsDisabled || message is not KeyPressMsg key || _items.Count == 0)
        {
            return false;
        }

        if (!IsReadOnly && key.Code == KeyCode.Character && key.Text.Length == 1 && key.Modifiers == KeyModifiers.None)
        {
            var c = char.ToLowerInvariant(key.Text[0]);
            for (var i = 0; i < _items.Count; i++)
            {
                if (char.ToLowerInvariant(_items[i].Shortcut) != c)
                {
                    continue;
                }

                SelectedIndex = i;
                ActivateItem(_items[i]);
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

        if (!IsReadOnly && ActivateKey.Matches(key))
        {
            ActivateSelectedItem();
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (IsDisabled || _items.Count == 0 || bounds.IsEmpty)
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

                if (!IsReadOnly)
                {
                    ActivateSelectedItem();
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
            if (IsFocused)
            {
                states.Add(WidgetVisualState.Focused);
            }

            if (IsDisabled)
            {
                states.Add(WidgetVisualState.Disabled);
            }

            if (IsReadOnly)
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

    private void ActivateSelectedItem()
    {
        ActivateItem(_items[SelectedIndex]);
    }

    private void ActivateItem(MenuBarItem item)
    {
        LastActivatedItemId = item.Id;
        ActivationVersion++;
        ItemActivated?.Invoke(this, new MenuBarItemActivatedEventArgs(item));
    }
}
