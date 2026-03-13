using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a read-only markdown renderer.
/// </summary>
public sealed class MarkdownView : Control
{
    private readonly MarkdownViewerComponent _component = new();

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

    public bool Wrap
    {
        get => _component.Wrap;
        set => _component.Wrap = value;
    }

    public bool ShowLineNumbers
    {
        get => _component.ShowLineNumbers;
        set => _component.ShowLineNumbers = value;
    }

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void SetMarkdown(string markdown) => _component.SetMarkdown(markdown);

    public override bool Handle(Message message) => ControlForwarder.Forward(_component, message);

    public override void Render(Canvas canvas, Rect rect) => _component.Render(canvas, rect);
}
