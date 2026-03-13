using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Controls;

public sealed class Button : Control
{
    private readonly ButtonComponent _component = new();

    public event EventHandler? Activated
    {
        add => _component.Pressed += value;
        remove => _component.Pressed -= value;
    }

    public string Text
    {
        get => _component.Label;
        set => _component.Label = value ?? string.Empty;
    }

    public string? Description
    {
        get => _component.Description;
        set => _component.Description = value;
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

    public int ActivationCount => _component.PressCount;

    public bool IsPressed => _component.IsPressed;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => !_component.Enabled;
        set => _component.Enabled = !value;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeActivation() => _component.TryConsumePress();

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
