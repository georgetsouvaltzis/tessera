using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class BadgeComponent : ICanvasComponent
{
    public string Text { get; set; } = "Badge";

    public WidgetVisualState State { get; set; } = WidgetVisualState.Default;

    public WidgetStatePalette Palette { get; } = WidgetStatePalette.CreateDefault();

    public bool ShowBrackets { get; set; } = true;

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var label = ShowBrackets
            ? $"[{Text}]"
            : Text;
        canvas.WriteText(clipped.X, clipped.Y, Palette.Render(label, State), clipped.Width);
    }
}

public sealed class ToggleSwitchComponent : IStatefulComponent, IMouseStatefulComponent
{
    private bool _hovered;

    public string Title { get; set; } = "Toggle";

    public string OnText { get; set; } = "ON";

    public string OffText { get; set; } = "OFF";

    public bool Value { get; private set; }

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public KeyBinding ToggleKey { get; set; } = new("enter/space", "toggle", "enter", "space");

    public KeyBinding TurnOnKey { get; set; } = new("right", "on", "right");

    public KeyBinding TurnOffKey { get; set; } = new("left", "off", "left");

    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetValue(bool value)
    {
        Value = value;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (ToggleKey.Matches(key))
        {
            Value = !Value;
            return true;
        }

        if (TurnOnKey.Matches(key))
        {
            var changed = !Value;
            Value = true;
            return changed;
        }

        if (TurnOffKey.Matches(key))
        {
            var changed = Value;
            Value = false;
            return changed;
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
                changed |= SetHovered(false);
            }

            return changed;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHovered(true);
            return changed;
        }

        if (message is MouseClickMsg && InteractionProfile.HoverOnClick)
        {
            changed |= SetHovered(true);
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (wheel.Button == MouseButton.WheelUp)
            {
                var was = Value;
                Value = true;
                changed |= !was;
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                var was = Value;
                Value = false;
                changed |= was;
            }
        }

        if (message is MouseClickMsg { Button: MouseButton.Left } && InteractionProfile.ActivateOnClick)
        {
            Value = !Value;
            changed = true;
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

        var states = ResolveStates();
        if (Value)
        {
            states.Add(WidgetVisualState.Checked);
            states.Add(WidgetVisualState.Success);
        }
        else
        {
            states.Add(WidgetVisualState.Unchecked);
        }

        var label = Value ? OnText : OffText;
        canvas.WriteText(content.X, content.Y, StatePalette.Render($"<{label}>", states), content.Width);
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

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

    private bool SetHovered(bool hovered)
    {
        if (_hovered == hovered)
        {
            return false;
        }

        _hovered = hovered;
        return true;
    }
}

public sealed class SliderComponent : IStatefulComponent, IMouseStatefulComponent
{
    private bool _hovered;
    private bool _dragging;

    public string Title { get; set; } = "Slider";

    public double Min { get; set; } = 0.0;

    public double Max { get; set; } = 100.0;

    public double Value { get; private set; }

    public double Step { get; set; } = 1.0;

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public KeyBinding DecreaseKey { get; set; } = new("left/-", "decrease", "left", "-");

    public KeyBinding IncreaseKey { get; set; } = new("right/+", "increase", "right", "+");

    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetValue(double value)
    {
        Value = Clamp(value);
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (DecreaseKey.Matches(key))
        {
            var previous = Value;
            Value = Clamp(Value - Step);
            return !AreClose(previous, Value);
        }

        if (IncreaseKey.Matches(key))
        {
            var previous = Value;
            Value = Clamp(Value + Step);
            return !AreClose(previous, Value);
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

        var changed = false;
        if (message is MouseReleaseMsg { Button: MouseButton.Left })
        {
            var wasDragging = _dragging;
            _dragging = false;
            changed |= SetHovered(content.Contains(message.X, message.Y));
            return changed || wasDragging;
        }

        if (message is MouseMotionMsg motion && _dragging && motion.Button == MouseButton.Left)
        {
            changed |= SetHovered(content.Contains(motion.X, motion.Y));
            changed |= SetValueFromPointer(motion.X, content);
            return changed;
        }

        var inside = content.Contains(message.X, message.Y);
        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHovered(false);
            }

            return changed;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHovered(true);
            return changed;
        }

