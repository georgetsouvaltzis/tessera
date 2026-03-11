using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class StatusBarComponent : ICanvasComponent
{
    public StatusBarComponent()
    {
        Theme = new UiTheme();
    }

    public StatusBarComponent(StatusBarOptions options)
    {
        LeftText = options.LeftText;
        RightText = options.RightText;
        Theme = options.Theme ?? new UiTheme();
    }

    public string LeftText { get; set; } = string.Empty;

    public string RightText { get; set; } = string.Empty;

    public UiTheme Theme { get; set; }

    public void Render(Canvas canvas, Rect rect)
    {
        UiWidgets.DrawStatusBar(canvas, rect, LeftText, RightText, Theme);
    }
}
