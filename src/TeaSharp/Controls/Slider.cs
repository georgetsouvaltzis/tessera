using TeaSharp.Components.Advanced;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class Slider : Control
{
    private readonly SliderComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public double Min
    {
        get => _component.Min;
        set => _component.Min = value;
    }

    public double Max
    {
        get => _component.Max;
        set => _component.Max = value;
    }

    public double Step
    {
        get => _component.Step;
        set => _component.Step = value;
    }

    public double Value => _component.Value;

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

    public void SetValue(double value) => _component.SetValue(value);

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
