using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public enum LayoutContainerMode
{
    Vertical = 0,
    Horizontal = 1,
    Grid = 2,
}

public enum DialogResult
{
    None = 0,
    Accepted = 1,
    Dismissed = 2,
}

public sealed class LabelComponent : ICanvasComponent
{
    public string Text { get; set; } = string.Empty;

    public string? Title { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool DrawBorder
    {
        get => ShowBorder;
        set => ShowBorder = value;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Title ?? "Label");
            var content = clipped.Inset(1, 1);
            if (content.IsEmpty)
            {
                return;
            }

            DrawLines(canvas, content);
            return;
        }

        DrawLines(canvas, clipped);
    }

    private void DrawLines(Canvas canvas, Rect rect)
    {
        var lines = Text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

        var rows = Math.Min(rect.Height, lines.Length);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(rect.X, rect.Y + row, lines[row], rect.Width);
        }
    }
}

public sealed class ButtonComponent : IStatefulComponent
{
    private static readonly KeyBinding ActivateKey = new("enter/space", "activate", "enter", "space");

    public string Label { get; set; } = "Button";

    public string? Description { get; set; }

    public bool Focused { get; set; }

    public bool Enabled { get; set; } = true;

    public int PressCount { get; private set; }

    public bool WasPressed { get; private set; }

    public bool Update(IMessage message)
    {
        WasPressed = false;
        if (!Enabled || !Focused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (!ActivateKey.Matches(key))
        {
            return false;
        }

        PressCount++;
        WasPressed = true;
        return true;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var prefix = Focused ? "›" : " ";
        var state = Enabled ? string.Empty : " (disabled)";
        var text = $"{prefix} [{Label}]{state}";
        canvas.WriteText(clipped.X, clipped.Y, text, clipped.Width);
        if (!string.IsNullOrWhiteSpace(Description) && clipped.Height > 1)
        {
            canvas.WriteText(clipped.X, clipped.Y + 1, Description!, clipped.Width);
        }
    }
}

public sealed class TextInputComponent : IStatefulComponent
{
    public TextInputModel Input { get; } = new();

    public TextInputKeyMap KeyMap { get; set; } = TextInputKeyMap.Default;

    public KeyBinding CancelKey { get; set; } = new("esc", "cancel", "escape");

    public string Title { get; set; } = "Text Input";

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool ClearOnSubmit { get; set; }

    public bool ClearOnCancel { get; set; }

    public string LastSubmittedValue { get; private set; } = string.Empty;

    public string LastCancelledValue { get; private set; } = string.Empty;

    public int SubmitCount { get; private set; }

    public int CancelCount { get; private set; }

    public bool WasCancelled { get; private set; }

    public bool Update(IMessage message)
    {
        WasCancelled = false;
        if (message is KeyPressMsg key && CancelKey.Matches(key))
        {
            WasCancelled = true;
            LastCancelledValue = Input.Value;
            CancelCount++;
            if (ClearOnCancel)
            {
                Input.Clear();
            }

            return true;
        }

        var result = Input.Update(message, KeyMap);
        if (!result.Submitted)
        {
            return result.Changed;
        }

        LastSubmittedValue = Input.Value;
        SubmitCount++;
        if (ClearOnSubmit)
        {
            Input.Clear();
        }

        return true;
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

        var frame = Input.BuildFrame(content.Width);
        canvas.WriteText(content.X, content.Y, frame.Text, content.Width);
        if (content.Height > 1)
        {
            var submitted = SubmitCount == 0
                ? "submit: -"
                : $"submit: {SubmitCount}";
            canvas.WriteText(content.X, content.Y + 1, submitted, content.Width);
        }
    }
}

public sealed class TextAreaComponent : IStatefulComponent
{
    private readonly ViewportModel _viewport = new();

    public TextAreaComponent()
    {
        Input.Multiline = true;
    }

    public TextInputModel Input { get; } = new() { Multiline = true };

    public TextInputKeyMap InputKeyMap { get; set; } = TextInputKeyMap.Default;

    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public string Title { get; set; } = "Text Area";

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool ShowLineNumbers
    {
        get => _viewport.ShowLineNumbers;
        set => _viewport.ShowLineNumbers = value;
    }

