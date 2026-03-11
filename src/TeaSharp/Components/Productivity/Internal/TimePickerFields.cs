namespace TeaSharp.Components.Productivity.Internal;

internal static class TimePickerFields
{
    public static TimePickerField Next(TimePickerField field)
    {
        return (TimePickerField)(((int)field + 1) % 3);
    }

    public static TimePickerField Previous(TimePickerField field)
    {
        return (TimePickerField)(((int)field + 2) % 3);
    }

    public static TimePickerField? FieldFromPointer(Rect content, int x, int y)
    {
        if (y < content.Y || y >= content.Bottom)
        {
            return null;
        }

        var index = x - content.X;
        if (index < 0)
        {
            return null;
        }

        return index switch
        {
            <= 2 => TimePickerField.Hour,
            <= 5 => TimePickerField.Minute,
            <= 8 => TimePickerField.Second,
            _ => null,
        };
    }

    public static TimeOnly Adjust(TimeOnly value, TimePickerField activeField, int hourStep, int minuteStep, int secondStep, int direction)
    {
        var delta = activeField switch
        {
            TimePickerField.Hour => TimeSpan.FromHours(hourStep * direction),
            TimePickerField.Minute => TimeSpan.FromMinutes(minuteStep * direction),
            _ => TimeSpan.FromSeconds(secondStep * direction),
        };

        return value.Add(delta);
    }
}
