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
internal sealed class ToggleSwitchComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private bool _hovered;

    public string Title { get; set; } = "Toggle";

    public string OnText { get; set; } = "ON";

    public string OffText { get; set; } = "OFF";

    public bool Value { get; private set; }

    public bool IsFocused { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsReadOnly { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public KeyBinding ToggleKey { get; set; } = new("enter/space", "toggle", "enter", "space");

    public KeyBinding TurnOnKey { get; set; } = new("right", "on", "right");

    public KeyBinding TurnOffKey { get; set; } = new("left", "off", "left");

    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public void SetValue(bool value)
    {
        Value = value;
    }

    public bool Update(IMessage message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (ToggleKey.Matches(key))
        {
            Value = !Value;
            return true;
        }

        if (TurnOnKey.Matches(key))
        {
            var changed = !Value;
            Value = true;
            return changed;
        }

        if (TurnOffKey.Matches(key))
        {
            var changed = Value;
            Value = false;
            return changed;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly)
        {
            return false;
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
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

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (wheel.Button == MouseButton.WheelUp)
            {
                var was = Value;
                Value = true;
                changed |= !was;
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                var was = Value;
                Value = false;
                changed |= was;
            }
        }

        if (message is MouseClickMsg { Button: MouseButton.Left } && InteractionProfile.ActivateOnClick)
        {
            Value = !Value;
            changed = true;
        }

        return changed;
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

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var states = ResolveStates();
        if (Value)
        {
            states.Add(WidgetVisualState.Checked);
            states.Add(WidgetVisualState.Success);
        }
        else
        {
            states.Add(WidgetVisualState.Unchecked);
        }

        var label = Value ? OnText : OffText;
        canvas.WriteText(content.X, content.Y, StatePalette.Render($"<{label}>", states), content.Width);
    }

    private List<WidgetVisualState> ResolveStates()
    {
        var states = new List<WidgetVisualState>(4);
        if (IsFocused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (IsDisabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (IsReadOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
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
