using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class TextArea : Control
{
    private readonly TextAreaComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public string Value => _component.Value;

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

    public bool ShowLineNumbers
    {
        get => _component.ShowLineNumbers;
        set => _component.ShowLineNumbers = value;
    }

    public bool Wrap
    {
        get => _component.Wrap;
        set => _component.Wrap = value;
    }

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void SetValue(string value) => _component.SetValue(value ?? string.Empty);

    public void Clear() => _component.Clear();

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
