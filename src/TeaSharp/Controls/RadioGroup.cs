using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a single-choice group of radio options.
/// </summary>
public sealed class RadioGroup : Control
{
    private readonly RadioGroupComponent _component = new();
    private readonly List<string> _items = [];

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public int SelectedIndex => _component.SelectedIndex;

    public string SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < _items.Count
            ? _items[SelectedIndex]
            : string.Empty;

    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        _items.AddRange(items);
        _component.SetItems(_items);
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = ControlForwarder.Forward(_component, message);
        if (changed && previousIndex != SelectedIndex)
        {
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
        }

        return changed;
    }

    public override void Render(Canvas canvas, Rect rect) => _component.Render(canvas, rect);
}
