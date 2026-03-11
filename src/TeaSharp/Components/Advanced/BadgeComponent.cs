using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Advanced;

public sealed class BadgeComponent : ICanvasComponent
{
    public string Text { get; set; } = "Badge";

    public WidgetVisualState State { get; set; } = WidgetVisualState.Default;

    public WidgetStatePalette Palette { get; } = WidgetStatePalette.CreateDefault();

    public bool ShowBrackets { get; set; } = true;

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var label = ShowBrackets
            ? $"[{Text}]"
            : Text;
        canvas.WriteText(clipped.X, clipped.Y, Palette.Render(label, State), clipped.Width);
    }
}

