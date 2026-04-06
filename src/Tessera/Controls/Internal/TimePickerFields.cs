using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Components.Styling;
namespace Tessera.Controls.Internal;

internal static class TimePickerFields
{
    public static TimeField Next(TimeField field)
    {
        return (TimeField)(((int)field + 1) % 3);
    }

    public static TimeField Previous(TimeField field)
    {
        return (TimeField)(((int)field + 2) % 3);
    }

    public static TimeField? FieldFromPointer(Rect content, int x, int y)
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
            <= 2 => TimeField.Hour,
            <= 5 => TimeField.Minute,
            <= 8 => TimeField.Second,
            _ => null,
        };
    }

    public static TimeOnly Adjust(TimeOnly value, TimeField activeField, int hourStep, int minuteStep, int secondStep, int direction)
    {
        var delta = activeField switch
        {
            TimeField.Hour => TimeSpan.FromHours(hourStep * direction),
            TimeField.Minute => TimeSpan.FromMinutes(minuteStep * direction),
            _ => TimeSpan.FromSeconds(secondStep * direction),
        };

        return value.Add(delta);
    }
}
