using Tessera.Core.Abstractions;
using Tessera.Core.Messages;

namespace Tessera.Core.Input;

internal static class MouseSequenceDecoder
{
    private const int MouseModifierShiftMask = 0b0000_0100;
    private const int MouseModifierAltMask = 0b0000_1000;
    private const int MouseModifierCtrlMask = 0b0001_0000;
    private const int MouseMotionMask = 0b0010_0000;
    private const int MouseWheelMask = 0b0100_0000;
    private const int MouseExtendedButtonsMask = 0b1000_0000;

    public static DecodeResult DecodeX10Mouse(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (buffer.Length < 6)
        {
            _ = timeoutExpired;
            // Keep partial X10 reports buffered so split mouse packets cannot degrade
            // into Escape/character fragments on timeout boundaries.
            return new DecodeResult(0, null, true);
        }

        var cb = buffer[3] - 32;
        var cx = buffer[4] - 32;
        var cy = buffer[5] - 32;
        if (cb < 0 || cx <= 0 || cy <= 0)
        {
            return new DecodeResult(6, new UnknownInputMsg(DecoderCommon.ToAscii(buffer[..6])), false);
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

        return new DecodeResult(6, CreateMouseMessage(eventType, button, cx - 1, cy - 1, modifiers), false);
    }

    public static bool TryDecodeSgrMouse(char final, IReadOnlyList<int?> parameters, out IMessage? message)
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

        message = CreateMouseMessage(eventType, button, cx - 1, cy - 1, modifiers);
        return true;
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
}
