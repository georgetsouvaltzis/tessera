using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record MenuBarItem(
    string Id,
    string Title,
    char Shortcut = '\0',
    IReadOnlyCollection<WidgetVisualState>? States = null);

public sealed class MenuBarComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<MenuBarItem> _items = [];
    private int _hoveredIndex = -1;

    public int SelectedIndex { get; private set; }

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public string? LastActivatedItemId { get; private set; }

    public long ActivationVersion { get; private set; }

    public KeyBinding NextItemKey { get; set; } = new("right/l", "next item", "right", "l");

    public KeyBinding PreviousItemKey { get; set; } = new("left/h", "previous item", "left", "h");

    public KeyBinding ActivateKey { get; set; } = new("enter/space", "activate", "enter", "space");

    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public IReadOnlyList<MenuBarItem> Items => _items;

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

public sealed record ContextMenuItem(
    string Id,
    string Title,
    IReadOnlyCollection<WidgetVisualState>? States = null);

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

public sealed class NumberInputComponent : IStatefulComponent, IFocusableComponent
{
    private bool _replaceOnNextCharacter = true;

    public TextInputModel Input { get; } = new();

    public TextInputKeyMap InputKeyMap { get; set; } = TextInputKeyMap.Default;

    public string Title { get; set; } = "Number Input";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public double Min { get; set; } = 0.0;

    public double Max { get; set; } = 100.0;

    public double Step { get; set; } = 1.0;

    public int Precision { get; set; } = 2;

    public double Value { get; private set; }

    public double? LastSubmittedValue { get; private set; }

    public KeyBinding IncreaseKey { get; set; } = new("up/+", "increase", "up", "+");

    public KeyBinding DecreaseKey { get; set; } = new("down/-", "decrease", "down", "-");

    public KeyBinding SubmitKey { get; set; } = new("enter", "submit", "enter");

    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public void SetValue(double value)
    {
        Value = Clamp(value);
        SyncInput();
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly)
        {
            return false;
        }

        if (message is KeyPressMsg key)
        {
            if (IncreaseKey.Matches(key))
            {
                var before = Value;
                Value = Clamp(Value + Step);
                SyncInput();
                return !AreClose(before, Value);
            }

            if (DecreaseKey.Matches(key))
            {
                var before = Value;
                Value = Clamp(Value - Step);
                SyncInput();
                return !AreClose(before, Value);
            }

            if (SubmitKey.Matches(key))
            {
                if (TryParseInput(out var parsed))
                {
                    Value = Clamp(parsed);
                    LastSubmittedValue = Value;
                    SyncInput();
                }

                return true;
            }

            if (_replaceOnNextCharacter
                && key.Code == KeyCode.Character
                && key.Modifiers == KeyModifiers.None
                && key.Text.Length == 1)
            {
                Input.SetValue(string.Empty);
                _replaceOnNextCharacter = false;
            }
        }

        var result = Input.Update(message, InputKeyMap);
        if (result.Changed)
        {
            _replaceOnNextCharacter = false;
        }

        if (!result.Changed && !result.Submitted)
        {
            return false;
        }

        if (result.Submitted && TryParseInput(out var submitted))
        {
            Value = Clamp(submitted);
            LastSubmittedValue = Value;
            SyncInput();
            return true;
        }

        if (result.Changed && TryParseInput(out var edited))
        {
            Value = Clamp(edited);
        }

        return result.Changed || result.Submitted;
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

