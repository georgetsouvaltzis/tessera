using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class LogView : Control
{
    private readonly LogViewerComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
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

    public bool AutoScroll
    {
        get => _component.AutoScroll;
        set => _component.AutoScroll = value;
    }

    public bool IsPaused => _component.Paused;

    public int Count => _component.Count;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void Append(string line) => _component.Append(line ?? string.Empty);

    public void Clear() => _component.Clear();

    public void SetFilter(string filter) => _component.SetFilter(filter ?? string.Empty);

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
