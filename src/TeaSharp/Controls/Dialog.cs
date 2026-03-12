using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class Dialog : Control
{
    private readonly DialogComponent _component = new();

    public event EventHandler? Accepted
    {
        add => _component.Accepted += value;
        remove => _component.Accepted -= value;
    }

    public event EventHandler? Dismissed
    {
        add => _component.Dismissed += value;
        remove => _component.Dismissed -= value;
    }

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public IReadOnlyList<string> BodyLines
    {
        get => _component.BodyLines;
        set => _component.BodyLines = value ?? Array.Empty<string>();
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

    public bool IsVisible
    {
        get => _component.IsVisible;
        set => _component.IsVisible = value;
    }

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void Show(string title, params string[] lines)
    {
        Title = title;
        BodyLines = lines;
        IsVisible = true;
    }

    public void Hide()
    {
        IsVisible = false;
    }

    public bool TryConsumeResult(out DialogResult result)
    {
        if (_component.TryConsumeResult(out var current))
        {
            result = current switch
            {
                global::TeaSharp.Components.Prebuilt.DialogResult.Accepted => DialogResult.Accepted,
                global::TeaSharp.Components.Prebuilt.DialogResult.Dismissed => DialogResult.Dismissed,
                _ => DialogResult.None,
            };
            return true;
        }

        result = DialogResult.None;
        return false;
    }

    public override bool Handle(Message message)
    {
        return Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
