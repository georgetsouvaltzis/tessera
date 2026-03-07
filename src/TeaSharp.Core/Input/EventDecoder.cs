using System.Buffers;
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

        return DecodePlain(buffer, timeoutExpired);
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
        if (buffer.Length >= 2 && buffer[1] == 0x1B)
        {
            var nested = DecodeEscape(buffer[1..], timeoutExpired);
            if (nested.NeedMoreData && nested.Consumed == 0)
            {
                return new DecodeResult(0, null, true);
            }

            if (nested.Message is KeyPressMsg nestedKey)
            {
                return new DecodeResult(
                    1 + nested.Consumed,
                    nestedKey with { Modifiers = nestedKey.Modifiers | KeyModifiers.Alt },
                    false);
            }

            if (nested.Message is KeyReleaseMsg nestedRelease)
            {
                return new DecodeResult(
                    1 + nested.Consumed,
                    nestedRelease with { Modifiers = nestedRelease.Modifiers | KeyModifiers.Alt },
                    false);
            }

            if (nested.Consumed > 0)
            {
                return new DecodeResult(1 + nested.Consumed, nested.Message, false);
            }
        }

        if (buffer.Length >= 2)
        {
            var second = buffer[1];
            if (second == 0x7F || second == 0x08)
            {
                return new DecodeResult(2, new KeyPressMsg(KeyCode.Backspace, string.Empty, KeyModifiers.Alt), false);
            }

            if (second == 0x09)
            {
                return new DecodeResult(2, new KeyPressMsg(KeyCode.Tab, string.Empty, KeyModifiers.Alt), false);
            }

            if (second is 0x0A or 0x0D)
            {
                return new DecodeResult(2, new KeyPressMsg(KeyCode.Enter, string.Empty, KeyModifiers.Alt), false);
            }

            if (TryDecodeControlByte(second, out var controlMessage) && controlMessage is KeyPressMsg controlKey)
            {
                return new DecodeResult(
                    2,
                    controlKey with { Modifiers = controlKey.Modifiers | KeyModifiers.Alt },
                    false);
            }
        }

        if (TryDecodeRune(buffer[1..], out var ch, out var runeLen, out var needMoreData))
        {
            return new DecodeResult(1 + runeLen, new KeyPressMsg(KeyCode.Character, ch, KeyModifiers.Alt), false);
        }

        if (needMoreData && !timeoutExpired)
        {
            return new DecodeResult(0, null, true);
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

    private static DecodeResult DecodePlain(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        var first = buffer[0];
        if (TryDecodeControlByte(first, out var controlMessage))
        {
            return new DecodeResult(1, controlMessage, false);
        }

        return first switch
        {
            0x09 => new DecodeResult(1, new KeyPressMsg(KeyCode.Tab), false),
            0x0A => new DecodeResult(1, new KeyPressMsg(KeyCode.Enter), false),
            0x0D => new DecodeResult(1, new KeyPressMsg(KeyCode.Enter), false),
            0x7F => new DecodeResult(1, new KeyPressMsg(KeyCode.Backspace), false),
            _ => DecodeUtf8(buffer, timeoutExpired),
        };
    }

    private static bool TryDecodeControlByte(byte value, out IMessage? message)
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

    private static DecodeResult DecodeUtf8(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (TryDecodeRune(buffer, out var str, out var len, out var needMoreData))
        {
            return new DecodeResult(len, new KeyPressMsg(KeyCode.Character, str), false);
        }

        if (needMoreData && !timeoutExpired)
        {
            return new DecodeResult(0, null, true);
        }

        return new DecodeResult(1, new UnknownInputMsg($"0x{buffer[0]:X2}"), false);
    }

    private static bool TryDecodeRune(ReadOnlySpan<byte> buffer, out string value, out int len, out bool needMoreData)
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

        if (final == 'y' && TryDecodeModeReport(parameters, out message))
        {
            return true;
        }

        if (TryDecodeFocus(final, out message))
        {
            return true;
        }

        if (final == 'u' && TryDecodeCsiUnicodeKey(parameters, out message))
        {
            return true;
        }

        if (final == 'Z' && TryDecodeCsiBackTab(parameters, out message))
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
        var (button, modifiers) = DecodeMouseButtonAndModifiers(cb, isWheel);

        var message = CreateMouseMessage(
            eventType,
            button,
            cx - 1,
            cy - 1,
            modifiers);

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

    private static bool TryDecodeModeReport(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if (parameters.Count < 2
            || parameters[0] is not int mode
            || parameters[1] is not int stateRaw)
        {
            return false;
        }

        var state = stateRaw switch
        {
            0 => ModeReportState.Unsupported,
            1 => ModeReportState.Set,
            2 => ModeReportState.Reset,
            3 => ModeReportState.PermanentlySet,
            4 => ModeReportState.PermanentlyReset,
            _ => ModeReportState.Unknown,
        };

        message = new ModeReportMsg(mode, state);
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
        var (button, modifiers) = DecodeMouseButtonAndModifiers(cb, isWheel);

        message = CreateMouseMessage(
            eventType,
            button,
            cx - 1,
            cy - 1,
            modifiers);
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

        if (code == 27 && parameters.Count >= 3 && parameters[2] is int modifyOtherKeyCodePoint)
        {
            return TryCreateKeyMessageFromCodePoint(
                modifyOtherKeyCodePoint,
                ParseModifiers(parameters[1]),
                eventType: null,
                out message);
        }

        var keyCode = code switch
        {
            1 or 7 => KeyCode.Home,
            2 => KeyCode.Insert,
            3 => KeyCode.Delete,
            4 or 8 => KeyCode.End,
            5 => KeyCode.PageUp,
            6 => KeyCode.PageDown,
            11 => KeyCode.F1,
            12 => KeyCode.F2,
            13 => KeyCode.F3,
            14 => KeyCode.F4,
            15 => KeyCode.F5,
            17 => KeyCode.F6,
            18 => KeyCode.F7,
            19 => KeyCode.F8,
            20 => KeyCode.F9,
            21 => KeyCode.F10,
            23 => KeyCode.F11,
            24 => KeyCode.F12,
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

    private static bool TryDecodeCsiUnicodeKey(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if (parameters.Count == 0 || parameters[0] is not int codePoint)
        {
            return false;
        }

        var modifiers = parameters.Count > 1
            ? ParseModifiers(parameters[1])
            : KeyModifiers.None;
        var eventType = parameters.Count > 2 ? parameters[2] : null;

        return TryCreateKeyMessageFromCodePoint(codePoint, modifiers, eventType, out message);
    }

    private static bool TryDecodeCsiBackTab(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        var modifier = GetCsiModifierParameter(parameters, 'Z');
        var modifiers = modifier is null
            ? KeyModifiers.Shift
            : ParseModifiers(modifier) | KeyModifiers.Shift;

        message = new KeyPressMsg(KeyCode.Tab, string.Empty, modifiers);
        return true;
    }

    private static bool TryCreateKeyMessageFromCodePoint(
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

        var isRepeat = eventType == 2;
        message = new KeyPressMsg(keyCode, text, modifiers, IsRepeat: isRepeat);

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
            'P' => KeyCode.F1,
            'Q' => KeyCode.F2,
            'R' => KeyCode.F3,
            'S' => KeyCode.F4,
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
            var extendedButtonIndex = (encoded & ~(MouseWheelMask | MouseMotionMask)) - MouseExtendedButtonsMask;
            return DecodeExtendedMouseButton(extendedButtonIndex);
        }

        return low switch
        {
            0 => MouseButton.Left,
            1 => MouseButton.Middle,
            2 => MouseButton.Right,
            _ => MouseButton.None,
        };
    }

    private static (MouseButton Button, KeyModifiers Modifiers) DecodeMouseButtonAndModifiers(int encoded, bool isWheel)
    {
        var button = DecodeMouseButton(encoded, isWheel);
        if (isWheel || button is MouseButton.None)
        {
            return (button, ParseMouseModifiers(encoded));
        }

        if ((encoded & MouseExtendedButtonsMask) == 0)
        {
            return (button, ParseMouseModifiers(encoded));
        }

        var extendedButtonIndex = (encoded & ~(MouseWheelMask | MouseMotionMask)) - MouseExtendedButtonsMask;
        if (extendedButtonIndex <= 3)
        {
            return (button, ParseMouseModifiers(encoded));
        }

        // Higher extended button indices overlap with modifier bits in the legacy encoding.
        // Prefer stable high-button mapping and drop ambiguous modifier flags.
        return (button, KeyModifiers.None);
    }

    private static MouseButton DecodeExtendedMouseButton(int extendedButtonIndex)
    {
        return extendedButtonIndex switch
        {
            0 => MouseButton.Backward,
            1 => MouseButton.Forward,
            2 => MouseButton.Button10,
            3 => MouseButton.Button11,
            4 => MouseButton.Button12,
            5 => MouseButton.Button13,
            6 => MouseButton.Button14,
            7 => MouseButton.Button15,
            8 => MouseButton.Button16,
            9 => MouseButton.Button17,
            10 => MouseButton.Button18,
            11 => MouseButton.Button19,
            12 => MouseButton.Button20,
            13 => MouseButton.Button21,
            14 => MouseButton.Button22,
            15 => MouseButton.Button23,
            16 => MouseButton.Button24,
            _ => MouseButton.None,
        };
    }

    private static MouseMsg CreateMouseMessage(
        MouseEventType eventType,
        MouseButton button,
        int x,
        int y,
        KeyModifiers modifiers)
    {
        return eventType switch
        {
            MouseEventType.Press => new MouseClickMsg(button, x, y, modifiers),
            MouseEventType.Release => new MouseReleaseMsg(button, x, y, modifiers),
            MouseEventType.Motion => new MouseMotionMsg(button, x, y, modifiers),
            MouseEventType.Wheel => new MouseWheelMsg(button, x, y, modifiers),
            _ => new MouseMotionMsg(button, x, y, modifiers),
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
        var parts = text.Split([';', ':']);
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

            var end = start;
            while (end < segment.Length && char.IsDigit(segment[end]))
            {
                end++;
            }

            if (int.TryParse(segment[start..end], out var value))
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
