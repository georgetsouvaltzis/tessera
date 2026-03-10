using System;
using System.Text;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Input;

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
        var payload = DecoderCommon.ToAscii(payloadBytes);
        var separator = payload.IndexOf(';', StringComparison.Ordinal);
        if (separator <= 0 || !int.TryParse(payload[..separator], out var code))
        {
            return new DecodeResult(consumed, null, false);
        }

        var data = separator + 1 < payload.Length ? payload[(separator + 1)..] : string.Empty;
        if (code == 52 && TryDecodeClipboardOsc(data, out var clipboard))
        {
            return new DecodeResult(consumed, clipboard, false);
        }

        return code switch
        {
            10 => new DecodeResult(consumed, new ForegroundColorMsg(NormalizeOscColor(data)), false),
            11 => new DecodeResult(consumed, new BackgroundColorMsg(NormalizeOscColor(data)), false),
            12 => new DecodeResult(consumed, new CursorColorMsg(NormalizeOscColor(data)), false),
            _ => new DecodeResult(consumed, null, false),
        };
    }

    private static bool TryDecodeClipboardOsc(string data, out ClipboardMsg message)
    {
        message = default!;
        var parts = data.Split(';');
        if (parts.Length < 2)
        {
            return false;
        }

        var selection = parts[0].Trim();
        if (selection.Length != 1 || (selection[0] != 'c' && selection[0] != 'p'))
        {
            return false;
        }

        var encoded = parts[1].Trim();
        if (encoded == "?")
        {
            return false;
        }

        try
        {
            var bytes = Convert.FromBase64String(encoded);
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

        var chars = new List<char>(input.Length / 2);
        for (var i = 0; i + 1 < input.Length; i += 2)
        {
            if (!byte.TryParse(input.AsSpan(i, 2), System.Globalization.NumberStyles.HexNumber, null, out var value))
            {
                return input;
            }

            chars.Add((char)value);
        }

        return new string([.. chars]);
    }

    private static string NormalizeOscColor(string value)
    {
        var raw = value.Trim();
        if (raw.StartsWith("rgb:", StringComparison.OrdinalIgnoreCase))
        {
            var channels = raw[4..].Split('/');
            if (channels.Length != 3)
            {
                return raw;
            }

            if (!TryParseOscColorChannel(channels[0], out var r)
                || !TryParseOscColorChannel(channels[1], out var g)
                || !TryParseOscColorChannel(channels[2], out var b))
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

    private static bool TryParseOscColorChannel(string value, out byte channel)
    {
        channel = 0;
        var normalized = value.Trim();
        if (normalized.Length is < 1 or > 4)
        {
            return false;
        }

        if (!ushort.TryParse(normalized, System.Globalization.NumberStyles.HexNumber, null, out var parsed))
        {
            return false;
        }

        if (normalized.Length <= 2)
        {
            channel = (byte)parsed;
            return true;
        }

        var max = normalized.Length == 3 ? 0x0FFFu : 0xFFFFu;
        channel = (byte)Math.Round((parsed / (double)max) * 255d, MidpointRounding.AwayFromZero);
        return true;
    }
}