    public bool Wrap
    {
        get => _viewport.Wrap;
        set => _viewport.SetWrap(value);
    }

    public bool Update(IMessage message)
    {
        var changed = false;
        var update = Input.Update(message, InputKeyMap);
        if (update.Changed)
        {
            SyncViewport();
            changed = true;
        }

        if (_viewport.Update(message, ViewportKeyMap))
        {
            changed = true;
        }

        _viewport.HighlightVisualLine = CursorLineIndex();
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

        _viewport.Resize(content.Width, content.Height);
        _viewport.HighlightVisualLine = CursorLineIndex();
        SyncViewport();

        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, lines[row], content.Width);
        }
    }

    private void SyncViewport()
    {
        _viewport.SetContent(Input.Value);
    }

    private int CursorLineIndex()
    {
        if (Input.Cursor <= 0)
        {
            return 0;
        }

        var cursor = Math.Min(Input.Cursor, Input.Value.Length);
        var lines = 0;
        for (var i = 0; i < cursor; i++)
        {
            if (Input.Value[i] == '\n')
            {
                lines++;
            }
        }

        return lines;
    }
}

public sealed class ListComponent<T> : IStatefulComponent, IMouseStatefulComponent
{
    private int? _hoveredFilteredIndex;

    public ListComponent(IEnumerable<T> items, Func<T, string> toText)
    {
        Model = new ListModel<T>(items, toText);
    }

    public ListModel<T> Model { get; }

    public string Title { get; set; } = "List";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public Func<T, IReadOnlyCollection<WidgetVisualState>?>? ItemStateResolver { get; set; }

    public ListKeyMap KeyMap { get; set; } = ListKeyMap.Default;

    public bool Update(IMessage message)
    {
        if (Disabled)
        {
            return false;
        }

        return Model.Update(message, KeyMap);
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled)
        {
            return false;
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return false;
        }

        if (!content.Contains(message.X, message.Y) && message is not MouseWheelMsg)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                return SetHoveredFilteredIndex(null);
            }

            return false;
        }

        Model.PageSize = Math.Max(1, content.Height);
        var hoverChanged = message is MouseMotionMsg or MouseClickMsg
            ? SetHoveredByPointer(message.X, message.Y, content)
            : false;

        if (message is MouseMotionMsg)
        {
            return hoverChanged;
        }

        if (message is MouseWheelMsg wheel)
        {
            return hoverChanged | Model.Update(wheel, KeyMap);
        }

        if (message is not MouseClickMsg { Button: MouseButton.Left } click)
        {
            return false;
        }

        if (!content.Contains(click.X, click.Y))
        {
            return false;
        }

        var row = click.Y - content.Y;
        if (row < 0 || row >= content.Height)
        {
            return false;
        }

        var visibleRows = Model.VisibleRows();
        if (row >= visibleRows.Count)
        {
            return false;
        }

        return hoverChanged | Model.SelectFilteredIndex(visibleRows[row].Index);
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

        Model.PageSize = Math.Max(1, content.Height);
        var rows = Model.VisibleRows();
        if (rows.Count == 0 && content.Height > 0)
        {
            canvas.WriteText(content.X, content.Y, ItemStatePalette.Render("(empty)", ResolveBaseStates(isEmpty: true)), content.Width);
            return;
        }

        for (var row = 0; row < rows.Count && row < content.Height; row++)
        {
            var visible = rows[row];
            var marker = visible.Selected
                ? "›"
                : _hoveredFilteredIndex == visible.Index ? "▸" : " ";
            var text = $"{marker} {Model.LabelFor(visible.Item)}";
            canvas.WriteText(content.X, content.Y + row, ItemStatePalette.Render(text, ResolveRowStates(visible)), content.Width);
        }
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveBaseStates(bool isEmpty = false)
    {
        var states = new List<WidgetVisualState>(4);
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

        if (isEmpty)
        {
            states.Add(WidgetVisualState.Empty);
        }

        return states;
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveRowStates(ListRow<T> visible)
    {
        var states = new List<WidgetVisualState>(6);
        states.AddRange(ResolveBaseStates());
        if (visible.Selected)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (_hoveredFilteredIndex == visible.Index)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (ItemStateResolver?.Invoke(visible.Item) is { } custom)
        {
            states.AddRange(custom);
        }

        return states;
    }

    private Rect ResolveContentRect(Rect rect)
    {
        if (ShowBorder)
        {
            return rect.Inset(1, 1);
        }

        return rect;
    }

    private bool SetHoveredByPointer(int x, int y, Rect content)
    {
        if (!content.Contains(x, y))
        {
            return SetHoveredFilteredIndex(null);
        }

        var row = y - content.Y;
        if (row < 0 || row >= content.Height)
        {
            return SetHoveredFilteredIndex(null);
        }

        var rows = Model.VisibleRows();
        if (row >= rows.Count)
        {
            return SetHoveredFilteredIndex(null);
        }

        return SetHoveredFilteredIndex(rows[row].Index);
    }

    private bool SetHoveredFilteredIndex(int? filteredIndex)
    {
        if (_hoveredFilteredIndex == filteredIndex)
        {
            return false;
        }

        _hoveredFilteredIndex = filteredIndex;
        return true;
    }
}

