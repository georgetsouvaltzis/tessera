using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Styles;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class ButtonComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private static readonly KeyBinding ActivateKey = new("enter/space", "activate", "enter", "space");
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private bool _hovered;
    private bool _pressed;

    public ButtonComponent()
    {
        StatePalette.Set(WidgetVisualState.Active, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithInverse().WithBold(),
        });
    }

    public ButtonComponent(ButtonOptions options)
        : this()
    {
        Label = options.Label;
        Description = options.Description;
        Focused = options.Focused;
        Enabled = options.Enabled;
        Border = options.Border;
        Padding = options.Padding;
        InteractionProfile = options.InteractionProfile ?? WidgetInteractionProfile.Default;
    }

    public string Label { get; set; } = "Button";

    public string? Description { get; set; }

    public bool Focused { get; set; }

    public bool Enabled { get; set; } = true;

    public BorderStyle Border { get; set; } = BorderStyle.None;

    public Thickness Padding { get; set; }

    public bool Hovered => _hovered;

    public bool Pressed => _pressed;

    public int PressCount { get; private set; }

    public bool WasPressed { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

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

        return Activate(pressed: false);
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        WasPressed = false;
        if (!Enabled)
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
            if (message is MouseMotionMsg or MouseClickMsg or MouseReleaseMsg)
            {
                changed |= SetHovered(false);
                changed |= SetPressed(false);
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

        if (message is MouseClickMsg { Button: MouseButton.Left } && InteractionProfile.ActivateOnClick)
        {
            changed |= Activate(pressed: true);
            return changed;
        }

        if (message is MouseReleaseMsg { Button: MouseButton.Left })
        {
            changed |= SetPressed(false);
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped, null, Border, Padding);

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var states = ResolveStates();
        var suffix = Enabled ? string.Empty : " (disabled)";
        var plainText = $"[{Label}]{suffix}";
        var text = StatePalette.Render(plainText, states);
        var plainDescription = string.IsNullOrWhiteSpace(Description) ? null : Description!;
        var description = plainDescription is null ? null : StatePalette.Render(plainDescription, states);

        var rowCount = description is null || content.Height < 2 ? 1 : 2;
        var top = content.Y + Math.Max(0, (content.Height - rowCount) / 2);
        WriteCentered(canvas, content, top, plainText.Length, text);
        if (description is not null && rowCount > 1)
        {
            WriteCentered(canvas, content, top + 1, plainDescription!.Length, description);
        }
    }

    private bool Activate(bool pressed)
    {
        PressCount++;
        WasPressed = true;
        SetPressed(pressed);
        return true;
    }

    private List<WidgetVisualState> ResolveStates()
    {
        var states = new List<WidgetVisualState>(4);
        if (Focused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (!Enabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (_pressed)
        {
            states.Add(WidgetVisualState.Active);
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

    private bool SetPressed(bool pressed)
    {
        if (_pressed == pressed)
        {
            return false;
        }

        _pressed = pressed;
        return true;
    }

    private static void WriteCentered(Canvas canvas, Rect content, int y, int displayWidth, string text)
    {
        if (y < content.Y || y > content.Bottom)
        {
            return;
        }

        var x = content.X;
        var width = content.Width;
        if (displayWidth < content.Width)
        {
            var offset = (content.Width - displayWidth) / 2;
            x += offset;
            width -= offset;
        }

        canvas.WriteText(x, y, text, width);
    }
}