        if (message is MouseClickMsg && InteractionProfile.HoverOnClick)
        {
            changed |= SetHovered(true);
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            var before = Value;
            if (wheel.Button == MouseButton.WheelUp)
            {
                Value = Clamp(Value + Step);
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                Value = Clamp(Value - Step);
            }

            changed |= !AreClose(before, Value);
        }

        if (message is MouseClickMsg { Button: MouseButton.Left } click
            && InteractionProfile.ActivateOnClick
            && IsPointerOnBarRow(content, click.Y))
        {
            _dragging = true;
            changed |= SetValueFromPointer(click.X, content);
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

        var states = ResolveStates();
        var label = $"{Value:0.##} / {Max:0.##}";
        canvas.WriteText(content.X, content.Y, StatePalette.Render(label, states), content.Width);
        if (content.Height > 1)
        {
            var normalized = Normalize();
            Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y + 1, content.Width, 1), normalized);
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

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
    }

    private double Normalize()
    {
        var range = Max - Min;
        if (range <= 0.0)
        {
            return 0.0;
        }

        return Math.Clamp((Value - Min) / range, 0.0, 1.0);
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

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

    private static bool IsPointerOnBarRow(Rect content, int y)
    {
        var barY = content.Height > 1
            ? content.Y + 1
            : content.Y;
        return y == barY;
    }

    private bool SetValueFromPointer(int x, Rect content)
    {
        if (Max <= Min)
        {
            return false;
        }

        var barX = content.X + 1;
        var barWidth = Math.Max(1, content.Width - 2);
        var clampedX = Math.Clamp(x, barX, barX + barWidth - 1);
        var normalized = barWidth == 1
            ? 1.0
            : (double)(clampedX - barX) / Math.Max(1, barWidth - 1);
        var before = Value;
        Value = Clamp(Min + ((Max - Min) * normalized));
        return !AreClose(before, Value);
    }

    private bool SetHovered(bool hovered)
    {
        if (_hovered == hovered)
        {
            return false;
        }

        _hovered = hovered;
        return true;
    }
}

public sealed class SpinnerComponent : IStatefulComponent, IMouseStatefulComponent
{
    private IReadOnlyList<string> _frames = ["|", "/", "-", "\\"];
    private bool _hovered;

    public string Title { get; set; } = "Spinner";

    public bool Focused { get; set; }

    public bool Running { get; private set; } = true;

    public bool ShowBorder { get; set; } = true;

    public int FrameIndex { get; private set; }

    public string Label { get; set; } = "loading";

    public KeyBinding AdvanceKey { get; set; } = new("right/space", "advance", "right", "space");

    public KeyBinding ToggleRunKey { get; set; } = new("enter", "toggle running", "enter");

    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetFrames(IEnumerable<string> frames)
    {
        var list = frames
            .Where(frame => !string.IsNullOrWhiteSpace(frame))
            .ToList();
        if (list.Count == 0)
        {
            return;
        }

        _frames = list;
        FrameIndex = Math.Clamp(FrameIndex, 0, _frames.Count - 1);
    }

    public void Advance()
    {
        if (_frames.Count == 0)
        {
            return;
        }

        FrameIndex = (FrameIndex + 1) % _frames.Count;
    }

