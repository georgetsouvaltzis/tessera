using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

public sealed class RadioGroupComponent : IStatefulComponent
{
    private readonly List<string> _items = [];

    public int SelectedIndex { get; private set; }

    public string Title { get; set; } = "Radio";

    public KeyBinding NextItemKey { get; set; } = new("down/right", "next item", "down", "right");

    public KeyBinding PreviousItemKey { get; set; } = new("up/left", "previous item", "up", "left");

    public void SetItems(IEnumerable<string> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = Math.Max(0, _items.Count - 1);
        }
    }

    public bool Update(IMessage message)
    {
        if (_items.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + 1) % _items.Count;
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + _items.Count - 1) % _items.Count;
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
            var marker = row == SelectedIndex ? "(•)" : "( )";
            canvas.WriteText(content.X, content.Y + row, $"{marker} {_items[row]}", content.Width);
        }
    }
}

