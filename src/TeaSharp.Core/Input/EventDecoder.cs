using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Input;

public readonly record struct DecodeResult(int Consumed, IMessage? Message, bool NeedMoreData);

public sealed class EventDecoder
{
    private static ReadOnlySpan<char> CsiCursorFinals => "ABCDHF";
    private const int MouseModifierShiftMask = 0b0000_0100;
    private const int MouseModifierAltMask = 0b0000_1000;
    private const int MouseModifierCtrlMask = 0b0001_0000;
    private const int MouseMotionMask = 0b0010_0000;
    private const int MouseWheelMask = 0b0100_0000;
    private const int MouseExtendedButtonsMask = 0b1000_0000;

    public DecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (buffer.IsEmpty)
        {
            return new DecodeResult(0, null, false);
        }

        if (buffer[0] == 0x1B)
        {
            return DecodeEscape(buffer, timeoutExpired);
        }

        return DecodePlain(buffer);
    }

    private static DecodeResult DecodeEscape(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (buffer.Length == 1)
        {
            if (!timeoutExpired)
            {
                return new DecodeResult(0, null, true);
            }

            return new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false);
        }

        return buffer[1] switch
        {
            (byte)'[' => DecodeCsi(buffer, timeoutExpired),
            (byte)'O' => DecodeSs3(buffer, timeoutExpired),
            (byte)']' => DecodeOsc(buffer, timeoutExpired),
            _ => DecodeAltSequence(buffer, timeoutExpired),
        };
    }

    private static DecodeResult DecodeAltSequence(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        var ch = DecodeRune(buffer[1..], out var runeLen);
        if (ch is not null)
        {
            return new DecodeResult(1 + runeLen, new KeyPressMsg(KeyCode.Character, ch, KeyModifiers.Alt), false);
        }

        if (!timeoutExpired)
        {
            return new DecodeResult(0, null, true);
        }

        return new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false);
    }

    private static DecodeResult DecodeCsi(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (buffer.Length >= 3 && buffer[2] == (byte)'M')
        {
            return DecodeX10Mouse(buffer, timeoutExpired);
        }

        if (!TryFindFinalByte(buffer, 2, out var finalIndex))
        {
            if (!timeoutExpired)
            {
                return new DecodeResult(0, null, true);
            }

            return new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false);
        }

        var consumed = finalIndex + 1;
        var final = (char)buffer[finalIndex];
        var parameters = ParseIntegerParameters(buffer[2..finalIndex]);

        if (TryDecodeCsiMessage(final, parameters, out var message))
        {
            return new DecodeResult(consumed, message, false);
        }

        return new DecodeResult(consumed, new UnknownInputMsg(ToAscii(buffer[..consumed])), false);
    }

    private static DecodeResult DecodeSs3(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (!TryFindFinalByte(buffer, 2, out var finalIndex))
        {
            if (!timeoutExpired)
            {
                return new DecodeResult(0, null, true);
            }

            return new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false);
        }

        var consumed = finalIndex + 1;
        var final = (char)buffer[finalIndex];
        var parameters = ParseIntegerParameters(buffer[2..finalIndex]);
        var modifiers = parameters.Count == 0 ? KeyModifiers.None : ParseModifiers(parameters[^1]);

        if (TryDecodeSs3Key(final, modifiers, out var keyCode))
        {
            return new DecodeResult(consumed, new KeyPressMsg(keyCode, string.Empty, modifiers), false);
        }

        return new DecodeResult(consumed, new UnknownInputMsg(ToAscii(buffer[..consumed])), false);
    }

    private static DecodeResult DecodeOsc(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        for (var i = 2; i < buffer.Length; i++)
        {
            if (buffer[i] == 0x07)
            {
                return new DecodeResult(i + 1, null, false);
            }

            if (buffer[i] == 0x1B && i + 1 < buffer.Length && buffer[i + 1] == (byte)'\\')
            {
                return new DecodeResult(i + 2, null, false);
            }
        }

        if (!timeoutExpired)
        {
            return new DecodeResult(0, null, true);
        }

        return new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false);
    }

    private static DecodeResult DecodePlain(ReadOnlySpan<byte> buffer)
    {
        return buffer[0] switch
        {
            0x03 => new DecodeResult(1, new KeyPressMsg(KeyCode.Character, "c", KeyModifiers.Ctrl), false),
            0x09 => new DecodeResult(1, new KeyPressMsg(KeyCode.Tab), false),
            0x0A => new DecodeResult(1, new KeyPressMsg(KeyCode.Enter), false),
            0x0D => new DecodeResult(1, new KeyPressMsg(KeyCode.Enter), false),
            0x7F => new DecodeResult(1, new KeyPressMsg(KeyCode.Backspace), false),
            _ => DecodeUtf8(buffer),
        };
    }

    private static DecodeResult DecodeUtf8(ReadOnlySpan<byte> buffer)
    {
        var str = DecodeRune(buffer, out var len);
        if (str is null)
        {
            return new DecodeResult(1, new UnknownInputMsg($"0x{buffer[0]:X2}"), false);
        }

        return new DecodeResult(len, new KeyPressMsg(KeyCode.Character, str), false);
    }

    private static string? DecodeRune(ReadOnlySpan<byte> buffer, out int len)
    {
        len = 0;
        if (buffer.IsEmpty)
        {
            return null;
        }

        for (var count = 1; count <= Math.Min(4, buffer.Length); count++)
        {
            try
            {
                var text = Encoding.UTF8.GetString(buffer[..count]);
                if (!string.IsNullOrEmpty(text))
                {
                    len = count;
                    return text;
                }
            }
            catch (DecoderFallbackException)
            {
                // Continue trying until max UTF-8 rune size.
            }
        }

        return null;
    }

    private static bool TryDecodeCsiMessage(char final, IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;

        if (TryDecodeSgrMouse(final, parameters, out message))
        {
            return true;
        }

        if (final == '~' && TryDecodeBracketedPaste(parameters, out message))
        {
            return true;
        }

        if (final == 't' && TryDecodeWindowSize(parameters, out message))
        {
            return true;
        }

        if (TryDecodeFocus(final, out message))
        {
            return true;
        }

        if (CsiCursorFinals.Contains(final) && TryDecodeCsiCursorKey(final, parameters, out message))
        {
            return true;
        }

        if (final == '~' && TryDecodeCsiTildeKey(parameters, out message))
        {
            return true;
        }

        return false;
    }

    private static DecodeResult DecodeX10Mouse(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (buffer.Length < 6)
        {
            if (!timeoutExpired)
            {
                return new DecodeResult(0, null, true);
            }

            return new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false);
        }

        var cb = buffer[3] - 32;
        var cx = buffer[4] - 32;
        var cy = buffer[5] - 32;
        if (cb < 0 || cx <= 0 || cy <= 0)
        {
            return new DecodeResult(6, new UnknownInputMsg(ToAscii(buffer[..6])), false);
        }

        var isWheel = (cb & MouseWheelMask) != 0;
        var isMotion = !isWheel && (cb & MouseMotionMask) != 0;
        var eventType = isWheel
            ? MouseEventType.Wheel
            : isMotion
                ? MouseEventType.Motion
                : (cb & 0b11) == 0b11
                    ? MouseEventType.Release
                    : MouseEventType.Press;

        var message = new MouseMsg(
            eventType,
            DecodeMouseButton(cb, isWheel),
            cx - 1,
            cy - 1,
            ParseMouseModifiers(cb));

        return new DecodeResult(6, message, false);
    }

    private static bool TryDecodeBracketedPaste(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if (parameters.Count == 0 || parameters[0] is not int code)
        {
            return false;
        }

        if (code == 200)
        {
            message = new PasteStartMsg();
            return true;
        }

        if (code == 201)
        {
            message = new PasteEndMsg();
            return true;
        }

        return false;
    }

    private static bool TryDecodeWindowSize(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if (parameters.Count < 3 || parameters[0] != 8 || parameters[1] is not int rows || parameters[2] is not int cols)
        {
            return false;
        }

        message = new WindowSizeMsg(cols, rows);
        return true;
    }

    private static bool TryDecodeSgrMouse(char final, IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if ((final != 'M' && final != 'm')
            || parameters.Count < 3
            || parameters[0] is not int cb
            || parameters[1] is not int cx
            || parameters[2] is not int cy
            || cx <= 0
            || cy <= 0)
        {
            return false;
        }

        var isWheel = (cb & MouseWheelMask) != 0;
        var isMotion = !isWheel && (cb & MouseMotionMask) != 0;
        var eventType = isWheel
            ? MouseEventType.Wheel
            : isMotion
                ? MouseEventType.Motion
                : final == 'm'
                    ? MouseEventType.Release
                    : MouseEventType.Press;

        message = new MouseMsg(
            eventType,
            DecodeMouseButton(cb, isWheel),
            cx - 1,
            cy - 1,
            ParseMouseModifiers(cb));
        return true;
    }

    private static bool TryDecodeFocus(char final, out IMessage? message)
    {
        message = final switch
        {
            'I' => new FocusInMsg(),
            'O' => new FocusOutMsg(),
            _ => null,
        };

        return message is not null;
    }

    private static bool TryDecodeCsiCursorKey(char final, IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        var keyCode = final switch
        {
            'A' => KeyCode.Up,
            'B' => KeyCode.Down,
            'C' => KeyCode.Right,
            'D' => KeyCode.Left,
            'H' => KeyCode.Home,
            'F' => KeyCode.End,
            _ => KeyCode.Unknown,
        };

        if (keyCode == KeyCode.Unknown)
        {
            return false;
        }

        var modifier = GetCsiModifierParameter(parameters, final);
        var modifiers = ParseModifiers(modifier);
        message = new KeyPressMsg(keyCode, string.Empty, modifiers);
        return true;
    }

    private static bool TryDecodeCsiTildeKey(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if (parameters.Count == 0 || parameters[0] is not int code)
        {
            return false;
        }

        var keyCode = code switch
        {
            1 or 7 => KeyCode.Home,
            2 => KeyCode.Insert,
            3 => KeyCode.Delete,
            4 or 8 => KeyCode.End,
            5 => KeyCode.PageUp,
            6 => KeyCode.PageDown,
            _ => KeyCode.Unknown,
        };

        if (keyCode == KeyCode.Unknown)
        {
            return false;
        }

        var modifiers = parameters.Count > 1
            ? ParseModifiers(parameters[1])
            : KeyModifiers.None;

        message = new KeyPressMsg(keyCode, string.Empty, modifiers);
        return true;
    }

    private static bool TryDecodeSs3Key(char final, KeyModifiers modifiers, out KeyCode keyCode)
    {
        _ = modifiers;

        keyCode = final switch
        {
            'A' => KeyCode.Up,
            'B' => KeyCode.Down,
            'C' => KeyCode.Right,
            'D' => KeyCode.Left,
            'H' => KeyCode.Home,
            'F' => KeyCode.End,
            _ => KeyCode.Unknown,
        };

        return keyCode != KeyCode.Unknown;
    }

    private static KeyModifiers ParseModifiers(int? rawModifier)
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

    private static KeyModifiers ParseMouseModifiers(int encoded)
    {
        var modifiers = KeyModifiers.None;

        if ((encoded & MouseModifierShiftMask) != 0)
        {
            modifiers |= KeyModifiers.Shift;
        }

        if ((encoded & MouseModifierAltMask) != 0)
        {
            modifiers |= KeyModifiers.Alt;
        }

        if ((encoded & MouseModifierCtrlMask) != 0)
        {
            modifiers |= KeyModifiers.Ctrl;
        }

        return modifiers;
    }

    private static MouseButton DecodeMouseButton(int encoded, bool isWheel)
    {
        var low = encoded & 0b11;
        if (isWheel)
        {
            return low switch
            {
                0 => MouseButton.WheelUp,
                1 => MouseButton.WheelDown,
                2 => MouseButton.WheelLeft,
                3 => MouseButton.WheelRight,
                _ => MouseButton.None,
            };
        }

        if ((encoded & MouseExtendedButtonsMask) != 0)
        {
            return low switch
            {
                0 => MouseButton.Backward,
                1 => MouseButton.Forward,
                2 => MouseButton.Button10,
                3 => MouseButton.Button11,
                _ => MouseButton.None,
            };
        }

        return low switch
        {
            0 => MouseButton.Left,
            1 => MouseButton.Middle,
            2 => MouseButton.Right,
            _ => MouseButton.None,
        };
    }

    private static int? GetCsiModifierParameter(IReadOnlyList<int?> parameters, char final)
    {
        if (parameters.Count >= 2)
        {
            return parameters[^1];
        }

        if (parameters.Count == 1 && CsiCursorFinals.Contains(final))
        {
            return parameters[0];
        }

        return null;
    }

    private static List<int?> ParseIntegerParameters(ReadOnlySpan<byte> bytes)
    {
        var values = new List<int?>();
        if (bytes.IsEmpty)
        {
            return values;
        }

        var text = ToAscii(bytes);
        var parts = text.Split(';');
        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
            {
                values.Add(null);
                continue;
            }

            var segment = part.Trim();
            var start = 0;
            while (start < segment.Length && !char.IsDigit(segment[start]))
            {
                start++;
            }

            if (start >= segment.Length)
            {
                values.Add(null);
                continue;
            }

            if (int.TryParse(segment[start..], out var value))
            {
                values.Add(value);
            }
            else
            {
                values.Add(null);
            }
        }

        return values;
    }

    private static bool TryFindFinalByte(ReadOnlySpan<byte> buffer, int startIndex, out int finalIndex)
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

    private static string ToAscii(ReadOnlySpan<byte> bytes)
    {
        return Encoding.ASCII.GetString(bytes);
    }
}
