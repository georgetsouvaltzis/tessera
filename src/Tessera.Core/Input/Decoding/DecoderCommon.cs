using System.Buffers;
using System.Text;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;

namespace Tessera.Core.Input;

internal static class DecoderCommon
{
    private static ReadOnlySpan<char> CsiCursorFinals => "ABCDHF";

    public static bool IsCsiCursorFinal(char final)
    {
        return CsiCursorFinals.Contains(final);
    }

    public static bool TryDecodeControlByte(byte value, out IMessage? message)
    {
        message = null;
        switch (value)
        {
            case 0x00:
                message = new KeyPressMsg(KeyCode.Character, "@", KeyModifiers.Ctrl);
                return true;
            case >= 0x01 and <= 0x08:
            case 0x0B:
            case 0x0C:
            case >= 0x0E and <= 0x1A:
            {
                var text = ((char)('a' + value - 1)).ToString();
                message = new KeyPressMsg(KeyCode.Character, text, KeyModifiers.Ctrl);
                return true;
            }
            case 0x1C:
                message = new KeyPressMsg(KeyCode.Character, "\\", KeyModifiers.Ctrl);
                return true;
            case 0x1D:
                message = new KeyPressMsg(KeyCode.Character, "]", KeyModifiers.Ctrl);
                return true;
            case 0x1E:
                message = new KeyPressMsg(KeyCode.Character, "^", KeyModifiers.Ctrl);
                return true;
            case 0x1F:
                message = new KeyPressMsg(KeyCode.Character, "_", KeyModifiers.Ctrl);
                return true;
            default:
                return false;
        }
    }

    public static bool TryDecodeRune(ReadOnlySpan<byte> buffer, out string value, out int len, out bool needMoreData)
    {
        value = string.Empty;
        len = 0;
        needMoreData = false;

        if (buffer.IsEmpty)
        {
            needMoreData = true;
            return false;
        }

        var status = Rune.DecodeFromUtf8(buffer, out var rune, out len);
        if (status == OperationStatus.Done)
        {
            value = rune.ToString();
            return true;
        }

        if (status == OperationStatus.NeedMoreData)
        {
            needMoreData = true;
            return false;
        }

        return false;
    }

    public static bool TryParseInteger(ReadOnlySpan<byte> bytes, out int value)
    {
        value = 0;
        var seen = false;
        foreach (var b in bytes)
        {
            if (b is < (byte)'0' or > (byte)'9')
            {
                continue;
            }

            seen = true;
            value = (value * 10) + (b - (byte)'0');
        }

        return seen;
    }

    public static List<int?> ParseIntegerParameters(ReadOnlySpan<byte> bytes)
    {
        var values = new List<int?>(8);
        if (bytes.IsEmpty)
        {
            return values;
        }

        var segmentStart = 0;
        for (var i = 0; i <= bytes.Length; i++)
        {
            if (i < bytes.Length && bytes[i] is not (byte)';' and not (byte)':')
            {
                continue;
            }

            values.Add(ParseIntegerSegment(bytes[segmentStart..i]));
            segmentStart = i + 1;
        }

        return values;
    }

    public static bool TryFindFinalByte(ReadOnlySpan<byte> buffer, int startIndex, out int finalIndex)
    {
        finalIndex = -1;
        for (var i = startIndex; i < buffer.Length; i++)
        {
            var b = buffer[i];
            if (b >= 0x40 && b <= 0x7E)
            {
                finalIndex = i;
                return true;
            }
        }

        return false;
    }

    public static string ToAscii(ReadOnlySpan<byte> bytes)
    {
        return Encoding.ASCII.GetString(bytes);
    }

    private static int? ParseIntegerSegment(ReadOnlySpan<byte> segment)
    {
        if (segment.IsEmpty)
        {
            return null;
        }

        var value = 0;
        var foundDigit = false;
        var readingDigits = false;

        foreach (var b in segment)
        {
            if (b is >= (byte)'0' and <= (byte)'9')
            {
                value = (value * 10) + (b - (byte)'0');
                foundDigit = true;
                readingDigits = true;
                continue;
            }

            if (readingDigits)
            {
                break;
            }
        }

        return foundDigit ? value : null;
    }

    public static KeyModifiers ParseModifiers(int? rawModifier)
    {
        if (rawModifier is null or <= 1)
        {
            return KeyModifiers.None;
        }

        var encoded = rawModifier.Value - 1;
        var modifiers = KeyModifiers.None;

        if ((encoded & 0b0001) != 0)
        {
            modifiers |= KeyModifiers.Shift;
        }

        if ((encoded & 0b0010) != 0)
        {
            modifiers |= KeyModifiers.Alt;
        }

        if ((encoded & 0b0100) != 0)
        {
            modifiers |= KeyModifiers.Ctrl;
        }

        if ((encoded & 0b1000) != 0)
        {
            modifiers |= KeyModifiers.Meta;
        }

        return modifiers;
    }

    public static int? GetCsiModifierParameter(IReadOnlyList<int?> parameters, char final)
    {
        if (parameters.Count >= 2)
        {
            return parameters[^1];
        }

        if (parameters.Count == 1 && IsCsiCursorFinal(final))
        {
            return parameters[0];
        }

        return null;
    }

    public static bool TryCreateKeyMessageFromCodePoint(
        int codePoint,
        KeyModifiers modifiers,
        int? eventType,
        out IMessage? message)
    {
        message = null;
        if (!Rune.IsValid(codePoint))
        {
            return false;
        }

        var keyCode = codePoint switch
        {
            9 => KeyCode.Tab,
            10 or 13 => KeyCode.Enter,
            27 => KeyCode.Escape,
            127 => KeyCode.Backspace,
            _ => KeyCode.Character,
        };

        var text = keyCode == KeyCode.Character
            ? new Rune(codePoint).ToString()
            : string.Empty;

        if (eventType == 3)
        {
            message = new KeyReleaseMsg(keyCode, text, modifiers);
            return true;
        }

        message = new KeyPressMsg(keyCode, text, modifiers, IsRepeat: eventType == 2);
        return true;
    }
}
