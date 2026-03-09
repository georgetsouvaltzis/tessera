using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class CheckboxListComponent : IStatefulComponent
{
    private readonly List<(string Label, bool Checked)> _items = [];

    public int SelectedIndex { get; private set; }

    public string Title { get; set; } = "Checklist";

    public KeyBinding NextItemKey { get; set; } = new("down", "next item", "down");

    public KeyBinding PreviousItemKey { get; set; } = new("up", "previous item", "up");

    public KeyBinding ToggleItemKey { get; set; } = new("enter/space", "toggle item", "enter", "space");

    public void SetItems(IEnumerable<(string Label, bool Checked)> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = Math.Max(0, _items.Count - 1);
        }
    }

    public IReadOnlyList<(string Label, bool Checked)> Items => _items;

    public bool Update(IMessage message)
    {
        if (_items.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            SelectedIndex = Math.Min(_items.Count - 1, SelectedIndex + 1);
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            SelectedIndex = Math.Max(0, SelectedIndex - 1);
            return true;
        }

        if (ToggleItemKey.Matches(key))
        {
            var item = _items[SelectedIndex];
            _items[SelectedIndex] = (item.Label, !item.Checked);
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, Title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        for (var row = 0; row < rows; row++)
        {
            var item = _items[row];
            var selected = row == SelectedIndex ? "›" : " ";
            var marker = item.Checked ? "[x]" : "[ ]";
            canvas.WriteText(content.X, content.Y + row, $"{selected} {marker} {item.Label}", content.Width);
        }
    }
}