        var states = ResolveStates();
        var frame = Input.BuildFrame(content.Width);
        canvas.WriteText(content.X, content.Y, StatePalette.Render(frame.Text, states), content.Width);
        if (content.Height > 1)
        {
            canvas.WriteText(content.X, content.Y + 1, StatePalette.Render($"value={FormatValue(Value)} range=[{FormatValue(Min)}, {FormatValue(Max)}]", states), content.Width);
        }
    }

    private List<WidgetVisualState> ResolveStates()
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

        return states;
    }

    private bool TryParseInput(out double value)
    {
        var text = Input.Value.Trim();
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }

        text = text.Replace(',', '.');
        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private void SyncInput()
    {
        Input.SetValue(FormatValue(Value));
        _replaceOnNextCharacter = true;
    }

    private string FormatValue(double value)
    {
        var precision = Math.Clamp(Precision, 0, 8);
        return value.ToString($"F{precision}", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }

    private double Clamp(double value)
    {
        if (Max <= Min)
        {
            return Min;
        }

        return Math.Clamp(value, Min, Max);
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.000001;
    }
}

public sealed class DatePickerComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private DateOnly? _hoveredDate;

    public string Title { get; set; } = "Date Picker";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public DateOnly SelectedDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    public DateOnly CurrentMonth { get; private set; } = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

    public DateOnly? LastCommittedDate { get; private set; }

    public KeyBinding PreviousDayKey { get; set; } = new("left/h", "previous day", "left", "h");

    public KeyBinding NextDayKey { get; set; } = new("right/l", "next day", "right", "l");

    public KeyBinding PreviousWeekKey { get; set; } = new("up/k", "previous week", "up", "k");

    public KeyBinding NextWeekKey { get; set; } = new("down/j", "next week", "down", "j");

    public KeyBinding PreviousMonthKey { get; set; } = new("pageup", "previous month", "pageup");

    public KeyBinding NextMonthKey { get; set; } = new("pagedown", "next month", "pagedown");

    public KeyBinding CommitKey { get; set; } = new("enter/space", "commit date", "enter", "space");

    public WidgetStatePalette DayStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetDate(DateOnly date)
    {
        SelectedDate = date;
        CurrentMonth = new DateOnly(date.Year, date.Month, 1);
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (PreviousDayKey.Matches(key))
        {
            SetDate(SelectedDate.AddDays(-1));
            return true;
        }

        if (NextDayKey.Matches(key))
        {
            SetDate(SelectedDate.AddDays(1));
            return true;
        }

        if (PreviousWeekKey.Matches(key))
        {
            SetDate(SelectedDate.AddDays(-7));
            return true;
        }

        if (NextWeekKey.Matches(key))
        {
            SetDate(SelectedDate.AddDays(7));
            return true;
        }

        if (PreviousMonthKey.Matches(key))
        {
            SetDate(SelectedDate.AddMonths(-1));
            return true;
        }

        if (NextMonthKey.Matches(key))
        {
            SetDate(SelectedDate.AddMonths(1));
            return true;
        }

        if (CommitKey.Matches(key))
        {
            LastCommittedDate = SelectedDate;
            return true;
        }

        return false;
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
                changed |= SetHoveredDate(null);
            }

            return changed;
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (wheel.Button == MouseButton.WheelUp)
            {
                SetDate(SelectedDate.AddMonths(-1));
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                SetDate(SelectedDate.AddMonths(1));
                changed = true;
            }
        }

        if (!TryGetDateAtPointer(content, message.X, message.Y, out var hovered))
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredDate(null);
            }

            return changed;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredDate(hovered);
            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredDate(hovered);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && hovered != SelectedDate)
            {
                SetDate(hovered);
                changed = true;
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

        if (content.IsEmpty || content.Height < 3)
        {
            return;
        }

        var monthLabel = $"{CurrentMonth:yyyy-MM}";
        canvas.WriteText(content.X, content.Y, monthLabel, content.Width);
        if (content.Height == 1)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y + 1, "Mo Tu We Th Fr Sa Su", content.Width);
        if (content.Height < 3)
        {
            return;
        }

        var first = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
        var day = 1;
        for (var row = 0; row < 6 && (content.Y + 2 + row) < content.Bottom; row++)
        {
            for (var col = 0; col < 7; col++)
            {
                var cell = row * 7 + col;
                if (cell < startOffset || day > daysInMonth)
                {
                    continue;
                }

                var x = content.X + (col * 3);
                if (x + 1 >= content.Right)
                {
                    continue;
                }

                var text = day.ToString().PadLeft(2, ' ');
                var date = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, day);
                var states = new List<WidgetVisualState>(5);
                if (date == SelectedDate)
                {
                    states.Add(WidgetVisualState.Selected);
                    states.Add(WidgetVisualState.Cursor);
                }

                if (Focused)
                {
                    states.Add(WidgetVisualState.Focused);
                }

                if (_hoveredDate.HasValue && _hoveredDate.Value == date)
                {
                    states.Add(WidgetVisualState.Hovered);
                }

                canvas.WriteText(x, content.Y + 2 + row, DayStatePalette.Render(text, states), Math.Min(2, content.Right - x));
                day++;
            }
        }
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

    private bool TryGetDateAtPointer(Rect content, int x, int y, out DateOnly date)
    {
        date = default;
        var row = y - (content.Y + 2);
        if (row < 0 || row >= 6)
        {
            return false;
        }

        var relativeX = x - content.X;
        if (relativeX < 0)
        {
            return false;
        }

        var col = relativeX / 3;
        if (col < 0 || col > 6)
        {
            return false;
        }

        var first = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
        var cell = (row * 7) + col;
        var day = cell - startOffset + 1;
        if (day < 1 || day > daysInMonth)
        {
            return false;
        }

        date = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, day);
        return true;
    }

    private bool SetHoveredDate(DateOnly? date)
    {
        if (_hoveredDate == date)
        {
            return false;
        }

        _hoveredDate = date;
        return true;
    }
}

