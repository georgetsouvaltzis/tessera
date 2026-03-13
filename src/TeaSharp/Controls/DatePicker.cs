using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;

namespace TeaSharp.Controls;

public sealed class DatePicker : Control
{
    private readonly DatePickerComponent _component = new();

    public event EventHandler<DateChangedEventArgs>? DateChanged;

    public DatePicker()
    {
        _component.DateChanged += (_, args) => DateChanged?.Invoke(this, new DateChangedEventArgs(args.PreviousDate, args.SelectedDate));
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

    public DateOnly SelectedDate => _component.SelectedDate;

    public DateOnly CurrentMonth => _component.CurrentMonth;

    public DateOnly? LastCommittedDate => _component.LastCommittedDate;

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

    public void SetDate(DateOnly date) => _component.SetDate(date);

    public override bool Handle(Message message) => ControlForwarder.Forward(_component, message);

    public override bool Handle(Message message, Rect bounds) => ControlForwarder.Forward(_component, message, bounds) || Handle(message);

    public override void Render(Canvas canvas, Rect rect) => _component.Render(canvas, rect);
}
