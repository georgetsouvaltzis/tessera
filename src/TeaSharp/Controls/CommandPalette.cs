using System.ComponentModel;
using LegacyCommandPaletteItem = TeaSharp.Components.Advanced.CommandPaletteItem;
using TeaSharp.Components.Advanced;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class CommandPalette : Control
{
    private readonly CommandPaletteComponent _component = new();
    private readonly List<CommandPaletteItem> _items = [];
    private readonly Dictionary<string, CommandPaletteItem> _itemsById = new(StringComparer.Ordinal);

    public event EventHandler<CommandPaletteItemExecutedEventArgs>? ItemExecuted;

    public CommandPalette()
    {
        _component.ItemExecuted += (_, args) =>
        {
            if (_itemsById.TryGetValue(args.ItemId, out var item))
            {
                ItemExecuted?.Invoke(this, new CommandPaletteItemExecutedEventArgs(item));
            }
        };
    }

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public bool IsVisible => _component.IsOpen;

    public int MaxVisibleItems
    {
        get => _component.MaxVisibleItems;
        set => _component.MaxVisibleItems = value;
    }

    public string QueryText
    {
        get => _component.QueryText;
        set => _component.SetQueryText(value ?? string.Empty);
    }

    public string? LastExecutedItemId => _component.LastExecutedItemId;

    public IReadOnlyList<CommandPaletteItem> Items => _items;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void SetItems(IEnumerable<CommandPaletteItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items.Clear();
        _itemsById.Clear();

        var mapped = new List<LegacyCommandPaletteItem>();
        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            _items.Add(item);
            _itemsById[item.Id] = item;
            mapped.Add(new LegacyCommandPaletteItem(item.Id, item.Title, item.Description));
        }

        _component.SetItems(mapped);
    }

    public void ClearQuery() => _component.ClearQuery();

    public void Open() => _component.Open();

    public void Close() => _component.Close();

    public override bool Handle(Message message)
    {
        return Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return Forward(_component, message, bounds) || Handle(message);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeExecution(out string itemId) => _component.TryConsumeExecution(out itemId);

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