public enum TimePickerField
{
    Hour = 0,
    Minute = 1,
    Second = 2,
}

public sealed class TimePickerComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private TimePickerField? _hoveredField;

    public string Title { get; set; } = "Time Picker";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public TimeOnly Value { get; private set; } = TimeOnly.FromDateTime(DateTime.UtcNow);

    public TimeOnly? LastCommittedTime { get; private set; }

    public TimePickerField ActiveField { get; private set; }

    public int HourStep { get; set; } = 1;

    public int MinuteStep { get; set; } = 1;

    public int SecondStep { get; set; } = 5;

    public KeyBinding NextFieldKey { get; set; } = new("right/l", "next field", "right", "l");

    public KeyBinding PreviousFieldKey { get; set; } = new("left/h", "previous field", "left", "h");

    public KeyBinding IncreaseKey { get; set; } = new("up/k", "increase", "up", "k");

    public KeyBinding DecreaseKey { get; set; } = new("down/j", "decrease", "down", "j");

    public KeyBinding CommitKey { get; set; } = new("enter/space", "commit time", "enter", "space");

    public WidgetStatePalette FieldStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetValue(TimeOnly time)
    {
        Value = time;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextFieldKey.Matches(key))
        {
            ActiveField = (TimePickerField)(((int)ActiveField + 1) % 3);
            return true;
        }

        if (PreviousFieldKey.Matches(key))
        {
            ActiveField = (TimePickerField)(((int)ActiveField + 2) % 3);
            return true;
        }

        if (IncreaseKey.Matches(key))
        {
            Adjust(1);
            return true;
        }

        if (DecreaseKey.Matches(key))
        {
            Adjust(-1);
            return true;
        }

        if (CommitKey.Matches(key))
        {
            LastCommittedTime = Value;
            return true;
        }

        return false;
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
                changed |= SetHoveredField(null);
            }

            return changed;
        }

        var field = FieldFromPointer(content, message.X, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredField(field);
            return changed;
        }

        if (message is MouseClickMsg or MouseReleaseMsg)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredField(field);
            }

            if (message.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && field.HasValue)
            {
                if (ActiveField != field.Value)
                {
                    ActiveField = field.Value;
                    changed = true;
                }
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (field.HasValue && ActiveField != field.Value)
            {
                ActiveField = field.Value;
                changed = true;
            }

            if (wheel.Button == MouseButton.WheelUp)
            {
                Adjust(1);
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                Adjust(-1);
                changed = true;
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

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var hour = RenderField(Value.Hour.ToString("D2"), TimePickerField.Hour);
        var minute = RenderField(Value.Minute.ToString("D2"), TimePickerField.Minute);
        var second = RenderField(Value.Second.ToString("D2"), TimePickerField.Second);
        canvas.WriteText(content.X, content.Y, $"{hour}:{minute}:{second}", content.Width);
    }

    private string RenderField(string value, TimePickerField field)
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

        if (field == ActiveField)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (_hoveredField.HasValue && _hoveredField.Value == field)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return FieldStatePalette.Render(value, states);
    }

    private void Adjust(int direction)
    {
        var delta = ActiveField switch
        {
            TimePickerField.Hour => TimeSpan.FromHours(HourStep * direction),
            TimePickerField.Minute => TimeSpan.FromMinutes(MinuteStep * direction),
            _ => TimeSpan.FromSeconds(SecondStep * direction),
        };
        Value = Value.Add(delta);
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

    private static TimePickerField? FieldFromPointer(Rect content, int x, int y)
    {
        if (y < content.Y || y >= content.Bottom)
        {
            return null;
        }

        var index = x - content.X;
        if (index < 0)
        {
            return null;
        }

        return index switch
        {
            <= 2 => TimePickerField.Hour,
            <= 5 => TimePickerField.Minute,
            <= 8 => TimePickerField.Second,
            _ => null,
        };
    }

    private bool SetHoveredField(TimePickerField? field)
    {
        if (_hoveredField == field)
        {
            return false;
        }

        _hoveredField = field;
        return true;
    }
}

