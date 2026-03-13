using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a bounded progress indicator.
/// </summary>
public sealed class ProgressBar : Control
{
    private readonly ProgressBarComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public double Value => _component.Value;

    public double Step
    {
        get => _component.Step;
        set => _component.Step = value;
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

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void SetValue(double value) => _component.SetValue(value);

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
