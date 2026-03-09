using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
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

        if (!TryGetDateAtPointer(content, message.X, message.Y, out var hovered))
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
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
            content = clipped.Inset(1, 1);
        }
        else
        {
            content = clipped;
        }

        if (content.IsEmpty || content.Height < 3)
        {
            return;
        }

        var monthLabel = $"{CurrentMonth:yyyy-MM}";
        canvas.WriteText(content.X, content.Y, monthLabel, content.Width);
        if (content.Height == 1)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y + 1, "Mo Tu We Th Fr Sa Su", content.Width);
        if (content.Height < 3)
        {
            return;
        }

        var first = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
        var day = 1;
        for (var row = 0; row < 6 && (content.Y + 2 + row) < content.Bottom; row++)
        {
            for (var col = 0; col < 7; col++)
            {
                var cell = row * 7 + col;
                if (cell < startOffset || day > daysInMonth)
                {
                    continue;
                }

                var x = content.X + (col * 3);
                if (x + 1 >= content.Right)
                {
                    continue;
                }

                var text = day.ToString().PadLeft(2, ' ');
                var date = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, day);
                var states = new List<WidgetVisualState>(5);
                if (date == SelectedDate)
                {
                    states.Add(WidgetVisualState.Selected);
                    states.Add(WidgetVisualState.Cursor);
                }

                if (Focused)
                {
                    states.Add(WidgetVisualState.Focused);
                }

                if (_hoveredDate.HasValue && _hoveredDate.Value == date)
                {
                    states.Add(WidgetVisualState.Hovered);
                }

                canvas.WriteText(x, content.Y + 2 + row, DayStatePalette.Render(text, states), Math.Min(2, content.Right - x));
                day++;
            }
        }
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

    private bool TryGetDateAtPointer(Rect content, int x, int y, out DateOnly date)
    {
        date = default;
        var row = y - (content.Y + 2);
        if (row < 0 || row >= 6)
        {
            return false;
        }

        var relativeX = x - content.X;
        if (relativeX < 0)
        {
            return false;
        }

        var col = relativeX / 3;
        if (col < 0 || col > 6)
        {
            return false;
        }

        var first = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
        var cell = (row * 7) + col;
        var day = cell - startOffset + 1;
        if (day < 1 || day > daysInMonth)
        {
            return false;
        }

        date = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, day);
        return true;
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

