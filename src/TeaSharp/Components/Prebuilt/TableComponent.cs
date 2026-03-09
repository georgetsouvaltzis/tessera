using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class TableComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    public TableComponent(IReadOnlyList<string> headers)
    {
        Inner = new SortableTableComponent(headers);
    }

    public TableComponent(TableOptions options)
        : this(options.Headers)
    {
        Title = options.Title;
        Focused = options.Focused;
        ShowBorder = options.ShowBorder;
        if (options.PageSize.HasValue)
        {
            PageSize = options.PageSize.Value;
        }
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

    public int PageSize
    {
        get => Inner.PageSize;
        set => Inner.PageSize = value;
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
