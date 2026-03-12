using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class ListView<T> : Control
{
    private readonly ListComponent<T> _component;

    public ListView(Func<T, string>? textSelector = null)
    {
        _component = new ListComponent<T>(Array.Empty<T>(), textSelector ?? DefaultText);
        _component.SelectionChanged += (_, args) =>
            SelectionChanged?.Invoke(this, new ListSelectionChangedEventArgs<T>(args.PreviousIndex, args.SelectedIndex, args.PreviousItem, args.SelectedItem));
    }

    public event EventHandler<ListSelectionChangedEventArgs<T>>? SelectionChanged;

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

    public int Count => _component.Count;

    public int SelectedIndex => _component.SelectedIndex;

    public T? SelectedItem => _component.SelectedItem;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => _component.IsDisabled;
        set => _component.IsDisabled = value;
    }

    public override bool IsReadOnly
    {
        get => _component.IsReadOnly;
        set => _component.IsReadOnly = value;
    }

    public void SetItems(IEnumerable<T> items) => _component.SetItems(items);

    public void SetFilter(string filter) => _component.SetFilter(filter ?? string.Empty);

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

    private static string DefaultText(T item) => item?.ToString() ?? string.Empty;
}