public sealed class DropdownComponent : IStatefulComponent, IMouseStatefulComponent
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

public sealed class ComboboxComponent : IStatefulComponent, IMouseStatefulComponent
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

public sealed class TableComponent : IStatefulComponent, IMouseStatefulComponent
{
    public TableComponent(IReadOnlyList<string> headers)
    {
        Inner = new SortableTableComponent(headers);
    }

    public SortableTableComponent Inner { get; }

    public bool Focused { get; set; }

    public bool ShowBorder
    {
        get => Inner.ShowBorder;
        set => Inner.ShowBorder = value;
    }

    public string Title
    {
        get => Inner.Title;
        set => Inner.Title = value;
    }

    public void SetRows(IEnumerable<IReadOnlyList<string>> rows)
    {
        Inner.SetRows(rows);
    }

    public bool Update(IMessage message)
    {
        return Inner.Update(message);
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        return Inner.UpdateMouse(message, bounds);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var original = Inner.Title;
        Inner.Title = Focused ? $"{original} *" : original.Replace(" *", string.Empty, StringComparison.Ordinal);
        Inner.Render(canvas, rect);
        Inner.Title = original;
    }
}

public sealed class ProgressBarComponent : IStatefulComponent
{
    public double Value { get; private set; }

    public string Title { get; set; } = "Progress";

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public double Step { get; set; } = 0.05;

    public KeyBinding DecreaseKey { get; set; } = new("left/-", "decrease", "left", "-");

    public KeyBinding IncreaseKey { get; set; } = new("right/+", "increase", "right", "+");

    public bool Update(IMessage message)
    {
        if (message is not KeyPressMsg key || !Focused)
        {
            return false;
        }

        if (DecreaseKey.Matches(key))
        {
            SetValue(Value - Step);
            return true;
        }

        if (IncreaseKey.Matches(key))
        {
            SetValue(Value + Step);
            return true;
        }

        return false;
    }

    public void SetValue(double value)
    {
        Value = Math.Clamp(value, 0.0, 1.0);
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

        var percent = (int)Math.Round(Value * 100, MidpointRounding.AwayFromZero);
        Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y, content.Width, 1), Value, $"{percent}%");
    }
}

public sealed class StatusBarComponent : ICanvasComponent
{
    public string LeftText { get; set; } = string.Empty;

    public string RightText { get; set; } = string.Empty;

    public UiTheme Theme { get; set; } = new();

    public void Render(Canvas canvas, Rect rect)
    {
        UiWidgets.DrawStatusBar(canvas, rect, LeftText, RightText, Theme);
    }
}

public sealed class LogViewerComponent : IStatefulComponent
{
    private readonly ViewportModel _viewport = new();
    private readonly List<string> _entries = [];

    public LogViewerComponent()
    {
        _viewport.SetWrap(false);
    }

    public string Title { get; set; } = "Logs";

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool AutoScroll { get; set; } = true;

