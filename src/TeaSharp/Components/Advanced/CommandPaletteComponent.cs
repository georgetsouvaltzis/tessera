using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class CommandPaletteComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
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

