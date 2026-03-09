using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class DropdownComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<string> _items = [];
    private int _highlightedIndex;
    private int _hoveredIndex = -1;
    private bool _fieldHovered;

    public string Title { get; set; } = "Dropdown";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public WidgetStatePalette FieldStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetStatePalette OptionStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public Func<string, int, IReadOnlyCollection<WidgetVisualState>?>? OptionStateResolver { get; set; }

    public bool IsOpen { get; private set; }

    public int SelectedIndex { get; private set; } = -1;

    public int MaxVisibleItems { get; set; } = 6;

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public KeyBinding ToggleOpenKey { get; set; } = new("enter/space", "toggle", "enter", "space");

    public KeyBinding OpenKey { get; set; } = new("down", "open", "down");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ConfirmSelectionKey { get; set; } = new("enter/space", "select", "enter", "space");

    public string SelectedItem => SelectedIndex >= 0 && SelectedIndex < _items.Count
        ? _items[SelectedIndex]
        : string.Empty;

    public void SetItems(IEnumerable<string> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (_items.Count == 0)
        {
            SelectedIndex = -1;
            _highlightedIndex = 0;
            _hoveredIndex = -1;
            _fieldHovered = false;
            IsOpen = false;
            return;
        }

        if (SelectedIndex < 0 || SelectedIndex >= _items.Count)
        {
            SelectedIndex = 0;
        }

        _highlightedIndex = SelectedIndex;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key || _items.Count == 0)
        {
            return false;
        }

        if (!IsOpen)
        {
            if (ToggleOpenKey.Matches(key) || OpenKey.Matches(key))
            {
                IsOpen = true;
                _highlightedIndex = Math.Clamp(SelectedIndex, 0, _items.Count - 1);
                return true;
            }

            return false;
        }

        if (CloseKey.Matches(key))
        {
            IsOpen = false;
            return true;
        }

        if (NextItemKey.Matches(key))
        {
            _highlightedIndex = (_highlightedIndex + 1) % _items.Count;
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            _highlightedIndex = (_highlightedIndex + _items.Count - 1) % _items.Count;
            return true;
        }

        if (ConfirmSelectionKey.Matches(key))
        {
            SelectedIndex = _highlightedIndex;
            IsOpen = false;
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || ReadOnly || _items.Count == 0)
        {
            return false;
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(message.X, message.Y);
        var changed = false;

        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetFieldHovered(false);
                changed |= SetHoveredOptionIndex(-1);
            }

            if (message is not MouseWheelMsg)
            {
                return changed;
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && IsOpen)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                _highlightedIndex = (_highlightedIndex + 1) % _items.Count;
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                _highlightedIndex = (_highlightedIndex + _items.Count - 1) % _items.Count;
                changed = true;
            }
        }

        if (!inside)
        {
            return changed;
        }

        var hoveredField = message.Y == content.Y;
        var hoveredOptionIndex = RowToItemIndex(content, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetFieldHovered(hoveredField);
            changed |= SetHoveredOptionIndex(hoveredOptionIndex);
            if (hoveredOptionIndex >= 0)
            {
                _highlightedIndex = hoveredOptionIndex;
            }

            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetFieldHovered(hoveredField);
                changed |= SetHoveredOptionIndex(hoveredOptionIndex);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick)
            {
                if (hoveredField)
                {
                    if (!IsOpen && InteractionProfile.OpenOnClick)
                    {
                        IsOpen = true;
                        _highlightedIndex = Math.Clamp(SelectedIndex, 0, _items.Count - 1);
                        changed = true;
                    }
                    else if (IsOpen)
                    {
                        IsOpen = false;
                        changed = true;
                    }
                }
                else if (IsOpen && hoveredOptionIndex >= 0)
                {
                    _highlightedIndex = hoveredOptionIndex;
                    if (SelectedIndex != hoveredOptionIndex)
                    {
                        SelectedIndex = hoveredOptionIndex;
                        changed = true;
                    }

                    if (IsOpen)
                    {
                        IsOpen = false;
                        changed = true;
                    }
                }
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
            content = clipped.Inset(1, 1);
        }
        else
        {
            content = clipped;
        }

        if (content.IsEmpty)
        {
            return;
        }

        var indicator = IsOpen ? "^" : "v";
        var selected = _items.Count == 0 ? "(empty)" : SelectedItem;
        canvas.WriteText(content.X, content.Y, FieldStatePalette.Render($"{indicator} {selected}", ResolveFieldStates()), content.Width);

        if (!IsOpen || content.Height <= 1 || _items.Count == 0)
        {
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = ComputeWindowStart(_highlightedIndex, visibleRows, _items.Count);
        var end = Math.Min(_items.Count, start + visibleRows);
        var row = 0;
        for (var index = start; index < end; index++, row++)
        {
            var highlight = index == _highlightedIndex ? ">" : " ";
            var selectedMarker = index == SelectedIndex ? "*" : " ";
            var text = $"{highlight}{selectedMarker} {_items[index]}";
            canvas.WriteText(content.X, content.Y + 1 + row, OptionStatePalette.Render(text, ResolveOptionStates(index)), content.Width);
        }
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveFieldStates()
    {
        var states = new List<WidgetVisualState>(5);
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

        if (_items.Count == 0)
        {
            states.Add(WidgetVisualState.Empty);
        }

        if (_fieldHovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveOptionStates(int index)
    {
        var states = new List<WidgetVisualState>(7);
        states.AddRange(ResolveFieldStates());
        if (index == _highlightedIndex)
        {
            states.Add(WidgetVisualState.Cursor);
        }

        if (index == SelectedIndex)
        {
            states.Add(WidgetVisualState.Selected);
        }

        if (index == _hoveredIndex)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (OptionStateResolver?.Invoke(_items[index], index) is { } custom)
        {
            states.AddRange(custom);
        }

        return states;
    }

    private static int ComputeWindowStart(int highlightedIndex, int rows, int count)
    {
        if (count <= rows)
        {
            return 0;
        }

        var half = rows / 2;
        var start = highlightedIndex - half;
        if (start < 0)
        {
            return 0;
        }

        var maxStart = count - rows;
        if (start > maxStart)
        {
            return maxStart;
        }

        return start;
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

    private int RowToItemIndex(Rect content, int y)
    {
        if (!IsOpen || content.Height <= 1)
        {
            return -1;
        }

        var row = y - (content.Y + 1);
        if (row < 0 || row >= Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1))
        {
            return -1;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = ComputeWindowStart(_highlightedIndex, visibleRows, _items.Count);
        var index = start + row;
        if (index < 0 || index >= _items.Count)
        {
            return -1;
        }

        return index;
    }

    private bool SetHoveredOptionIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private bool SetFieldHovered(bool hovered)
    {
        if (_fieldHovered == hovered)
        {
            return false;
        }

        _fieldHovered = hovered;
        return true;
    }
}

