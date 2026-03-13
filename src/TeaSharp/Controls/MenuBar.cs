using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using System.ComponentModel;

namespace TeaSharp.Controls;

public sealed class MenuBar : Control
{
    private readonly MenuBarComponent _component = new();
    private readonly List<MenuItem> _items = [];
    private readonly Dictionary<string, MenuItem> _itemsById = new(StringComparer.Ordinal);

    public event EventHandler<MenuItemActivatedEventArgs>? ItemActivated;

    public MenuBar()
    {
        _component.ItemActivated += (_, args) =>
        {
            if (_itemsById.TryGetValue(args.ItemId, out var item))
            {
                ItemActivated?.Invoke(this, new MenuItemActivatedEventArgs(item));
            }
        };
    }

    public int SelectedIndex => _component.SelectedIndex;

    public IReadOnlyList<MenuItem> Items => _items;

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

    public void SetItems(IEnumerable<MenuItem> items)
    {
        _items.Clear();
        _itemsById.Clear();
        var mapped = new List<TeaSharp.Components.Productivity.MenuBarItem>();
        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            _items.Add(item);
            _itemsById[item.Id] = item;
            mapped.Add(new TeaSharp.Components.Productivity.MenuBarItem(item.Id, item.Text, item.Shortcut));
        }

        _component.SetItems(mapped);
    }

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return ControlForwarder.Forward(_component, message, bounds) || Handle(message);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeActivation(out string itemId) => _component.TryConsumeActivation(out itemId);

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
