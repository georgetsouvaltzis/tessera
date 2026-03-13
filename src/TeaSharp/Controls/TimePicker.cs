using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a control for editing a time-of-day value.
/// </summary>
public sealed class TimePicker : Control
{
    private readonly TimePickerComponent _component = new();

    public event EventHandler<TimeValueChangedEventArgs>? ValueChanged;

    public TimePicker()
    {
        _component.ValueChanged += (_, args) => ValueChanged?.Invoke(this, new TimeValueChangedEventArgs(args.PreviousValue, args.Value));
    }

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    public TimeOnly Value => _component.Value;

    public TimeOnly? LastCommittedTime => _component.LastCommittedTime;

    public TimeField ActiveField => _component.ActiveField switch
    {
        TimePickerField.Hour => TimeField.Hour,
        TimePickerField.Minute => TimeField.Minute,
        TimePickerField.Second => TimeField.Second,
        _ => TimeField.Hour,
    };

    public int HourStep
    {
        get => _component.HourStep;
        set => _component.HourStep = value;
    }

    public int MinuteStep
    {
        get => _component.MinuteStep;
        set => _component.MinuteStep = value;
    }

    public int SecondStep
    {
        get => _component.SecondStep;
        set => _component.SecondStep = value;
    }

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => _component.IsDisabled;
        set => _component.IsDisabled = value;
    }

    public override bool IsReadOnly
    {
        get => _component.IsReadOnly;
        set => _component.IsReadOnly = value;
    }

    public void SetValue(TimeOnly time) => _component.SetValue(time);

    public override bool Handle(Message message) => ControlForwarder.Forward(_component, message);

    public override bool Handle(Message message, Rect bounds) => ControlForwarder.Forward(_component, message, bounds) || Handle(message);

    public override void Render(Canvas canvas, Rect rect) => _component.Render(canvas, rect);
}
