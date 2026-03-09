using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class ButtonComponent : IStatefulComponent, IFocusableComponent
{
    private static readonly KeyBinding ActivateKey = new("enter/space", "activate", "enter", "space");

    public string Label { get; set; } = "Button";

    public string? Description { get; set; }

    public bool Focused { get; set; }

    public bool Enabled { get; set; } = true;

    public int PressCount { get; private set; }

    public bool WasPressed { get; private set; }

    public bool Update(IMessage message)
    {
        WasPressed = false;
        if (!Enabled || !Focused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (!ActivateKey.Matches(key))
        {
            return false;
        }

        PressCount++;
        WasPressed = true;
        return true;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var prefix = Focused ? "›" : " ";
        var state = Enabled ? string.Empty : " (disabled)";
        var text = $"{prefix} [{Label}]{state}";
        canvas.WriteText(clipped.X, clipped.Y, text, clipped.Width);
        if (!string.IsNullOrWhiteSpace(Description) && clipped.Height > 1)
        {
            canvas.WriteText(clipped.X, clipped.Y + 1, Description!, clipped.Width);
        }
    }
}

