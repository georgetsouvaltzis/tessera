using TeaSharp.Components.Primitives;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a control for choosing multiple items from a list.
/// </summary>
public sealed class MultiSelect : Control
{
    private readonly List<(string Label, bool Checked)> _items = [];

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Checklist";

    public int SelectedIndex { get; private set; }

    public string? SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < _items.Count
            ? _items[SelectedIndex].Label
            : null;

    public IReadOnlyList<string> CheckedItems =>
        _items.Where(static item => item.Checked).Select(static item => item.Label).ToArray();

    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        SetItems(items.Select(static item => (item, false)));
    }

    public void SetItems(IEnumerable<(string Label, bool Checked)> items)
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
        if (!IsFocused || IsDisabled || IsReadOnly || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down))
        {
            var next = Math.Min(_items.Count - 1, SelectedIndex + 1);
            if (next == SelectedIndex)
            {
                return false;
            }

            SelectedIndex = next;
            return true;
        }

        if (key.Is(Key.Up))
        {
            var previous = Math.Max(0, SelectedIndex - 1);
            if (previous == SelectedIndex)
            {
                return false;
            }

            SelectedIndex = previous;
            return true;
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            var item = _items[SelectedIndex];
            _items[SelectedIndex] = (item.Label, !item.Checked);
            return true;
        }

        return false;
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
            var item = _items[row];
            var selected = row == SelectedIndex ? "›" : " ";
            var marker = item.Checked ? "[x]" : "[ ]";
            canvas.WriteText(content.X, content.Y + row, $"{selected} {marker} {item.Label}", content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Title.Length + 4;
        for (var index = 0; index < _items.Count; index++)
        {
            width = Math.Max(width, _items[index].Label.Length + 7);
        }

        var height = Math.Max(3, _items.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
