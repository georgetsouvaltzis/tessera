using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class ComboboxComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly OptionListController _options = new();
    private readonly TextInputModel _input = new();
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private bool _fieldHovered;

    public ComboboxComponent()
    {
    }

    public ComboboxComponent(ComboboxOptions options)
    {
        Title = options.Title;
        Placeholder = options.Placeholder;
        Focused = options.Focused;
        Disabled = options.Disabled;
        ReadOnly = options.ReadOnly;
        ShowBorder = options.ShowBorder;
        MaxVisibleItems = options.MaxVisibleItems;
        InputKeyMap = options.InputKeyMap ?? TextInputKeyMap.Default;
        InteractionProfile = options.InteractionProfile ?? WidgetInteractionProfile.Default;
        OpenKey = options.OpenKey ?? new KeyBinding("down", "open", "down");
        CloseKey = options.CloseKey ?? new KeyBinding("esc", "close", "escape");
        NextItemKey = options.NextItemKey ?? new KeyBinding("down/j", "next item", "down", "j");
        PreviousItemKey = options.PreviousItemKey ?? new KeyBinding("up/k", "previous item", "up", "k");
        ConfirmSelectionKey = options.ConfirmSelectionKey ?? new KeyBinding("enter", "select", "enter");
        if (options.Items is { Count: > 0 } items)
        {
            SetItems(items);
        }

        if (!string.IsNullOrEmpty(options.InitialFilter))
        {
            SetFilterText(options.InitialFilter);
        }
    }

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

    public int SelectedIndex => _options.SelectedIndex;

    public int MaxVisibleItems { get; set; } = 6;

    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public KeyBinding OpenKey { get; set; } = new("down", "open", "down");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ConfirmSelectionKey { get; set; } = new("enter", "select", "enter");

    public string SelectedItem => _options.SelectedItem;

    public string FilterText => _input.Value;

    public string Placeholder
    {
        get => _input.Placeholder;
        set => _input.Placeholder = value;
    }

    public void SetFilterText(string value)
    {
        _input.SetValue(value);
        _options.ApplyFilter(_input.Value);
    }

    public void SetItems(IEnumerable<string> items)
    {
        _options.SetItems(items, selectFirstItemWhenUnset: false);
        _options.ApplyFilter(_input.Value);
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

            if (IsOpen && NextItemKey.Matches(key) && _options.VisibleCount > 0)
            {
                _options.MoveNextVisible();
                return true;
            }

            if (IsOpen && PreviousItemKey.Matches(key) && _options.VisibleCount > 0)
            {
                _options.MovePreviousVisible();
                return true;
            }

            if (IsOpen && ConfirmSelectionKey.Matches(key))
            {
                return SelectHighlighted();
            }

            if (!IsOpen && OpenKey.Matches(key))
            {
                IsOpen = true;
                _options.AlignHighlightToSelectionOrStart();
                return true;
            }
        }

        var inputResult = _input.Update(message, InputKeyMap);
        if (inputResult.Changed)
        {
            _options.ApplyFilter(_input.Value);
            IsOpen = true;
            return true;
        }

        if (inputResult.Submitted && IsOpen && _options.VisibleCount > 0)
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
                changed |= _options.SetHoveredVisibleIndex(-1);
            }

            if (message is not MouseWheelMsg)
            {
                return changed;
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && IsOpen && _options.VisibleCount > 0)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                _options.MoveNextVisible();
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                _options.MovePreviousVisible();
                changed = true;
            }
        }

        if (!inside)
        {
            return changed;
        }

        var hoveredField = message.Y == content.Y;
        var hoveredOption = RowToVisibleIndex(content, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetFieldHovered(hoveredField);
            changed |= _options.SetHoveredVisibleIndex(hoveredOption);
            if (hoveredOption >= 0)
            {
                SetHighlightedVisibleIndex(hoveredOption);
            }

            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetFieldHovered(hoveredField);
                changed |= _options.SetHoveredVisibleIndex(hoveredOption);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick)
            {
                if (hoveredField)
                {
                    if (!IsOpen && InteractionProfile.OpenOnClick)
                    {
                        IsOpen = true;
                        _options.AlignHighlightToSelectionOrStart();
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
                    changed |= SetHighlightedVisibleIndex(hoveredOption);
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
        var frame = _input.BuildFrame(frameWidth);
        canvas.WriteText(content.X, content.Y, FieldStatePalette.Render($"{(IsOpen ? "^" : "v")} {frame.Text}", ResolveFieldStates()), content.Width);

        if (!IsOpen || content.Height <= 1)
        {
            return;
        }

        if (_options.VisibleCount == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, OptionStatePalette.Render("(no matches)", ResolveNoMatchStates()), content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = OptionListViewport.ComputeWindowStart(_options.HighlightedVisibleIndex, visibleRows, _options.VisibleCount);
        var end = Math.Min(_options.VisibleCount, start + visibleRows);
        var row = 0;
        for (var visibleIndex = start; visibleIndex < end; visibleIndex++, row++)
        {
            var itemIndex = _options.VisibleItemIndexAt(visibleIndex);
            var highlight = visibleIndex == _options.HighlightedVisibleIndex ? ">" : " ";
            var selectedMarker = itemIndex == _options.SelectedIndex ? "*" : " ";
            var text = $"{highlight}{selectedMarker} {_options.Items[itemIndex]}";
            canvas.WriteText(content.X, content.Y + 1 + row, OptionStatePalette.Render(text, ResolveOptionStates(visibleIndex, itemIndex)), content.Width);
        }
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveFieldStates()
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

        if (_options.Count == 0)
        {
            states.Add(WidgetVisualState.Empty);
        }

        if (!string.IsNullOrEmpty(_input.Value))
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
        if (!string.IsNullOrWhiteSpace(_input.Value))
        {
            states.Add(WidgetVisualState.FilteredOut);
        }

        return states;
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveOptionStates(int visibleIndex, int itemIndex)
    {
        var states = new List<WidgetVisualState>(7);
        states.AddRange(ResolveFieldStates());
        if (visibleIndex == _options.HighlightedVisibleIndex)
        {
            states.Add(WidgetVisualState.Cursor);
        }

        if (itemIndex == _options.SelectedIndex)
        {
            states.Add(WidgetVisualState.Selected);
        }

        if (visibleIndex == _options.HoveredVisibleIndex)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (OptionStateResolver?.Invoke(_options.Items[itemIndex], itemIndex) is { } custom)
        {
            states.AddRange(custom);
        }

        return states;
    }

    private bool SelectHighlighted()
    {
        if (!_options.TrySelectHighlighted(out var selectedIndex))
        {
            IsOpen = false;
            return true;
        }

        _input.SetValue(_options.Items[selectedIndex]);
        _options.ApplyFilter(_input.Value);
        IsOpen = false;
        return true;
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder ? bounds.Inset(1, 1) : bounds;
    }

    private int RowToVisibleIndex(Rect content, int y)
    {
        return IsOpen
            ? OptionListViewport.RowToVisibleIndex(content, y, MaxVisibleItems, _options.VisibleCount, _options.HighlightedVisibleIndex)
            : -1;
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

    private bool SetHighlightedVisibleIndex(int index)
    {
        if (index < 0 || index >= _options.VisibleCount || index == _options.HighlightedVisibleIndex)
        {
            return false;
        }

        while (_options.HighlightedVisibleIndex != index)
        {
            if (_options.HighlightedVisibleIndex < index)
            {
                _options.MoveNextVisible();
            }
            else
            {
                _options.MovePreviousVisible();
            }
        }

        return true;
    }
}
