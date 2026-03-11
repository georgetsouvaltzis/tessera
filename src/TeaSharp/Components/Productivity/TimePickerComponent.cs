using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Provides field-based time selection with keyboard and mouse navigation.
/// </summary>
public sealed class TimePickerComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private TimePickerField? _hoveredField;

    public TimePickerComponent()
    {
    }

    public TimePickerComponent(TimePickerOptions options)
    {
        Title = options.Title;
        Focused = options.Focused;
        Disabled = options.Disabled;
        ReadOnly = options.ReadOnly;
        Border = options.Border;
        Padding = options.Padding;
        ActiveField = options.ActiveField;
        HourStep = options.HourStep;
        MinuteStep = options.MinuteStep;
        SecondStep = options.SecondStep;
        NextFieldKey = options.NextFieldKey ?? NextFieldKey;
        PreviousFieldKey = options.PreviousFieldKey ?? PreviousFieldKey;
        IncreaseKey = options.IncreaseKey ?? IncreaseKey;
        DecreaseKey = options.DecreaseKey ?? DecreaseKey;
        CommitKey = options.CommitKey ?? CommitKey;
        InteractionProfile = options.InteractionProfile ?? WidgetInteractionProfile.Default;
        if (options.InitialValue is { } initialTime)
        {
            SetValue(initialTime);
        }
    }

    public string Title { get; set; } = "Time Picker";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public TimeOnly Value { get; private set; } = TimeOnly.FromDateTime(DateTime.UtcNow);

    public TimeOnly? LastCommittedTime { get; private set; }

    public TimePickerField ActiveField { get; private set; }

    public int HourStep { get; set; } = 1;

    public int MinuteStep { get; set; } = 1;

    public int SecondStep { get; set; } = 5;

    public KeyBinding NextFieldKey { get; set; } = new("right/l", "next field", "right", "l");

    public KeyBinding PreviousFieldKey { get; set; } = new("left/h", "previous field", "left", "h");

    public KeyBinding IncreaseKey { get; set; } = new("up/k", "increase", "up", "k");

    public KeyBinding DecreaseKey { get; set; } = new("down/j", "decrease", "down", "j");

    public KeyBinding CommitKey { get; set; } = new("enter/space", "commit time", "enter", "space");

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetStatePalette FieldStatePalette { get; } = WidgetStatePalette.CreateDefault();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public void SetValue(TimeOnly time)
    {
        Value = time;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextFieldKey.Matches(key))
        {
            ActiveField = TimePickerFields.Next(ActiveField);
            return true;
        }

        if (PreviousFieldKey.Matches(key))
        {
            ActiveField = TimePickerFields.Previous(ActiveField);
            return true;
        }

        if (IncreaseKey.Matches(key))
        {
            Value = TimePickerFields.Adjust(Value, ActiveField, HourStep, MinuteStep, SecondStep, 1);
            return true;
        }

        if (DecreaseKey.Matches(key))
        {
            Value = TimePickerFields.Adjust(Value, ActiveField, HourStep, MinuteStep, SecondStep, -1);
            return true;
        }

        if (CommitKey.Matches(key))
        {
            LastCommittedTime = Value;
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || ReadOnly)
        {
            return false;
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
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
                changed |= SetHoveredField(null);
            }

            return changed;
        }

        var field = TimePickerFields.FieldFromPointer(content, message.X, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredField(field);
            return changed;
        }

        if (message is MouseClickMsg or MouseReleaseMsg)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredField(field);
            }

            if (message.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && field.HasValue)
            {
                if (ActiveField != field.Value)
                {
                    ActiveField = field.Value;
                    changed = true;
                }
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (field.HasValue && ActiveField != field.Value)
            {
                ActiveField = field.Value;
                changed = true;
            }

            if (wheel.Button == MouseButton.WheelUp)
            {
                Value = TimePickerFields.Adjust(Value, ActiveField, HourStep, MinuteStep, SecondStep, 1);
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                Value = TimePickerFields.Adjust(Value, ActiveField, HourStep, MinuteStep, SecondStep, -1);
                changed = true;
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        TimePickerRenderer.Render(canvas, rect, Title, Focused, Disabled, ReadOnly, Border, Padding, Value, ActiveField, _hoveredField, FieldStatePalette);
    }

    private bool SetHoveredField(TimePickerField? field)
    {
        if (_hoveredField == field)
        {
            return false;
        }

        _hoveredField = field;
        return true;
    }
}
