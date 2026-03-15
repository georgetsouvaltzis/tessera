using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a compact status badge with a semantic tone.
/// </summary>
public sealed class Badge : Control
{
    public string Text
    {
        get;
        set => field = value ?? string.Empty;
    } = "Badge";

    public bool ShowBrackets
    {
        get;
        set;
    } = true;

    public BadgeTone Tone { get; set; }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var label = ShowBrackets
            ? $"[{Text}]"
            : Text;
        var state = Tone switch
        {
            BadgeTone.Success => WidgetVisualState.Success,
            BadgeTone.Warning => WidgetVisualState.Warning,
            BadgeTone.Error => WidgetVisualState.Error,
            _ => WidgetVisualState.Default,
        };
        var palette = WidgetStatePalette.CreateDefault();
        canvas.WriteText(clipped.X, clipped.Y, palette.Render(label, state), clipped.Width);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth(ShowBrackets ? $"[{Text}]" : Text);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(1, 0, availableBounds.Height));
    }
}
