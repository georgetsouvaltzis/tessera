using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class StatusBarComponent : ICanvasComponent
{
    public string LeftText { get; set; } = string.Empty;

    public string RightText { get; set; } = string.Empty;

    public UiTheme Theme { get; set; } = new();

    public void Render(Canvas canvas, Rect rect)
    {
        UiWidgets.DrawStatusBar(canvas, rect, LeftText, RightText, Theme);
    }
}

