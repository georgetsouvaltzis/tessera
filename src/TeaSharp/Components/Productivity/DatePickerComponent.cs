using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Components.Internal;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class DatePickerComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private DateOnly? _hoveredDate;

    public string Title { get; set; } = "Date Picker";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public DateOnly SelectedDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    public DateOnly CurrentMonth { get; private set; } = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

    public DateOnly? LastCommittedDate { get; private set; }

    public KeyBinding PreviousDayKey { get; set; } = new("left/h", "previous day", "left", "h");

    public KeyBinding NextDayKey { get; set; } = new("right/l", "next day", "right", "l");

    public KeyBinding PreviousWeekKey { get; set; } = new("up/k", "previous week", "up", "k");

    public KeyBinding NextWeekKey { get; set; } = new("down/j", "next week", "down", "j");

    public KeyBinding PreviousMonthKey { get; set; } = new("pageup", "previous month", "pageup");

    public KeyBinding NextMonthKey { get; set; } = new("pagedown", "next month", "pagedown");

    public KeyBinding CommitKey { get; set; } = new("enter/space", "commit date", "enter", "space");

    public WidgetStatePalette DayStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetDate(DateOnly date)
    {
        SelectedDate = date;
        CurrentMonth = new DateOnly(date.Year, date.Month, 1);
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (PreviousDayKey.Matches(key))
        {
            SetDate(SelectedDate.AddDays(-1));
            return true;
        }

        if (NextDayKey.Matches(key))
        {
            SetDate(SelectedDate.AddDays(1));
            return true;
        }

        if (PreviousWeekKey.Matches(key))
        {
            SetDate(SelectedDate.AddDays(-7));
            return true;
        }

        if (NextWeekKey.Matches(key))
        {
            SetDate(SelectedDate.AddDays(7));
            return true;
        }

        if (PreviousMonthKey.Matches(key))
        {
            SetDate(SelectedDate.AddMonths(-1));
            return true;
        }

        if (NextMonthKey.Matches(key))
        {
            SetDate(SelectedDate.AddMonths(1));
            return true;
        }

        if (CommitKey.Matches(key))
        {
            LastCommittedDate = SelectedDate;
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

        var content = DatePickerCalendar.ResolveContentRect(bounds, ShowBorder);
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
                changed |= SetHoveredDate(null);
            }

            return changed;
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (wheel.Button == MouseButton.WheelUp)
            {
                SetDate(SelectedDate.AddMonths(-1));
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                SetDate(SelectedDate.AddMonths(1));
                changed = true;
            }
        }

        if (!DatePickerCalendar.TryGetDateAtPointer(CurrentMonth, content, message.X, message.Y, out var hovered))
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredDate(null);
            }

            return changed;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredDate(hovered);
            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredDate(hovered);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && hovered != SelectedDate)
            {
                SetDate(hovered);
                changed = true;
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        DatePickerRenderer.Render(canvas, rect, Title, Focused, ShowBorder, CurrentMonth, SelectedDate, _hoveredDate, DayStatePalette);
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