    public void SetRunning(bool running)
    {
        Running = running;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (ToggleRunKey.Matches(key))
        {
            Running = !Running;
            return true;
        }

        if (AdvanceKey.Matches(key))
        {
            if (Running)
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        var content = ResolveContentRect(bounds);
        if (_frames.Count == 0 || content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(message.X, message.Y);
        var changed = false;
        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHovered(false);
            }

            return changed;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHovered(true);
            return changed;
        }

        if (message is MouseClickMsg && InteractionProfile.HoverOnClick)
        {
            changed |= SetHovered(true);
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && Running)
        {
            if (wheel.Button is MouseButton.WheelUp or MouseButton.WheelDown)
            {
                Advance();
                changed = true;
            }
        }

        if (message is MouseClickMsg { Button: MouseButton.Left } && InteractionProfile.ActivateOnClick)
        {
            Running = !Running;
            changed = true;
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || _frames.Count == 0)
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

        var states = new List<WidgetVisualState>(3);
        if (Focused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (Running)
        {
            states.Add(WidgetVisualState.Loading);
        }
        else
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        canvas.WriteText(content.X, content.Y, StatePalette.Render($"{_frames[FrameIndex]} {Label}", states), content.Width);
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

    private bool SetHovered(bool hovered)
    {
        if (_hovered == hovered)
        {
            return false;
        }

        _hovered = hovered;
        return true;
    }
}

public sealed record CommandPaletteItem(
    string Id,
    string Title,
    string Description = "",
    IReadOnlyCollection<WidgetVisualState>? States = null);

public sealed class CommandPaletteComponent : IStatefulComponent, IMouseStatefulComponent
{
    private readonly List<CommandPaletteItem> _items = [];
    private readonly List<int> _filtered = [];
    private int _selectedFilteredIndex;
    private int _hoveredFilteredIndex = -1;

    public TextInputModel Query { get; } = new();

    public TextInputKeyMap QueryKeyMap { get; set; } = TextInputKeyMap.Default;

    public string Title { get; set; } = "Command Palette";

    public bool Focused { get; set; }

    public bool IsOpen { get; private set; }

    public int MaxVisibleItems { get; set; } = 8;

    public string? LastExecutedItemId { get; private set; }

    public KeyBinding OpenKey { get; set; } = new("ctrl+p", "open", "ctrl+p");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ExecuteKey { get; set; } = new("enter", "execute", "enter");

    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetItems(IEnumerable<CommandPaletteItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
        RefreshFiltered();
    }

    public void Open()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;
        Query.Clear();
        RefreshFiltered();
    }

    public void Close()
    {
        IsOpen = false;
    }

    public bool Update(IMessage message)
    {
        if (!Focused)
        {
            return false;
        }

        if (!IsOpen)
        {
            if (message is KeyPressMsg openKey && OpenKey.Matches(openKey))
            {
                Open();
                return true;
            }

            return false;
        }

        if (message is KeyPressMsg key)
        {
            if (CloseKey.Matches(key))
            {
                Close();
                return true;
            }

            if (NextItemKey.Matches(key) && _filtered.Count > 0)
            {
                _selectedFilteredIndex = (_selectedFilteredIndex + 1) % _filtered.Count;
                return true;
            }

            if (PreviousItemKey.Matches(key) && _filtered.Count > 0)
            {
                _selectedFilteredIndex = (_selectedFilteredIndex + _filtered.Count - 1) % _filtered.Count;
                return true;
            }

            if (ExecuteKey.Matches(key))
            {
                return ExecuteSelected();
            }
        }

        var inputResult = Query.Update(message, QueryKeyMap);
        if (inputResult.Changed)
        {
            RefreshFiltered();
            return true;
        }

        if (inputResult.Submitted)
        {
            return ExecuteSelected();
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (!IsOpen || !TryResolveModal(bounds, out var modal, out var content))
        {
            return false;
        }

        var insideModal = modal.Contains(message.X, message.Y);
        var changed = false;
        if (!insideModal)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredFilteredIndex(-1);
            }

            if (message is MouseClickMsg { Button: MouseButton.Left } && InteractionProfile.ActivateOnClick)
            {
                Close();
                changed = true;
            }

            return changed;
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && _filtered.Count > 0)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                _selectedFilteredIndex = (_selectedFilteredIndex + 1) % _filtered.Count;
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                _selectedFilteredIndex = (_selectedFilteredIndex + _filtered.Count - 1) % _filtered.Count;
                changed = true;
            }
        }