    public bool Paused { get; private set; }

    public string Filter { get; private set; } = string.Empty;

    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public KeyBinding TogglePauseKey { get; set; } = new("p", "toggle pause", "p");

    public KeyBinding ClearKey { get; set; } = new("c", "clear", "c");

    public int Count => _entries.Count;

    public void Append(string line)
    {
        if (Paused)
        {
            return;
        }

        _entries.Add(line);
        RefreshViewport();
        if (AutoScroll)
        {
            _viewport.ScrollToBottom();
        }
    }

    public void Clear()
    {
        _entries.Clear();
        RefreshViewport();
    }

    public void SetFilter(string filter)
    {
        Filter = filter ?? string.Empty;
        RefreshViewport();
    }

    public bool Update(IMessage message)
    {
        if (message is KeyPressMsg key)
        {
            if (TogglePauseKey.Matches(key))
            {
                Paused = !Paused;
                return true;
            }

            if (ClearKey.Matches(key))
            {
                Clear();
                return true;
            }
        }

        return _viewport.Update(message, ViewportKeyMap);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Focused
            ? $"{Title} *"
            : Title;
        if (Paused)
        {
            title += " [paused]";
        }

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(clipped, title);
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

        _viewport.Resize(content.Width, content.Height);
        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, lines[row], content.Width);
        }
    }

    private void RefreshViewport()
    {
        var visible = string.IsNullOrWhiteSpace(Filter)
            ? _entries
            : _entries.Where(line => line.Contains(Filter, StringComparison.OrdinalIgnoreCase)).ToList();
        _viewport.SetContent(string.Join("\n", visible));
    }
}

public sealed class DialogComponent : IStatefulComponent
{
    public string Title { get; set; } = "Dialog";

    public IReadOnlyList<string> Lines { get; set; } = ["Confirm?"];

    public bool Visible { get; set; }

    public bool Focused { get; set; }

    public BorderStyle BorderStyle { get; set; } = BorderStyle.Rounded;

    public UiTheme Theme { get; set; } = new();

    public DialogResult LastResult { get; private set; }

    public KeyBinding AcceptKey { get; set; } = new("enter/space", "accept", "enter", "space");

    public KeyBinding DismissKey { get; set; } = new("esc", "dismiss", "escape");

    public bool Update(IMessage message)
    {
        if (!Visible || !Focused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (DismissKey.Matches(key))
        {
            Visible = false;
            LastResult = DialogResult.Dismissed;
            return true;
        }

        if (AcceptKey.Matches(key))
        {
            Visible = false;
            LastResult = DialogResult.Accepted;
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!Visible)
        {
            return;
        }

        var modal = new ModalComponent
        {
            Visible = true,
            Title = Title,
            Lines = Lines,
            BorderStyle = BorderStyle,
            Theme = Theme,
        };
        modal.Render(canvas, rect);
    }
}

public sealed class LayoutContainerComponent : IStatefulComponent, IMouseStatefulComponent
{
    private readonly List<(ICanvasComponent Component, int Weight)> _children = [];
    private bool _draggingSplit;

    public LayoutContainerMode Mode { get; set; } = LayoutContainerMode.Vertical;

    public int GridRows { get; set; } = 1;

    public int GridColumns { get; set; } = 1;

    public bool EnableMouseInteractions { get; set; } = true;

    public bool ClickToFocusChildren { get; set; } = true;

    public bool EnableMouseResize { get; set; } = true;

    public int SplitterHitThickness { get; set; } = 1;

    public int MinPrimarySize { get; set; } = 8;

    public int MinSecondarySize { get; set; } = 8;

    public int? PrimarySize { get; private set; }

    public IReadOnlyList<(ICanvasComponent Component, int Weight)> Children => _children;

    public void Clear()
    {
        _children.Clear();
        _draggingSplit = false;
        PrimarySize = null;
    }

    public void Add(ICanvasComponent component, int weight = 1)
    {
        _children.Add((component, Math.Max(1, weight)));
    }

    public void SetPrimarySize(int size)
    {
        PrimarySize = Math.Max(0, size);
    }

