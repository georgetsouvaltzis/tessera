using TeaSharp.Components.Advanced;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class Toggle : Control
{
    private readonly ToggleSwitchComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public string OnText
    {
        get => _component.OnText;
        set => _component.OnText = value ?? string.Empty;
    }

    public string OffText
    {
        get => _component.OffText;
        set => _component.OffText = value ?? string.Empty;
    }

    public bool Value => _component.Value;

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

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => _component.IsDisabled;
        set => _component.IsDisabled = value;
    }

    public override bool IsReadOnly
    {
        get => _component.IsReadOnly;
        set => _component.IsReadOnly = value;
    }

    public void SetValue(bool value) => _component.SetValue(value);

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return ControlForwarder.Forward(_component, message, bounds) || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
