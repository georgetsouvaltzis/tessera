using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a control for choosing multiple items from a list.
/// </summary>
public sealed class MultiSelect : Control
{
    private readonly CheckboxListComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public int SelectedIndex => _component.SelectedIndex;

    public string? SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < _component.Items.Count
            ? _component.Items[SelectedIndex].Label
            : null;

    public IReadOnlyList<string> CheckedItems =>
        _component.Items.Where(static item => item.Checked).Select(static item => item.Label).ToArray();

    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _component.SetItems(items.Select(static item => (item, false)));
    }

    public void SetItems(IEnumerable<(string Label, bool Checked)> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _component.SetItems(items);
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly)
        {
            return false;
        }

        return ControlForwarder.Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect) => _component.Render(canvas, rect);
}
