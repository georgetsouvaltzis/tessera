using TeaSharp.Components.Primitives;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a single-choice group of radio options.
/// </summary>
public sealed class RadioGroup : Control
{
    private readonly List<string> _items = [];

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Radio";

    public int SelectedIndex { get; private set; }

    public string SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < _items.Count
            ? _items[SelectedIndex]
            : string.Empty;

    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = Math.Max(0, _items.Count - 1);
        }
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = false;
        if (_items.Count > 0 && message is KeyPressed key)
        {
            if (key.Is(Key.Down) || key.Is(Key.Right))
            {
                SelectedIndex = (SelectedIndex + 1) % _items.Count;
                changed = true;
            }
            else if (key.Is(Key.Up) || key.Is(Key.Left))
            {
                SelectedIndex = (SelectedIndex + _items.Count - 1) % _items.Count;
                changed = true;
            }
        }

        if (changed && previousIndex != SelectedIndex)
        {
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
        }

        return changed;
    }

    public override void Render(Canvas canvas, Rect rect)
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

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Title.Length + 4;
        for (var index = 0; index < _items.Count; index++)
        {
            width = Math.Max(width, _items[index].Length + 5);
        }

        var height = Math.Max(3, _items.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
