using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using System.Globalization;

namespace TeaSharp.Components.Productivity.Internal;

internal static class DatePickerRenderer
{
    public static void Render(
        Canvas canvas,
        Rect rect,
        string title,
        bool focused,
        BorderStyle border,
        Thickness padding,
        DateOnly currentMonth,
        DateOnly selectedDate,
        DateOnly? hoveredDate,
        WidgetStatePalette dayStatePalette)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            border == BorderStyle.None ? null : focused ? $"{title} *" : title,
            border,
            padding);

        if (content.IsEmpty || content.Height < 3)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y, $"{currentMonth:yyyy-MM}", content.Width);
        if (content.Height == 1)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y + 1, "Mo Tu We Th Fr Sa Su", content.Width);
        if (content.Height < 3)
        {
            return;
        }

        var first = new DateOnly(currentMonth.Year, currentMonth.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
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

                var date = new DateOnly(currentMonth.Year, currentMonth.Month, day);
                var states = DatePickerStateResolver.ResolveDayStates(focused, selectedDate, hoveredDate, date);
                canvas.WriteText(
                    x,
                    content.Y + 2 + row,
                    dayStatePalette.Render(day.ToString(CultureInfo.InvariantCulture).PadLeft(2, ' '), states),
                    Math.Min(2, content.Right - x));
                day++;
            }
        }
    }
}
