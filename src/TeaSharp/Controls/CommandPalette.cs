using System.ComponentModel;
using TeaSharp.Components.Primitives;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a searchable command launcher overlay.
/// </summary>
public sealed class CommandPalette : Control
{
    private readonly List<CommandPaletteItem> _items = [];
    private readonly List<int> _filteredIndices = [];
    private readonly TextInputModel _query = new();
    private int _selectedFilteredIndex;
    private int _hoveredFilteredIndex = -1;
    private long _executionVersion;
    private long _consumedExecutionVersion;

    public event EventHandler<CommandPaletteItemExecutedEventArgs>? ItemExecuted;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Command Palette";

    public bool IsVisible { get; private set; }

    public int MaxVisibleItems
    {
        get;
        set;
    } = 8;

    public string QueryText
    {
        get => _query.Value;
        set => SetQueryText(value ?? string.Empty);
    }

    public string? LastExecutedItemId { get; private set; }

    public IReadOnlyList<CommandPaletteItem> Items => _items;

    public override bool IsFocused
    {
        get;
        set;
    }

    public void SetItems(IEnumerable<CommandPaletteItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items.Clear();
        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            _items.Add(item);
        }

        RefreshFilter();
    }

    public void ClearQuery() => SetQueryText(string.Empty);

    public void Open()
    {
        RequestFocus();
        if (IsVisible)
        {
            return;
        }

        IsVisible = true;
        ClearQuery();
    }

    public void Close()
    {
        IsVisible = false;
    }

    public void SetQueryText(string query)
    {
        _query.SetValue(query ?? string.Empty);
        RefreshFilter();
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused)
        {
            return false;
        }

        if (!IsVisible)
        {
            if (message is KeyPressed key && key.IsCharacter('p', ModifierKeys.Ctrl))
            {
                Open();
                return true;
            }

            return false;
        }

        if (message is KeyPressed input)
        {
            if (input.Is(Key.Escape))
            {
                Close();
                return true;
            }

            if (input.Is(Key.Down) && _filteredIndices.Count > 0)
            {
                MoveNext();
                return true;
            }

            if (input.Is(Key.Up) && _filteredIndices.Count > 0)
            {
                MovePrevious();
                return true;
            }

            if (input.Is(Key.Enter))
            {
                return ExecuteSelected();
            }
        }

        var inputResult = _query.Update(message);
        if (inputResult.Changed)
        {
            RefreshFilter();
            return true;
        }

        return inputResult.Submitted && ExecuteSelected();
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (!IsVisible || message is not PointerInput pointer || !TryResolveModal(bounds, out var modal, out var content))
        {
            return Handle(message);
        }

        var insideModal = modal.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!insideModal)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredFilteredIndex(-1);
            }

            if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
            {
                Close();
                changed = true;
            }

            return changed;
        }

        if (pointer.Kind == PointerEventKind.Wheel && _filteredIndices.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                MoveNext();
                return true;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                MovePrevious();
                return true;
            }
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredFilteredIndex(-1);
            }

            return changed;
        }

        var hovered = RowToFilteredIndex(content, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredFilteredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            changed |= SetHoveredFilteredIndex(hovered);
            changed |= SetSelectedFilteredIndex(hovered);
            changed |= ExecuteSelected();
        }

        return changed;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeExecution(out string itemId)
    {
        if (_executionVersion == _consumedExecutionVersion || string.IsNullOrEmpty(LastExecutedItemId))
        {
            itemId = string.Empty;
            return false;
        }

        _consumedExecutionVersion = _executionVersion;
        itemId = LastExecutedItemId;
        return true;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        if (!IsVisible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (!TryResolveModal(clipped, out var modal, out var content))
        {
            return;
        }

        canvas.DrawBox(modal, Title, BorderStyle.Rounded);

        var queryWidth = Math.Max(1, content.Width - 2);
        var frame = _query.BuildFrame(queryWidth);
        canvas.WriteText(content.X, content.Y, $"> {frame.Text}", content.Width);
        if (content.Height <= 1)
        {
            return;
        }

        if (_filteredIndices.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, "(no commands)", content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = ComputeWindowStart(_selectedFilteredIndex, visibleRows, _filteredIndices.Count);
        var end = Math.Min(_filteredIndices.Count, start + visibleRows);
        var row = 0;
        for (var filteredIndex = start; filteredIndex < end; filteredIndex++, row++)
        {
            var item = _items[_filteredIndices[filteredIndex]];
            var marker = filteredIndex == _selectedFilteredIndex ? ">" : filteredIndex == _hoveredFilteredIndex ? "▸" : " ";
            var summary = string.IsNullOrWhiteSpace(item.Description)
                ? item.Title
                : $"{item.Title} - {item.Description}";
            canvas.WriteText(content.X, content.Y + 1 + row, $"{marker} {summary}", content.Width);
        }
    }

    private void RefreshFilter()
    {
        _filteredIndices.Clear();
        var filter = _query.Value.Trim();
        for (var index = 0; index < _items.Count; index++)
        {
            var item = _items[index];
            var include = filter.Length == 0
                || item.Title.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || item.Description.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || item.Id.Contains(filter, StringComparison.OrdinalIgnoreCase);
            if (include)
            {
                _filteredIndices.Add(index);
            }
        }

        if (_filteredIndices.Count == 0)
        {
            _selectedFilteredIndex = 0;
            _hoveredFilteredIndex = -1;
            return;
        }

        _selectedFilteredIndex = Math.Clamp(_selectedFilteredIndex, 0, _filteredIndices.Count - 1);
        if (_hoveredFilteredIndex >= _filteredIndices.Count)
        {
            _hoveredFilteredIndex = _filteredIndices.Count - 1;
        }
    }

    private void MoveNext()
    {
        if (_filteredIndices.Count > 0)
        {
            _selectedFilteredIndex = (_selectedFilteredIndex + 1) % _filteredIndices.Count;
        }
    }

    private void MovePrevious()
    {
        if (_filteredIndices.Count > 0)
        {
            _selectedFilteredIndex = (_selectedFilteredIndex + _filteredIndices.Count - 1) % _filteredIndices.Count;
        }
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

    private bool SetSelectedFilteredIndex(int index)
    {
        if (_selectedFilteredIndex == index)
        {
            return false;
        }

        _selectedFilteredIndex = index;
        return true;
    }

    private bool ExecuteSelected()
    {
        if (_filteredIndices.Count == 0)
        {
            Close();
            return true;
        }

        var item = _items[_filteredIndices[Math.Clamp(_selectedFilteredIndex, 0, _filteredIndices.Count - 1)]];
        LastExecutedItemId = item.Id;
        _executionVersion++;
        ItemExecuted?.Invoke(this, new CommandPaletteItemExecutedEventArgs(item));
        Close();
        return true;
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
        return start > maxStart ? maxStart : start;
    }

    private int RowToFilteredIndex(Rect content, int y)
    {
        if (content.Height <= 1 || _filteredIndices.Count == 0)
        {
            return -1;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var row = y - (content.Y + 1);
        if (row < 0 || row >= visibleRows)
        {
            return -1;
        }

        var start = ComputeWindowStart(_selectedFilteredIndex, visibleRows, _filteredIndices.Count);
        var filteredIndex = start + row;
        return filteredIndex >= 0 && filteredIndex < _filteredIndices.Count
            ? filteredIndex
            : -1;
    }

    private static bool TryResolveModal(Rect bounds, out Rect modal, out Rect content)
    {
        modal = default;
        content = default;
        if (bounds.IsEmpty || bounds.Width < 24 || bounds.Height < 6)
        {
            return false;
        }

        var modalWidth = Math.Min(bounds.Width - 2, Math.Max(24, bounds.Width * 2 / 3));
        var modalHeight = Math.Min(bounds.Height - 2, Math.Max(8, bounds.Height * 2 / 3));
        var modalX = bounds.X + (bounds.Width - modalWidth) / 2;
        var modalY = bounds.Y + (bounds.Height - modalHeight) / 2;
        modal = new Rect(modalX, modalY, modalWidth, modalHeight);
        content = modal.Inset(1, 1);
        return !content.IsEmpty;
    }
}
