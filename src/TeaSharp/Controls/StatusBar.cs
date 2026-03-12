using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class StatusBar : Control
{
    private readonly StatusBarComponent _component = new();

    public string LeftText
    {
        get => _component.LeftText;
        set => _component.LeftText = value ?? string.Empty;
    }

    public string RightText
    {
        get => _component.RightText;
        set => _component.RightText = value ?? string.Empty;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
