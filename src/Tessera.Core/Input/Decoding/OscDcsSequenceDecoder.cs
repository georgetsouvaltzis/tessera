using System.Globalization;
using System.Text;
using Tessera.Core.Messages;

namespace Tessera.Core.Input.Decoding;

internal static class OscDcsSequenceDecoder
{
    public static DecodeResult DecodeOsc(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        for (var i = 2; i < buffer.Length; i++)
        {
            if (buffer[i] == 0x07)
            {
                return ParseOscResult(buffer[2..i], i + 1);
            }

            if (buffer[i] == 0x1B && i + 1 < buffer.Length && buffer[i + 1] == (byte)'\\')
            {
                return ParseOscResult(buffer[2..i], i + 2);
            }
        }

        return timeoutExpired
            ? new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false)
            : new DecodeResult(0, null, true);
    }

    public static DecodeResult DecodeDcs(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        for (var i = 2; i < buffer.Length - 1; i++)
        {
            if (buffer[i] == 0x1B && buffer[i + 1] == (byte)'\\')
            {
                return ParseDcsResult(buffer[2..i], i + 2);
            }
        }

        return timeoutExpired
            ? new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false)
            : new DecodeResult(0, null, true);
    }

    private static DecodeResult ParseOscResult(ReadOnlySpan<byte> payloadBytes, int consumed)
    {
        if (!TryReadOscCode(payloadBytes, out var code, out var separator))
        {
            return new DecodeResult(consumed, null, false);
        }

        var data = separator + 1 < payloadBytes.Length
            ? DecoderCommon.ToAscii(payloadBytes[(separator + 1)..])
            : string.Empty;
        if (code == 52 && TryDecodeClipboardOsc(data, out var clipboard))
        {
            return new DecodeResult(consumed, clipboard, false);
        }

        return code switch
        {
            10 => new DecodeResult(consumed, new ForegroundColorMsg(NormalizeOscColor(data)), false),
            11 => new DecodeResult(consumed, new BackgroundColorMsg(NormalizeOscColor(data)), false),
            12 => new DecodeResult(consumed, new CursorColorMsg(NormalizeOscColor(data)), false),
            _ => new DecodeResult(consumed, null, false)
        };
    }

    private static bool TryDecodeClipboardOsc(string data, out ClipboardMsg message)
    {
        message = default!;
        var firstSeparator = data.IndexOf(';', StringComparison.Ordinal);
        if (firstSeparator <= 0 || firstSeparator == data.Length - 1)
        {
            return false;
        }

        var selection = data.AsSpan(0, firstSeparator).Trim();
        if (selection.Length != 1 || (selection[0] != 'c' && selection[0] != 'p'))
        {
            return false;
        }

        var encodedSpan = data.AsSpan(firstSeparator + 1);
        var secondSeparator = encodedSpan.IndexOf(';');
        if (secondSeparator >= 0)
        {
            encodedSpan = encodedSpan[..secondSeparator];
        }

        encodedSpan = encodedSpan.Trim();
        if (encodedSpan.SequenceEqual("?".AsSpan()))
        {
            return false;
        }

        try
        {
            var bytes = Convert.FromBase64String(encodedSpan.ToString());
            var content = Encoding.UTF8.GetString(bytes);
            message = new ClipboardMsg(content, selection[0]);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static DecodeResult ParseDcsResult(ReadOnlySpan<byte> payloadBytes, int consumed)
    {
        var payload = DecoderCommon.ToAscii(payloadBytes);
        if (!payload.StartsWith("1+r", StringComparison.Ordinal))
        {
            return new DecodeResult(consumed, null, false);
        }

        var capabilityPayload = payload[3..];
        var separator = capabilityPayload.IndexOf('=', StringComparison.Ordinal);
        var encodedName = separator >= 0
            ? capabilityPayload[..separator]
            : capabilityPayload;
        var encodedValue = separator >= 0 && separator + 1 < capabilityPayload.Length
            ? capabilityPayload[(separator + 1)..]
            : null;

        var name = DecodeHexAscii(encodedName);
        var value = encodedValue is null ? null : DecodeHexAscii(encodedValue);
        return new DecodeResult(consumed, new CapabilityMsg(name, value, payload), false);
    }

    private static string DecodeHexAscii(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var pairCount = input.Length / 2;
        if (pairCount == 0)
        {
            return string.Empty;
        }

        var chars = pairCount <= 128 ? stackalloc char[pairCount] : new char[pairCount];
        for (var i = 0; i + 1 < input.Length; i += 2)
        {
            if (!TryDecodeHexPair(input[i], input[i + 1], out var value))
            {
                return input;
            }

            chars[i / 2] = (char)value;
        }

        return new string(chars);
    }

    private static string NormalizeOscColor(string value)
    {
        var raw = value.Trim();
        if (raw.StartsWith("rgb:", StringComparison.OrdinalIgnoreCase))
        {
            var channels = raw.AsSpan(4);
            var firstSeparator = channels.IndexOf('/');
            if (firstSeparator <= 0 || firstSeparator >= channels.Length - 1)
            {
                return raw;
            }

            var remaining = channels[(firstSeparator + 1)..];
            var secondSeparator = remaining.IndexOf('/');
            if (secondSeparator <= 0 || secondSeparator >= remaining.Length - 1)
            {
                return raw;
            }

            var redText = channels[..firstSeparator];
            var greenText = remaining[..secondSeparator];
            var blueText = remaining[(secondSeparator + 1)..];
            if (!TryParseOscColorChannel(redText, out var r)
                || !TryParseOscColorChannel(greenText, out var g)
                || !TryParseOscColorChannel(blueText, out var b))
            {
                return raw;
            }

            return $"#{r:X2}{g:X2}{b:X2}";
        }

        if (raw.Length > 0 && raw[0] == '#')
        {
            return raw.Length == 7 ? raw.ToUpperInvariant() : raw;
        }

        return raw;
    }

    private static bool TryParseOscColorChannel(ReadOnlySpan<char> value, out byte channel)
    {
        channel = 0;
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
            channel = (byte)parsed;
            return true;
        }

        var max = normalized.Length == 3 ? 0x0FFFu : 0xFFFFu;
        channel = (byte)Math.Round(parsed / (double)max * 255d, MidpointRounding.AwayFromZero);
        return true;
    }

    private static bool TryReadOscCode(ReadOnlySpan<byte> payloadBytes, out int code, out int separator)
    {
        code = 0;
        separator = -1;
        if (payloadBytes.IsEmpty)
        {
            return false;
        }

        for (var index = 0; index < payloadBytes.Length; index++)
        {
            if (payloadBytes[index] == (byte)';')
            {
                separator = index;
                break;
            }

            if (payloadBytes[index] is < (byte)'0' or > (byte)'9')
            {
                return false;
            }

            code = code * 10 + (payloadBytes[index] - (byte)'0');
        }

        return separator > 0;
    }

    private static bool TryDecodeHexPair(char high, char low, out byte value)
    {
        value = 0;
        if (!TryParseHexNibble(high, out var highNibble) || !TryParseHexNibble(low, out var lowNibble))
        {
            return false;
        }

        value = (byte)((highNibble << 4) | lowNibble);
        return true;
    }

    private static bool TryParseHexNibble(char value, out int nibble)
    {
        nibble = value switch
        {
            >= '0' and <= '9' => value - '0',
            >= 'a' and <= 'f' => 10 + (value - 'a'),
            >= 'A' and <= 'F' => 10 + (value - 'A'),
            _ => -1
        };

        return nibble >= 0;
    }
}
