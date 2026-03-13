using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

internal sealed class SortableTableComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<IReadOnlyList<string>> _rows = [];
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private int _hoveredVisibleRow = -1;
    private int _selectedVisibleRow = -1;

    public SortableTableComponent(IReadOnlyList<string> headers)
    {
        Headers = headers;
    }

    public IReadOnlyList<string> Headers { get; }

    public int SortColumn { get; private set; }

    public bool SortDescending { get; private set; }

    public int PageSize { get; set; } = 8;

    public int PageIndex { get; private set; }

    public string Title { get; set; } = "Table";

    public bool IsFocused { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public bool EnableVirtualization { get; set; }

    public int VirtualStartIndex { get; private set; }

    public int VirtualWindowSize { get; private set; } = 32;

    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public KeyBinding NextPageKey { get; set; } = new("]", "next page", "]");

    public KeyBinding PreviousPageKey { get; set; } = new("[", "previous page", "[");

    public KeyBinding ToggleSortDirectionKey { get; set; } = new("s", "toggle sort direction", "s");

    public KeyBinding NextSortColumnKey { get; set; } = new("c", "next sort column", "c");

    public KeyBinding VirtualForwardKey { get; set; } = new("v", "virtual forward", "v");

    public KeyBinding VirtualBackwardKey { get; set; } = new("shift+v", "virtual backward", "V");

    public void SetRows(IEnumerable<IReadOnlyList<string>> rows)
    {
        _rows.Clear();
        _rows.AddRange(rows);
        NormalizePage();
    }

    public bool Update(IMessage message)
    {
        if (Headers.Count == 0 || _rows.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextPageKey.Matches(key))
        {
            PageIndex++;
            NormalizePage();
            return true;
        }

        if (PreviousPageKey.Matches(key))
        {
            PageIndex = Math.Max(0, PageIndex - 1);
            return true;
        }

        if (ToggleSortDirectionKey.Matches(key))
        {
            SortDescending = !SortDescending;
            return true;
        }

        if (NextSortColumnKey.Matches(key))
        {
            SortColumn = (SortColumn + 1) % Headers.Count;
            return true;
        }

        if (EnableVirtualization && VirtualForwardKey.Matches(key))
        {
            VirtualStartIndex = Math.Max(0, VirtualStartIndex + Math.Max(1, VirtualWindowSize / 2));
            return true;
        }

        if (EnableVirtualization && VirtualBackwardKey.Matches(key))
        {
            VirtualStartIndex = Math.Max(0, VirtualStartIndex - Math.Max(1, VirtualWindowSize / 2));
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Headers.Count == 0 || _rows.Count == 0 || bounds.IsEmpty)
        {
            return false;
        }

        var state = BuildRenderState();
        var content = SortableTablePointerHelper.ResolveContentRect(bounds, Border, Padding, state.Title);
        if (content.IsEmpty || content.Height < 3)
        {
            return false;
        }

        var inside = content.Contains(message.X, message.Y);
        var changed = false;

        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredVisibleRow(-1);
            }

            if (message is not MouseWheelMsg)
            {
                return changed;
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            changed |= HandleWheelNavigation(wheel);
        }

        if (!inside)
        {
            return changed;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredVisibleRow(SortableTablePointerHelper.RowFromPointer(content, message.Y, state.VisibleRowCount));
            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredVisibleRow(SortableTablePointerHelper.RowFromPointer(content, click.Y, state.VisibleRowCount));
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick)
            {
                changed |= HandlePointerActivation(click.X, click.Y, content, state);
            }
        }

        return changed;
    }

    public void SetVirtualWindow(int startIndex, int windowSize)
    {
        VirtualStartIndex = Math.Max(0, startIndex);
        VirtualWindowSize = Math.Max(1, windowSize);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var state = BuildRenderState();
        NormalizeVisibleRowPointers(state.VisibleRowCount);

        TeaSharp.Components.Primitives.Widgets.DrawTable(
            canvas,
            rect,
            Headers,
            state.VisibleRows,
            selectedRow: _selectedVisibleRow >= 0 ? _selectedVisibleRow : _hoveredVisibleRow,
            title: state.Title,
            border: Border,
            padding: Padding);
    }

    private void NormalizePage()
    {
        var safePageSize = Math.Max(1, PageSize);
        var pageCount = Math.Max(1, (_rows.Count + safePageSize - 1) / safePageSize);
        PageIndex = Math.Clamp(PageIndex, 0, pageCount - 1);
    }

    private bool HandleWheelNavigation(MouseWheelMsg wheel)
    {
        if (EnableVirtualization)
        {
            var previous = VirtualStartIndex;
            var maxStart = Math.Max(0, _rows.Count - Math.Max(1, VirtualWindowSize));
            if (wheel.Button == MouseButton.WheelDown)
            {
                VirtualStartIndex = Math.Min(maxStart, VirtualStartIndex + 1);
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                VirtualStartIndex = Math.Max(0, VirtualStartIndex - 1);
            }
            else
            {
                return false;
            }

            NormalizeVisibleRowPointers(Math.Max(1, Math.Min(Math.Max(1, VirtualWindowSize), _rows.Count)));
            return VirtualStartIndex != previous;
        }

        var previousPage = PageIndex;
        if (wheel.Button == MouseButton.WheelDown)
        {
            PageIndex++;
            NormalizePage();
        }
        else if (wheel.Button == MouseButton.WheelUp)
        {
            PageIndex = Math.Max(0, PageIndex - 1);
        }
        else
        {
            return false;
        }

        NormalizeVisibleRowPointers(Math.Max(1, Math.Min(Math.Max(1, PageSize), _rows.Count)));
        return PageIndex != previousPage;
    }

    private bool HandlePointerActivation(int x, int y, Rect content, SortableTableRenderState state)
    {
        if (y == content.Y)
        {
            var column = SortableTablePointerHelper.HeaderColumnFromPointer(x, content, Headers.Count);
            if (column < 0)
            {
                return false;
            }

            if (column == SortColumn)
            {
                SortDescending = !SortDescending;
            }
            else
            {
                SortColumn = column;
                SortDescending = false;
            }

            return true;
        }

        var row = SortableTablePointerHelper.RowFromPointer(content, y, state.VisibleRowCount);
        if (row < 0 || _selectedVisibleRow == row)
        {
            return false;
        }

        _selectedVisibleRow = row;
        return true;
    }

    private bool SetHoveredVisibleRow(int row)
    {
        if (_hoveredVisibleRow == row)
        {
            return false;
        }

        _hoveredVisibleRow = row;
        return true;
    }

    private void NormalizeVisibleRowPointers(int visibleRows)
    {
        (_hoveredVisibleRow, _selectedVisibleRow) = SortableTablePointerHelper.NormalizeVisibleRowPointers(
            _hoveredVisibleRow,
            _selectedVisibleRow,
            visibleRows);
    }

    private SortableTableRenderState BuildRenderState()
    {
        return SortableTableRenderStateBuilder.Build(
            _rows,
            Headers,
            Title,
            SortColumn,
            SortDescending,
            PageSize,
            PageIndex,
            EnableVirtualization,
            VirtualStartIndex,
            VirtualWindowSize);
    }
}
