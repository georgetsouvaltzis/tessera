using TeaSharp.Components.Advanced;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class Spinner : Control
{
    private readonly SpinnerComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public string Label
    {
        get => _component.Label;
        set => _component.Label = value ?? string.Empty;
    }

    public bool Running => _component.Running;

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

    public void SetRunning(bool running) => _component.SetRunning(running);

    public void Advance() => _component.Advance();

    public override bool Handle(Message message)
    {
        return Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return Forward(_component, message, bounds) || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
