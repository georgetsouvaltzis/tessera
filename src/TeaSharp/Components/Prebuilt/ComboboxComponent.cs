using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class ComboboxComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<string> _items = [];
    private readonly List<int> _filteredIndices = [];
    private int _highlightedFilteredIndex;
    private int _hoveredFilteredIndex = -1;
    private bool _fieldHovered;

    public TextInputModel Input { get; } = new();

    public TextInputKeyMap InputKeyMap { get; set; } = TextInputKeyMap.Default;

    public string Title { get; set; } = "Combobox";

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

    public KeyBinding OpenKey { get; set; } = new("down", "open", "down");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ConfirmSelectionKey { get; set; } = new("enter", "select", "enter");

    public string SelectedItem => SelectedIndex >= 0 && SelectedIndex < _items.Count
        ? _items[SelectedIndex]
        : string.Empty;

    public void SetItems(IEnumerable<string> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = -1;
        }

        RefreshFilteredIndices();
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly)
        {
            return false;
        }

        if (message is KeyPressMsg key)
        {
            if (IsOpen && CloseKey.Matches(key))
            {
                IsOpen = false;
                return true;
            }

            if (IsOpen && NextItemKey.Matches(key) && _filteredIndices.Count > 0)
            {
                _highlightedFilteredIndex = (_highlightedFilteredIndex + 1) % _filteredIndices.Count;
                return true;
            }

            if (IsOpen && PreviousItemKey.Matches(key) && _filteredIndices.Count > 0)
            {
                _highlightedFilteredIndex = (_highlightedFilteredIndex + _filteredIndices.Count - 1) % _filteredIndices.Count;
                return true;
            }

            if (IsOpen && ConfirmSelectionKey.Matches(key))
            {
                return SelectHighlighted();
            }

            if (!IsOpen && OpenKey.Matches(key))
            {
                IsOpen = true;
                if (_filteredIndices.Count > 0)
                {
                    _highlightedFilteredIndex = 0;
                }

                return true;
            }
        }

        var inputResult = Input.Update(message, InputKeyMap);
        if (inputResult.Changed)
        {
            RefreshFilteredIndices();
            IsOpen = true;
            return true;
        }

        if (inputResult.Submitted && IsOpen && _filteredIndices.Count > 0)
        {
            return SelectHighlighted();
        }

        return inputResult.Submitted;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || ReadOnly)
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
                changed |= SetHoveredFilteredIndex(-1);
            }

            if (message is not MouseWheelMsg)
            {
                return changed;
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && IsOpen && _filteredIndices.Count > 0)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                _highlightedFilteredIndex = (_highlightedFilteredIndex + 1) % _filteredIndices.Count;
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                _highlightedFilteredIndex = (_highlightedFilteredIndex + _filteredIndices.Count - 1) % _filteredIndices.Count;
                changed = true;
            }
        }

        if (!inside)
        {
            return changed;
        }

        var hoveredField = message.Y == content.Y;
        var hoveredOption = RowToFilteredIndex(content, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetFieldHovered(hoveredField);
            changed |= SetHoveredFilteredIndex(hoveredOption);
            if (hoveredOption >= 0)
            {
                _highlightedFilteredIndex = hoveredOption;
            }

            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetFieldHovered(hoveredField);
                changed |= SetHoveredFilteredIndex(hoveredOption);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick)
            {
                if (hoveredField)
                {
                    if (!IsOpen && InteractionProfile.OpenOnClick)
                    {
                        IsOpen = true;
                        if (_filteredIndices.Count > 0)
                        {
                            _highlightedFilteredIndex = Math.Clamp(_highlightedFilteredIndex, 0, _filteredIndices.Count - 1);
                        }

                        changed = true;
                    }
                    else if (IsOpen)
                    {
                        IsOpen = false;
                        changed = true;
                    }
                }
                else if (IsOpen && hoveredOption >= 0)
                {
                    _highlightedFilteredIndex = hoveredOption;
                    changed |= SelectHighlighted();
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

        var frameWidth = Math.Max(1, content.Width - 2);
        var frame = Input.BuildFrame(frameWidth);
        canvas.WriteText(content.X, content.Y, FieldStatePalette.Render($"{(IsOpen ? "^" : "v")} {frame.Text}", ResolveFieldStates()), content.Width);

        if (!IsOpen || content.Height <= 1)
        {
            return;
        }

        if (_filteredIndices.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, OptionStatePalette.Render("(no matches)", ResolveNoMatchStates()), content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = ComputeWindowStart(_highlightedFilteredIndex, visibleRows, _filteredIndices.Count);
        var end = Math.Min(_filteredIndices.Count, start + visibleRows);
        var row = 0;
        for (var i = start; i < end; i++, row++)
        {
            var itemIndex = _filteredIndices[i];
            var highlight = i == _highlightedFilteredIndex ? ">" : " ";
            var selectedMarker = itemIndex == SelectedIndex ? "*" : " ";
            var text = $"{highlight}{selectedMarker} {_items[itemIndex]}";
            canvas.WriteText(content.X, content.Y + 1 + row, OptionStatePalette.Render(text, ResolveOptionStates(i, itemIndex)), content.Width);
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

        if (!string.IsNullOrEmpty(Input.Value))
        {
            states.Add(WidgetVisualState.Editing);
        }

        if (_fieldHovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveNoMatchStates()
    {
        var states = new List<WidgetVisualState>(6);
        states.AddRange(ResolveFieldStates());
        states.Add(WidgetVisualState.Empty);
        if (!string.IsNullOrWhiteSpace(Input.Value))
        {
            states.Add(WidgetVisualState.FilteredOut);
        }

        return states;
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveOptionStates(int filteredIndex, int itemIndex)
    {
        var states = new List<WidgetVisualState>(7);
        states.AddRange(ResolveFieldStates());
        if (filteredIndex == _highlightedFilteredIndex)
        {
            states.Add(WidgetVisualState.Cursor);
        }

        if (itemIndex == SelectedIndex)
        {
            states.Add(WidgetVisualState.Selected);
        }

        if (filteredIndex == _hoveredFilteredIndex)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (OptionStateResolver?.Invoke(_items[itemIndex], itemIndex) is { } custom)
        {
            states.AddRange(custom);
        }

        return states;
    }

    private bool SelectHighlighted()
    {
        if (_filteredIndices.Count == 0)
        {
            IsOpen = false;
            return true;
        }

        var selectedFiltered = Math.Clamp(_highlightedFilteredIndex, 0, _filteredIndices.Count - 1);
        SelectedIndex = _filteredIndices[selectedFiltered];
        Input.SetValue(_items[SelectedIndex]);
        RefreshFilteredIndices();
        IsOpen = false;
        return true;
    }

    private void RefreshFilteredIndices()
    {
        _filteredIndices.Clear();
        var filter = Input.Value.Trim();
        for (var i = 0; i < _items.Count; i++)
        {
            var include = filter.Length == 0
                || _items[i].Contains(filter, StringComparison.OrdinalIgnoreCase);
            if (include)
            {
                _filteredIndices.Add(i);
            }
        }

        if (_filteredIndices.Count == 0)
        {
            _highlightedFilteredIndex = 0;
            _hoveredFilteredIndex = -1;
            return;
        }

        if (SelectedIndex >= 0)
        {
            var selectedFilteredIndex = _filteredIndices.IndexOf(SelectedIndex);
            if (selectedFilteredIndex >= 0)
            {
                _highlightedFilteredIndex = selectedFilteredIndex;
                return;
            }
        }

        _highlightedFilteredIndex = Math.Clamp(_highlightedFilteredIndex, 0, _filteredIndices.Count - 1);
        if (_hoveredFilteredIndex >= _filteredIndices.Count)
        {
            _hoveredFilteredIndex = _filteredIndices.Count - 1;
        }
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

    private int RowToFilteredIndex(Rect content, int y)
    {
        if (!IsOpen || content.Height <= 1 || _filteredIndices.Count == 0)
        {
            return -1;
        }

        var row = y - (content.Y + 1);
        if (row < 0 || row >= Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1))
        {
            return -1;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = ComputeWindowStart(_highlightedFilteredIndex, visibleRows, _filteredIndices.Count);
        var filtered = start + row;
        if (filtered < 0 || filtered >= _filteredIndices.Count)
        {
            return -1;
        }

        return filtered;
    }

    private bool SetHoveredFilteredIndex(int index)
    {
        if (_hoveredFilteredIndex == index)
        {
            return false;
        }

        _hoveredFilteredIndex = index;
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

