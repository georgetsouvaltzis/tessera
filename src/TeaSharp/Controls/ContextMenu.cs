using System.ComponentModel;
using LegacyContextMenuItem = TeaSharp.Components.Productivity.ContextMenuItem;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;

namespace TeaSharp.Controls;

public sealed class ContextMenu : Control
{
    private readonly ContextMenuComponent _component = new();
    private readonly List<ContextMenuItem> _items = [];
    private readonly Dictionary<string, ContextMenuItem> _itemsById = new(StringComparer.Ordinal);

    public event EventHandler<ContextMenuItemExecutedEventArgs>? ItemExecuted;

    public ContextMenu()
    {
        _component.ItemExecuted += (_, args) =>
        {
            if (_itemsById.TryGetValue(args.ItemId, out var item))
            {
                ItemExecuted?.Invoke(this, new ContextMenuItemExecutedEventArgs(item));
            }
        };
    }

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public bool IsVisible => _component.IsVisible;

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

    public int AnchorX => _component.AnchorX;

    public int AnchorY => _component.AnchorY;

    public IReadOnlyList<ContextMenuItem> Items => _items;

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

    public void SetItems(IEnumerable<ContextMenuItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items.Clear();
        _itemsById.Clear();

        var mapped = new List<LegacyContextMenuItem>();
        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            _items.Add(item);
            _itemsById[item.Id] = item;
            mapped.Add(new LegacyContextMenuItem(item.Id, item.Title));
        }

        _component.SetItems(mapped);
    }

    public void OpenAt(int x, int y)
    {
        RequestFocus();
        _component.OpenAt(x, y);
    }

    public void Close() => _component.Close();

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return ControlForwarder.Forward(_component, message, bounds) || Handle(message);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeExecution(out string itemId) => _component.TryConsumeExecution(out itemId);

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
