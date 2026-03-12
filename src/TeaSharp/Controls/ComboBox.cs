using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class ComboBox : Control
{
    private readonly ComboboxComponent _component = new();

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public ComboBox()
    {
        _component.SelectionChanged += (_, args) =>
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(args.PreviousIndex, args.SelectedIndex, args.PreviousItem, args.SelectedItem));
    }

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public string Placeholder
    {
        get => _component.Placeholder;
        set => _component.Placeholder = value ?? string.Empty;
    }

    public string FilterText => _component.FilterText;

    public string SelectedItem => _component.SelectedItem;

    public bool IsOpen => _component.IsOpen;

    public int MaxVisibleItems
    {
        get => _component.MaxVisibleItems;
        set => _component.MaxVisibleItems = value;
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

    public void SetItems(IEnumerable<string> items) => _component.SetItems(items);

    public void SetFilterText(string value) => _component.SetFilterText(value ?? string.Empty);

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
