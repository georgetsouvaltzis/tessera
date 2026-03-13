using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents read-only text content.
/// </summary>
public sealed class Label : Control
{
    private readonly TextBlockComponent _component = new();

    public string Text
    {
        get => _component.Text;
        set => _component.Text = value ?? string.Empty;
    }

    public string? Title
    {
        get => _component.Title;
        set => _component.Title = value;
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

    public TeaStyle TextStyle
    {
        get => _component.TextStyle;
        set => _component.TextStyle = value;
    }

    public TeaSharp.Layout.HorizontalAlignment HorizontalAlignment
    {
        get => _component.HorizontalAlignment;
        set => _component.HorizontalAlignment = value;
    }

    public TeaSharp.Layout.VerticalAlignment VerticalAlignment
    {
        get => _component.VerticalAlignment;
        set => _component.VerticalAlignment = value;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
