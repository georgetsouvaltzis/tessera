using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class ProgressBarComponent : IStatefulComponent, IFocusableComponent
{
    public ProgressBarComponent()
    {
    }

    public ProgressBarComponent(ProgressBarOptions options)
    {
        Title = options.Title;
        IsFocused = options.IsFocused;
        Border = options.Border;
        Padding = options.Padding;
        Step = options.Step;
        DecreaseKey = options.DecreaseKey ?? new KeyBinding("left/-", "decrease", "left", "-");
        IncreaseKey = options.IncreaseKey ?? new KeyBinding("right/+", "increase", "right", "+");
        SetValue(options.InitialValue);
    }

    public double Value { get; private set; }

    public string Title { get; set; } = "Progress";

    public bool IsFocused { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public double Step { get; set; } = 0.05;

    public KeyBinding DecreaseKey { get; set; } = new("left/-", "decrease", "left", "-");

    public KeyBinding IncreaseKey { get; set; } = new("right/+", "increase", "right", "+");

    public bool Update(IMessage message)
    {
        if (message is not KeyPressMsg key || !IsFocused)
        {
            return false;
        }

        if (DecreaseKey.Matches(key))
        {
            SetValue(Value - Step);
            return true;
        }

        if (IncreaseKey.Matches(key))
        {
            SetValue(Value + Step);
            return true;
        }

        return false;
    }

    public void SetValue(double value)
    {
        Value = Math.Clamp(value, 0.0, 1.0);
    }

    public void Render(Canvas canvas, Rect rect)
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
        TeaSharp.Components.Primitives.Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y, content.Width, 1), Value, $"{percent}%");
    }
}
