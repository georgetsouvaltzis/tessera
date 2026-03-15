using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a bounded progress indicator.
/// </summary>
public sealed class ProgressBar : Control
{
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Progress";

    public double Value { get; private set; }

    public double Step
    {
        get;
        set;
    } = 0.05;

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    public Thickness Padding
    {
        get;
        set;
    }

    public override bool IsFocused
    {
        get;
        set;
    }

    public void SetValue(double value) => Value = Math.Clamp(value, 0.0, 1.0);

    public override bool Handle(Message message)
    {
        if (!IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('-'))
        {
            SetValue(Value - Step);
            return true;
        }

        if (key.Is(Key.Right) || key.IsCharacter('+'))
        {
            SetValue(Value + Step);
            return true;
        }

        return false;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : IsFocused ? $"{Title} *" : Title,
            Border,
            Padding);
        if (content.IsEmpty)
        {
            return;
        }

        var percent = (int)Math.Round(Value * 100, MidpointRounding.AwayFromZero);
        TeaSharp.Components.Primitives.Widgets.DrawProgressBar(
            canvas,
            new Rect(content.X, content.Y, content.Width, 1),
            Value,
            $"{percent}%");
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, Title.Length + 4) + Padding.Horizontal;
        var height = Padding.Vertical + 2;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
