using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a two-sided status strip.
/// </summary>
public sealed class StatusBar : Control
{
    public string LeftText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string RightText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public override void Render(Canvas canvas, Rect rect)
    {
        UiWidgets.DrawStatusBar(canvas, rect, LeftText, RightText);
    }
}