    public void ClearPrimarySize()
    {
        PrimarySize = null;
    }

    public bool Update(IMessage message)
    {
        var changed = false;
        foreach (var child in _children)
        {
            if (child.Component is IStatefulComponent stateful)
            {
                changed |= stateful.Update(message);
            }
        }

        return changed;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (!EnableMouseInteractions || _children.Count == 0 || bounds.IsEmpty)
        {
            return false;
        }

        var rects = BuildChildRects(bounds);
        var changed = HandleSplitMouse(message, bounds, rects, out var splitConsumed);
        if (splitConsumed)
        {
            return changed;
        }

        var targetIndex = FindTopMostChild(rects, message.X, message.Y);
        if (targetIndex < 0 || targetIndex >= _children.Count)
        {
            return changed;
        }

        if (ClickToFocusChildren && message is MouseClickMsg { Button: MouseButton.Left })
        {
            changed |= SetFocusedChild(targetIndex);
        }

        var child = _children[targetIndex].Component;
        if (child is IMouseStatefulComponent mouseStateful)
        {
            changed |= mouseStateful.UpdateMouse(message, rects[targetIndex]);
            return changed;
        }

        if (child is IStatefulComponent stateful)
        {
            changed |= stateful.Update(message);
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (_children.Count == 0)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var rects = BuildChildRects(clipped);
        var count = Math.Min(_children.Count, rects.Count);
        for (var i = 0; i < count; i++)
        {
            _children[i].Component.Render(canvas, rects[i]);
        }
    }

    private List<Rect> BuildChildRects(Rect rect)
    {
        return Mode switch
        {
            LayoutContainerMode.Horizontal => BuildHorizontalRects(rect),
            LayoutContainerMode.Grid => BuildGridRects(rect),
            _ => BuildVerticalRects(rect),
        };
    }

    private List<Rect> BuildVerticalRects(Rect rect)
    {
        var rects = new List<Rect>(_children.Count);
        if (_children.Count == 0)
        {
            return rects;
        }

        if (_children.Count == 2 && PrimarySize.HasValue)
        {
            var (first, second) = Layout.SplitHorizontal(
                rect,
                PrimarySize.Value,
                minFirst: Math.Max(0, MinPrimarySize),
                minSecond: Math.Max(0, MinSecondarySize));
            rects.Add(first);
            rects.Add(second);
            return rects;
        }

        var totalWeight = _children.Sum(entry => entry.Weight);
        var y = rect.Y;
        var consumed = 0;
        for (var i = 0; i < _children.Count; i++)
        {
            var remainingHeight = rect.Height - consumed;
            if (remainingHeight <= 0)
            {
                break;
            }

            var planned = i == _children.Count - 1
                ? remainingHeight
                : Math.Max(1, (rect.Height * _children[i].Weight) / Math.Max(1, totalWeight));
            var h = Math.Min(remainingHeight, planned);
            rects.Add(new Rect(rect.X, y, rect.Width, h));
            y += h;
            consumed += h;
        }

        return rects;
    }

    private List<Rect> BuildHorizontalRects(Rect rect)
    {
        var rects = new List<Rect>(_children.Count);
        if (_children.Count == 0)
        {
            return rects;
        }

        if (_children.Count == 2 && PrimarySize.HasValue)
        {
            var (first, second) = Layout.SplitVertical(
                rect,
                PrimarySize.Value,
                minFirst: Math.Max(0, MinPrimarySize),
                minSecond: Math.Max(0, MinSecondarySize));
            rects.Add(first);
            rects.Add(second);
            return rects;
        }

        var totalWeight = _children.Sum(entry => entry.Weight);
        var x = rect.X;
        var consumed = 0;
        for (var i = 0; i < _children.Count; i++)
        {
            var remainingWidth = rect.Width - consumed;
            if (remainingWidth <= 0)
            {
                break;
            }

            var planned = i == _children.Count - 1
                ? remainingWidth
                : Math.Max(1, (rect.Width * _children[i].Weight) / Math.Max(1, totalWeight));
            var w = Math.Min(remainingWidth, planned);
            rects.Add(new Rect(x, rect.Y, w, rect.Height));
            x += w;
            consumed += w;
        }

        return rects;
    }

    private List<Rect> BuildGridRects(Rect rect)
    {
        var rows = Math.Max(1, GridRows);
        var columns = Math.Max(1, GridColumns);
        var cells = Layout.Grid(rect, rows, columns);
        var count = Math.Min(cells.Length, _children.Count);
        var rects = new List<Rect>(count);
        for (var i = 0; i < count; i++)
        {
            rects.Add(cells[i]);
        }

        return rects;
    }

    private bool HandleSplitMouse(MouseMsg message, Rect bounds, IReadOnlyList<Rect> rects, out bool consumed)
    {
        consumed = false;
        if (!EnableMouseResize || Mode == LayoutContainerMode.Grid || _children.Count != 2 || rects.Count < 2)
        {
            return false;
        }

        if (message is MouseReleaseMsg { Button: MouseButton.Left } && _draggingSplit)
        {
            _draggingSplit = false;
            consumed = true;
            return true;
        }

        if (!TryGetSplitterHitRect(bounds, rects[0], out var splitterHit))
        {
            return false;
        }

        if (message is MouseClickMsg { Button: MouseButton.Left } click
            && splitterHit.Contains(click.X, click.Y))
        {
            _draggingSplit = true;
            consumed = true;
            return true;
        }

        if (message is MouseMotionMsg motion && _draggingSplit)
        {
            consumed = true;
            return ApplyDraggedPrimarySize(bounds, motion.X, motion.Y);
        }

        return false;
    }

    private bool ApplyDraggedPrimarySize(Rect bounds, int x, int y)
    {
        var totalSize = Mode == LayoutContainerMode.Horizontal
            ? bounds.Width
            : bounds.Height;
        if (totalSize <= 0)
        {
            return false;
        }

        var requested = Mode == LayoutContainerMode.Horizontal
            ? x - bounds.X
            : y - bounds.Y;
        var minFirst = Math.Clamp(MinPrimarySize, 0, totalSize);
        var maxSecond = Math.Max(0, totalSize - minFirst);
        var minSecond = Math.Clamp(MinSecondarySize, 0, maxSecond);
        var clamped = Math.Clamp(requested, minFirst, totalSize - minSecond);

        if (PrimarySize == clamped)
        {
            return false;
        }

        PrimarySize = clamped;
        return true;
    }

    private bool TryGetSplitterHitRect(Rect bounds, Rect firstRect, out Rect splitterHit)
    {
        splitterHit = default;
        var thickness = Math.Max(1, SplitterHitThickness);
        if (Mode == LayoutContainerMode.Horizontal)
        {
            var center = firstRect.Right;
            var start = center - (thickness / 2);
            splitterHit = Rect.Intersect(new Rect(start, bounds.Y, thickness, bounds.Height), bounds);
            return !splitterHit.IsEmpty;
        }

        if (Mode == LayoutContainerMode.Vertical)
        {
            var center = firstRect.Bottom;
            var start = center - (thickness / 2);
            splitterHit = Rect.Intersect(new Rect(bounds.X, start, bounds.Width, thickness), bounds);
            return !splitterHit.IsEmpty;
        }

        return false;
    }

    private static int FindTopMostChild(IReadOnlyList<Rect> rects, int x, int y)
    {
        for (var i = rects.Count - 1; i >= 0; i--)
        {
            if (rects[i].Contains(x, y))
            {
                return i;
            }
        }

        return -1;
    }

    private bool SetFocusedChild(int index)
    {
        var changed = false;
        for (var i = 0; i < _children.Count; i++)
        {
            changed |= TrySetFocused(_children[i].Component, i == index);
        }

        return changed;
    }

    private static bool TrySetFocused(ICanvasComponent component, bool focused)
    {
        var property = component.GetType().GetProperty("Focused");
        if (property is null || property.PropertyType != typeof(bool) || !property.CanWrite)
        {
            return false;
        }

        if (property.GetValue(component) is bool current && current == focused)
        {
            return false;
        }

        property.SetValue(component, focused);
        return true;
    }
}
