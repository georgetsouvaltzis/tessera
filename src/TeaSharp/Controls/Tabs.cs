using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;

namespace TeaSharp.Controls;

public sealed class Tabs : Control
{
    private readonly TabsComponent _component;

    public Tabs(IEnumerable<string> items)
    {
        _component = new TabsComponent(items ?? Array.Empty<string>());
        _component.SelectionChanged += (_, args) =>
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(args.PreviousIndex, args.SelectedIndex, args.PreviousTab, args.SelectedTab));
    }

    public Tabs(params string[] items)
        : this((IEnumerable<string>)items)
    {
    }

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public IReadOnlyList<string> Items => _component.Tabs;

    public int SelectedIndex => _component.SelectedIndex;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void Select(int index) => _component.Select(index);

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