        if (!content.Contains(message.X, message.Y))
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredFilteredIndex(-1);
            }

            return changed;
        }

        var hovered = RowToFilteredIndex(content, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredFilteredIndex(hovered);
            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredFilteredIndex(hovered);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && hovered >= 0)
            {
                _selectedFilteredIndex = hovered;
                changed |= ExecuteSelected();
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!IsOpen)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 24 || clipped.Height < 6)
        {
            return;
        }

        var modalWidth = Math.Min(clipped.Width - 2, Math.Max(24, clipped.Width * 2 / 3));
        var modalHeight = Math.Min(clipped.Height - 2, Math.Max(8, clipped.Height * 2 / 3));
        var modalX = clipped.X + (clipped.Width - modalWidth) / 2;
        var modalY = clipped.Y + (clipped.Height - modalHeight) / 2;
        var modal = new Rect(modalX, modalY, modalWidth, modalHeight);

        canvas.DrawBox(modal, Title, BorderStyle.Rounded);
        var content = modal.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var queryWidth = Math.Max(1, content.Width - 2);
        var frame = Query.BuildFrame(queryWidth);
        canvas.WriteText(content.X, content.Y, $"> {frame.Text}", content.Width);
        if (content.Height <= 1)
        {
            return;
        }

        if (_filtered.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, ItemStatePalette.Render("(no commands)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = ComputeWindowStart(_selectedFilteredIndex, visibleRows, _filtered.Count);
        var end = Math.Min(_filtered.Count, start + visibleRows);
        var row = 0;
        for (var i = start; i < end; i++, row++)
        {
            var index = _filtered[i];
            var item = _items[index];
            var marker = i == _selectedFilteredIndex ? ">" : " ";
            var summary = string.IsNullOrWhiteSpace(item.Description)
                ? item.Title
                : $"{item.Title} - {item.Description}";

            var states = new List<WidgetVisualState>(4);
            if (i == _selectedFilteredIndex)
            {
                states.Add(WidgetVisualState.Cursor);
                states.Add(WidgetVisualState.Selected);
            }

            if (i == _hoveredFilteredIndex)
            {
                states.Add(WidgetVisualState.Hovered);
            }

            if (item.States is not null)
            {
                states.AddRange(item.States);
            }

            canvas.WriteText(content.X, content.Y + 1 + row, ItemStatePalette.Render($"{marker} {summary}", states), content.Width);
        }
    }

    private bool ExecuteSelected()
    {
        if (_filtered.Count == 0)
        {
            Close();
            return true;
        }

        var selected = Math.Clamp(_selectedFilteredIndex, 0, _filtered.Count - 1);
        LastExecutedItemId = _items[_filtered[selected]].Id;
        Close();
        return true;
    }

    private void RefreshFiltered()
    {
        _filtered.Clear();
        var filter = Query.Value.Trim();
        for (var i = 0; i < _items.Count; i++)
        {
            var include = filter.Length == 0
                || _items[i].Title.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || _items[i].Description.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || _items[i].Id.Contains(filter, StringComparison.OrdinalIgnoreCase);
            if (include)
            {
                _filtered.Add(i);
            }
        }

        if (_filtered.Count == 0)
        {
            _selectedFilteredIndex = 0;
            _hoveredFilteredIndex = -1;
            return;
        }

        _selectedFilteredIndex = Math.Clamp(_selectedFilteredIndex, 0, _filtered.Count - 1);
        if (_hoveredFilteredIndex >= _filtered.Count)
        {
            _hoveredFilteredIndex = _filtered.Count - 1;
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

    private bool TryResolveModal(Rect bounds, out Rect modal, out Rect content)
    {
        modal = default;
        content = default;
        var clipped = bounds;
        if (clipped.IsEmpty || clipped.Width < 24 || clipped.Height < 6)
        {
            return false;
        }

        var modalWidth = Math.Min(clipped.Width - 2, Math.Max(24, clipped.Width * 2 / 3));
        var modalHeight = Math.Min(clipped.Height - 2, Math.Max(8, clipped.Height * 2 / 3));
        var modalX = clipped.X + (clipped.Width - modalWidth) / 2;
        var modalY = clipped.Y + (clipped.Height - modalHeight) / 2;
        modal = new Rect(modalX, modalY, modalWidth, modalHeight);
        content = modal.Inset(1, 1);
        return !content.IsEmpty;
    }

    private int RowToFilteredIndex(Rect content, int y)
    {
        if (_filtered.Count == 0)
        {
            return -1;
        }

        var row = y - (content.Y + 1);
        if (row < 0)
        {
            return -1;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), Math.Max(0, content.Height - 1));
        if (row >= visibleRows)
        {
            return -1;
        }

        var start = ComputeWindowStart(_selectedFilteredIndex, visibleRows, _filtered.Count);
        var filtered = start + row;
        if (filtered < 0 || filtered >= _filtered.Count)
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
}

public sealed class TreeItemNode
{
    private readonly List<TreeItemNode> _children = [];

    public TreeItemNode(string id, string label, IEnumerable<TreeItemNode>? children = null)
    {
        Id = id;
        Label = label;
        if (children is not null)
        {
            _children.AddRange(children);
        }
    }

    public string Id { get; }

    public string Label { get; set; }

    public bool Expanded { get; set; } = true;

    public List<WidgetVisualState> States { get; } = [];

    public IReadOnlyList<TreeItemNode> Children => _children;

    public void AddChild(TreeItemNode child)
    {
        _children.Add(child);
    }
}

public sealed class TreeViewComponent : IStatefulComponent, IMouseStatefulComponent
{
    private readonly List<TreeItemNode> _roots = [];
    private readonly List<(TreeItemNode Node, int Depth, int? ParentVisibleIndex)> _visible = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    public string Title { get; set; } = "Tree";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ExpandKey { get; set; } = new("right/l", "expand", "right", "l");

    public KeyBinding CollapseKey { get; set; } = new("left/h", "collapse", "left", "h");

    public KeyBinding ToggleExpandKey { get; set; } = new("enter/space", "toggle", "enter", "space");

    public WidgetStatePalette NodeStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public string? SelectedNodeId => _selectedIndex >= 0 && _selectedIndex < _visible.Count
        ? _visible[_selectedIndex].Node.Id
        : null;

    public void SetRoots(IEnumerable<TreeItemNode> roots)
    {
        _roots.Clear();
        _roots.AddRange(roots);
        RefreshVisible();
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (_visible.Count == 0)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            var previous = _selectedIndex;
            _selectedIndex = Math.Min(_visible.Count - 1, _selectedIndex + 1);
            return _selectedIndex != previous;
        }

        if (PreviousItemKey.Matches(key))
        {
            var previous = _selectedIndex;
            _selectedIndex = Math.Max(0, _selectedIndex - 1);
            return _selectedIndex != previous;
        }

        if (ExpandKey.Matches(key))
        {
            return ExpandOrMoveIntoChild();
        }

        if (CollapseKey.Matches(key))
        {
            return CollapseOrMoveToParent();
        }

        if (ToggleExpandKey.Matches(key))
        {
            var node = _visible[_selectedIndex].Node;
            if (node.Children.Count == 0)
            {
                return false;
            }

            node.Expanded = !node.Expanded;
            RefreshVisible();
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || ReadOnly || _visible.Count == 0)
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
                changed |= SetHoveredIndex(-1);
            }

            if (message is not MouseWheelMsg)
            {
                return changed;
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                var previous = _selectedIndex;
                _selectedIndex = Math.Min(_visible.Count - 1, _selectedIndex + 1);
                changed |= _selectedIndex != previous;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                var previous = _selectedIndex;
                _selectedIndex = Math.Max(0, _selectedIndex - 1);
                changed |= _selectedIndex != previous;
            }
        }

        if (!inside)
        {
            return changed;
        }

        var start = ComputeWindowStart(content.Height);
        var hovered = start + (message.Y - content.Y);
        if (hovered < 0 || hovered >= _visible.Count)
        {
            hovered = -1;
        }

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
                if (_selectedIndex != hovered)
                {
                    _selectedIndex = hovered;
                    changed = true;
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

        if (_visible.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, NodeStatePalette.Render("(empty)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_visible.Count, start + content.Height);
        var row = 0;
        for (var i = start; i < end; i++, row++)
        {
            var (node, depth, _) = _visible[i];
            var indent = new string(' ', Math.Max(0, depth) * 2);
            var marker = node.Children.Count == 0
                ? "•"
                : node.Expanded ? "▾" : "▸";
            var cursor = i == _selectedIndex ? ">" : " ";

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

            states.AddRange(node.States);
            canvas.WriteText(content.X, content.Y + row, NodeStatePalette.Render($"{cursor} {indent}{marker} {node.Label}", states), content.Width);
        }
    }

    private bool ExpandOrMoveIntoChild()
    {
        var node = _visible[_selectedIndex].Node;
        if (node.Children.Count == 0)
        {
            return false;
        }

        if (!node.Expanded)
        {
            node.Expanded = true;
            RefreshVisible();
            return true;
        }

        if (_selectedIndex + 1 < _visible.Count && _visible[_selectedIndex + 1].Depth > _visible[_selectedIndex].Depth)
        {
            _selectedIndex++;
            return true;
        }

        return false;
    }

    private bool CollapseOrMoveToParent()
    {
        var entry = _visible[_selectedIndex];
        var node = entry.Node;
        if (node.Expanded && node.Children.Count > 0)
        {
            node.Expanded = false;
            RefreshVisible();
            return true;
        }

        if (entry.ParentVisibleIndex is int parent)
        {
            _selectedIndex = parent;
            return true;
        }

        return false;
    }

    private void RefreshVisible()
    {
        _visible.Clear();
        for (var i = 0; i < _roots.Count; i++)
        {
            AppendVisible(_roots[i], depth: 0, parentVisibleIndex: null);
        }

        if (_visible.Count == 0)
        {
            _selectedIndex = 0;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, _visible.Count - 1);
    }

    private void AppendVisible(TreeItemNode node, int depth, int? parentVisibleIndex)
    {
        var visibleIndex = _visible.Count;
        _visible.Add((node, depth, parentVisibleIndex));
        if (!node.Expanded || node.Children.Count == 0)
        {
            return;
        }

        for (var i = 0; i < node.Children.Count; i++)
        {
            AppendVisible(node.Children[i], depth + 1, visibleIndex);
        }
    }

    private int ComputeWindowStart(int contentHeight)
    {
        return Math.Clamp(_selectedIndex - (contentHeight / 2), 0, Math.Max(0, _visible.Count - contentHeight));
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
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

public enum NotificationSeverity
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
}

public sealed record NotificationEntry(
    string Id,
    string Message,
    NotificationSeverity Severity,
    DateTimeOffset CreatedAt,
    bool IsRead = false);

public sealed class NotificationCenterComponent : IStatefulComponent, IMouseStatefulComponent
{
    private readonly List<NotificationEntry> _entries = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    public string Title { get; set; } = "Notifications";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public int MaxEntries { get; set; } = 128;

    public bool ShowTimestamp { get; set; } = true;

    public bool AutoSelectNewest { get; set; } = true;

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding MarkReadKey { get; set; } = new("enter/space", "mark read", "enter", "space");

    public KeyBinding DismissKey { get; set; } = new("d", "dismiss", "d");

    public KeyBinding ClearAllKey { get; set; } = new("c", "clear all", "c");

    public WidgetStatePalette EntryStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public IReadOnlyList<NotificationEntry> Entries => _entries;

    public void Push(string message, NotificationSeverity severity = NotificationSeverity.Info, string? id = null)
    {
        var entry = new NotificationEntry(
            id ?? Guid.NewGuid().ToString("n"),
            message,
            severity,
            DateTimeOffset.UtcNow,
            IsRead: false);
        _entries.Add(entry);
        if (_entries.Count > MaxEntries)
        {
            _entries.RemoveAt(0);
        }

        if (AutoSelectNewest)
        {
            _selectedIndex = Math.Max(0, _entries.Count - 1);
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _entries.Count - 1));
        }
    }

    public void Clear()
    {
        _entries.Clear();
        _selectedIndex = 0;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (ClearAllKey.Matches(key))
        {
            if (_entries.Count == 0)
            {
                return false;
            }

            Clear();
            return true;
        }

        if (_entries.Count == 0)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            var previous = _selectedIndex;
            _selectedIndex = Math.Min(_entries.Count - 1, _selectedIndex + 1);
            return _selectedIndex != previous;
        }

        if (PreviousItemKey.Matches(key))
        {
            var previous = _selectedIndex;
            _selectedIndex = Math.Max(0, _selectedIndex - 1);
            return _selectedIndex != previous;
        }

        if (DismissKey.Matches(key))
        {
            _entries.RemoveAt(_selectedIndex);
            _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _entries.Count - 1));
            return true;
        }

        if (MarkReadKey.Matches(key))
        {
            var current = _entries[_selectedIndex];
            if (current.IsRead)
            {
                return false;
            }

            _entries[_selectedIndex] = current with { IsRead = true };
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
                changed |= SetHoveredIndex(-1);
            }

            if (message is not MouseWheelMsg)
            {
                return changed;
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && _entries.Count > 0)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                var previous = _selectedIndex;
                _selectedIndex = Math.Min(_entries.Count - 1, _selectedIndex + 1);
                changed |= _selectedIndex != previous;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                var previous = _selectedIndex;
                _selectedIndex = Math.Max(0, _selectedIndex - 1);
                changed |= _selectedIndex != previous;
            }
        }

        if (!inside || _entries.Count == 0)
        {
            return changed;
        }

        var start = ComputeWindowStart(content.Height);
        var hovered = start + (message.Y - content.Y);
        if (hovered < 0 || hovered >= _entries.Count)
        {
            hovered = -1;
        }

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
                if (_selectedIndex != hovered)
                {
                    _selectedIndex = hovered;
                    changed = true;
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

        if (_entries.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, EntryStatePalette.Render("(empty)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_entries.Count, start + content.Height);
        var row = 0;
        for (var i = start; i < end; i++, row++)
        {
            var entry = _entries[i];
            var cursor = i == _selectedIndex ? ">" : " ";
            var readMark = entry.IsRead ? " " : "•";
            var timestamp = ShowTimestamp ? $"{entry.CreatedAt:HH:mm:ss} " : string.Empty;
            var line = $"{cursor}{readMark} {timestamp}{entry.Message}";
            var states = ResolveEntryStates(entry, i == _selectedIndex, i == _hoveredIndex);
            canvas.WriteText(content.X, content.Y + row, EntryStatePalette.Render(line, states), content.Width);
        }
    }

    private IReadOnlyCollection<WidgetVisualState> ResolveEntryStates(NotificationEntry entry, bool selected, bool hovered)
    {
        var states = new List<WidgetVisualState>(7);
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

        if (selected)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (!entry.IsRead)
        {
            states.Add(WidgetVisualState.New);
        }

        states.Add(entry.Severity switch
        {
            NotificationSeverity.Success => WidgetVisualState.Success,
            NotificationSeverity.Warning => WidgetVisualState.Warning,
            NotificationSeverity.Error => WidgetVisualState.Error,
            _ => WidgetVisualState.Default,
        });
        return states;
    }

    private int ComputeWindowStart(int contentHeight)
    {
        return Math.Clamp(_selectedIndex - (contentHeight / 2), 0, Math.Max(0, _entries.Count - contentHeight));
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
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
