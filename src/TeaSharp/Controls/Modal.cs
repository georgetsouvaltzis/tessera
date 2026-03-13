using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a dismissible overlay panel.
/// </summary>
public sealed class Modal : Control
{
    private readonly ModalComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public bool IsVisible
    {
        get => _component.IsVisible;
        set => _component.IsVisible = value;
    }

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

    public IReadOnlyList<string> BodyLines
    {
        get => _component.BodyLines;
        set => _component.BodyLines = value ?? ["(empty)"];
    }

    public char BackdropFill
    {
        get => _component.Theme.ModalBackdropFill;
        set => _component.Theme = _component.Theme with { ModalBackdropFill = value };
    }

    public void SetBodyLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        _component.BodyLines = [.. lines];
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
