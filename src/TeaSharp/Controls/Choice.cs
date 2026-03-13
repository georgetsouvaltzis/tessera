using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class Choice : Control
{
    private readonly DropdownComponent _component = new();

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public Choice()
    {
        _component.SelectionChanged += (_, args) =>
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(args.PreviousIndex, args.SelectedIndex, args.PreviousItem, args.SelectedItem));
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

    public int MaxVisibleItems
    {
        get => _component.MaxVisibleItems;
        set => _component.MaxVisibleItems = value;
    }

    public bool IsOpen => _component.IsOpen;

    public int SelectedIndex => _component.SelectedIndex;

    public string SelectedItem => _component.SelectedItem;

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

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return ControlForwarder.Forward(_component, message, bounds) || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
