using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class Table : Control
{
    private readonly TableComponent _component;

    public Table(IReadOnlyList<string> columns)
    {
        _component = new TableComponent(columns ?? Array.Empty<string>());
    }

    public Table(params string[] columns)
        : this((IReadOnlyList<string>)columns)
    {
    }

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    public int PageSize
    {
        get => _component.PageSize;
        set => _component.PageSize = value;
    }

    public int PageIndex => _component.PageIndex;

    public int SortColumn => _component.SortColumn;

    public bool SortDescending => _component.SortDescending;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void SetRows(IEnumerable<IReadOnlyList<string>> rows) => _component.SetRows(rows);

    public override bool Handle(Message message)
    {
        return Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return Forward(_component, message, bounds) || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
