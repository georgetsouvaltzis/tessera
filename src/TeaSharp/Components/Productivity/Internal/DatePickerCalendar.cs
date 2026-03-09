namespace TeaSharp.Components.Internal;

internal static class DatePickerCalendar
{
    public static Rect ResolveContentRect(Rect bounds, bool showBorder)
    {
        return showBorder ? bounds.Inset(1, 1) : bounds;
    }

    public static bool TryGetDateAtPointer(DateOnly currentMonth, Rect content, int x, int y, out DateOnly date)
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
        if (col is < 0 or > 6)
        {
            return false;
        }

        var first = new DateOnly(currentMonth.Year, currentMonth.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
        var cell = (row * 7) + col;
        var day = cell - startOffset + 1;
        if (day < 1 || day > daysInMonth)
        {
            return false;
        }

        date = new DateOnly(currentMonth.Year, currentMonth.Month, day);
        return true;
    }
}
