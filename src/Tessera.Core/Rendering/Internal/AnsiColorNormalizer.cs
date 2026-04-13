using System.Globalization;

namespace Tessera.Core.Rendering.Internal;

internal static class AnsiColorNormalizer
{
    public static string? NormalizeHex(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var value = input.Trim();
        if (value.StartsWith("rgb:", StringComparison.OrdinalIgnoreCase))
        {
            var channels = value[4..].Split('/');
            if (channels.Length != 3)
            {
                return null;
            }

            if (!TryParseRgbChannel(channels[0], out var r)
                || !TryParseRgbChannel(channels[1], out var g)
                || !TryParseRgbChannel(channels[2], out var b))
            {
                return null;
            }

            return $"#{r:X2}{g:X2}{b:X2}";
        }

        if (value[0] == '#')
        {
            value = value[1..];
        }

        if (value.Length == 3
            && byte.TryParse(new string(value[0], 2), NumberStyles.HexNumber, null, out var shortR)
            && byte.TryParse(new string(value[1], 2), NumberStyles.HexNumber, null, out var shortG)
            && byte.TryParse(new string(value[2], 2), NumberStyles.HexNumber, null, out var shortB))
        {
            return $"#{shortR:X2}{shortG:X2}{shortB:X2}";
        }

        if (value.Length == 6
            && byte.TryParse(value[..2], NumberStyles.HexNumber, null, out var r6)
            && byte.TryParse(value[2..4], NumberStyles.HexNumber, null, out var g6)
            && byte.TryParse(value[4..], NumberStyles.HexNumber, null, out var b6))
        {
            return $"#{r6:X2}{g6:X2}{b6:X2}";
        }

        return null;
    }

    private static bool TryParseRgbChannel(string value, out byte result)
    {
        result = 0;
        var normalized = value.Trim();
        if (normalized.Length is < 1 or > 4)
        {
            return false;
        }

        if (!ushort.TryParse(normalized, NumberStyles.HexNumber, null, out var parsed))
        {
            return false;
        }

        if (normalized.Length <= 2)
        {
            result = (byte)parsed;
            return true;
        }

        var max = normalized.Length == 3 ? 0x0FFFu : 0xFFFFu;
        result = (byte)Math.Round(parsed / (double)max * 255d, MidpointRounding.AwayFromZero);
        return true;
    }
}
