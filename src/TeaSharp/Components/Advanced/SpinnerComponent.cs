using TeaSharp.Components.Advanced.Internal;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Advanced;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class SpinnerComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private List<string> _frames = ["|", "/", "-", "\\"];
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private bool _hovered;

    public string Title { get; set; } = "Spinner";

    public bool IsFocused { get; set; }

    public bool Running { get; private set; } = true;

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public int FrameIndex { get; private set; }

    public string Label { get; set; } = "loading";

    public KeyBinding AdvanceKey { get; set; } = new("right/space", "advance", "right", "space");

    public KeyBinding ToggleRunKey { get; set; } = new("enter", "toggle running", "enter");

    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public void SetFrames(IEnumerable<string> frames)
    {
        var list = frames
            .Where(frame => !string.IsNullOrWhiteSpace(frame))
            .ToList();
        if (list.Count == 0)
        {
            return;
        }

        _frames = list;
        FrameIndex = Math.Clamp(FrameIndex, 0, _frames.Count - 1);
    }

    public void Advance()
    {
        if (_frames.Count == 0)
        {
            return;
        }

        FrameIndex = (FrameIndex + 1) % _frames.Count;
    }

    public void SetRunning(bool running)
    {
        Running = running;
    }

    public bool Update(IMessage message)
    {
        if (!IsFocused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (ToggleRunKey.Matches(key))
        {
            Running = !Running;
            return true;
        }

        if (AdvanceKey.Matches(key))
        {
            if (Running)
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        var content = ResolveContentRect(bounds);
        if (_frames.Count == 0 || content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(message.X, message.Y);
        var changed = false;
        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHovered(false);
            }

            return changed;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHovered(true);
            return changed;
        }

        if (message is MouseClickMsg && InteractionProfile.HoverOnClick)
        {
            changed |= SetHovered(true);
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && Running)
        {
            if (wheel.Button is MouseButton.WheelUp or MouseButton.WheelDown)
            {
                Advance();
                changed = true;
            }
        }

        if (message is MouseClickMsg { Button: MouseButton.Left } && InteractionProfile.ActivateOnClick)
        {
            Running = !Running;
            changed = true;
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || _frames.Count == 0)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : IsFocused ? $"{Title} *" : Title,
            Border,
            Padding);

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var states = new List<WidgetVisualState>(3);
        if (IsFocused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (Running)
        {
            states.Add(WidgetVisualState.Loading);
        }
        else
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        canvas.WriteText(content.X, content.Y, StatePalette.Render($"{_frames[FrameIndex]} {Label}", states), content.Width);
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return FrameLayout.ResolveContentRect(bounds, Border, Padding);
    }

    private bool SetHovered(bool hovered)
    {
        if (_hovered == hovered)
        {
            return false;
        }

        _hovered = hovered;
        return true;
    }
}