public sealed class MarkdownViewerComponent : IStatefulComponent, IFocusableComponent
{
    private readonly ViewportModel _viewport = new();
    private string _markdown = string.Empty;

    public string Title { get; set; } = "Markdown";

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool Wrap
    {
        get => _viewport.Wrap;
        set => _viewport.SetWrap(value);
    }

    public bool ShowLineNumbers
    {
        get => _viewport.ShowLineNumbers;
        set => _viewport.ShowLineNumbers = value;
    }

    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public void SetMarkdown(string markdown)
    {
        _markdown = markdown ?? string.Empty;
        _viewport.SetContent(RenderMarkdown(_markdown));
    }

    public bool Update(IMessage message)
    {
        return _viewport.Update(message, ViewportKeyMap);
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
        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, lines[row], content.Width);
        }
    }

    private static string RenderMarkdown(string markdown)
    {
        var lines = markdown
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

        var output = new List<string>(lines.Length);
        var inCode = false;
        foreach (var raw in lines)
        {
            var line = raw ?? string.Empty;
            var trimmed = line.TrimStart();

            if (trimmed.StartsWith("```", StringComparison.Ordinal))
            {
                inCode = !inCode;
                output.Add(inCode ? "┌ code" : "└");
                continue;
            }

            if (inCode)
            {
                output.Add($"  {line}");
                continue;
            }

            if (trimmed.StartsWith("### ", StringComparison.Ordinal))
            {
                output.Add($"### {trimmed[4..]}");
                continue;
            }

            if (trimmed.StartsWith("## ", StringComparison.Ordinal))
            {
                output.Add($"## {trimmed[3..]}");
                continue;
            }

            if (trimmed.StartsWith("# ", StringComparison.Ordinal))
            {
                output.Add($"# {trimmed[2..].ToUpperInvariant()}");
                continue;
            }

            if (trimmed.StartsWith("- ", StringComparison.Ordinal) || trimmed.StartsWith("* ", StringComparison.Ordinal))
            {
                output.Add($"• {trimmed[2..]}");
                continue;
            }

            output.Add(line);
        }

        return string.Join('\n', output);
    }
}
