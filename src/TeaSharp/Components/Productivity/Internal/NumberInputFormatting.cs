using System.Globalization;

namespace TeaSharp.Components.Productivity.Internal;

internal static class NumberInputFormatting
{
    public static bool TryParse(string text, out double value)
    {
        var normalized = text.Trim();
        if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }

        normalized = normalized.Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    public static string Format(double value, int precision)
    {
        var safePrecision = Math.Clamp(precision, 0, 8);
        return value.ToString($"F{safePrecision}", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }

    public static double Clamp(double value, double min, double max)
    {
        if (max <= min)
        {
            return min;
        }

        return Math.Clamp(value, min, max);
    }

    public static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.000001;
    }
}
