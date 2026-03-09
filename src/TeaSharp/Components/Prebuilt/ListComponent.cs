using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class ListComponent<T> : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
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

