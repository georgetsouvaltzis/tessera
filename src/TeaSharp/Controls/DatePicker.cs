using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a control for selecting a calendar date.
/// </summary>
public sealed class DatePicker : Control
{
    private readonly WidgetStatePalette _dayStatePalette = WidgetStatePalette.CreateDefault();
    private DateOnly? _hoveredDate;

    public event EventHandler<DateChangedEventArgs>? DateChanged;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Date Picker";

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

    public DateOnly SelectedDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    public DateOnly CurrentMonth { get; private set; } = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

    public DateOnly? LastCommittedDate { get; private set; }

    public override bool IsFocused
    {
        get;
        set;
    }

    public override bool IsDisabled
    {
        get;
        set;
    }

    public override bool IsReadOnly
    {
        get;
        set;
    }

    public void SetDate(DateOnly date)
    {
        var previousDate = SelectedDate;
        SelectedDate = date;
        CurrentMonth = new DateOnly(date.Year, date.Month, 1);
        if (previousDate != SelectedDate)
        {
            DateChanged?.Invoke(this, new DateChangedEventArgs(previousDate, SelectedDate));
        }
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            SetDate(SelectedDate.AddDays(-1));
            return true;
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            SetDate(SelectedDate.AddDays(1));
            return true;
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            SetDate(SelectedDate.AddDays(-7));
            return true;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            SetDate(SelectedDate.AddDays(7));
            return true;
        }

        if (key.Is(Key.PageUp))
        {
            SetDate(SelectedDate.AddMonths(-1));
            return true;
        }

        if (key.Is(Key.PageDown))
        {
            SetDate(SelectedDate.AddMonths(1));
            return true;
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            LastCommittedDate = SelectedDate;
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = DatePickerCalendar.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredDate(null);
            }

            return changed || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelUp)
            {
                SetDate(SelectedDate.AddMonths(-1));
                changed = true;
            }
            else if (pointer.Button == PointerButton.WheelDown)
            {
                SetDate(SelectedDate.AddMonths(1));
                changed = true;
            }
        }

        if (!DatePickerCalendar.TryGetDateAtPointer(CurrentMonth, content, pointer.X, pointer.Y, out var hovered))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredDate(null);
            }

            return changed || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            changed |= SetHoveredDate(hovered);
            return changed;
        }

        if (pointer.Kind == PointerEventKind.Press)
        {
            changed |= SetHoveredDate(hovered);
            if (pointer.Button == PointerButton.Left && hovered != SelectedDate)
            {
                SetDate(hovered);
                changed = true;
            }
        }

        return changed || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        DatePickerRenderer.Render(canvas, rect, Title, IsFocused, Border, Padding, CurrentMonth, SelectedDate, _hoveredDate, _dayStatePalette);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(20, Title.Length + 4) + Padding.Horizontal;
        var height = 8 + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool SetHoveredDate(DateOnly? date)
    {
        if (_hoveredDate == date)
        {
            return false;
        }

        _hoveredDate = date;
        return true;
    }
}
