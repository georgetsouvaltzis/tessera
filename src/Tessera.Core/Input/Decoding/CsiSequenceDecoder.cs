using Tessera.Core.Abstractions;
using Tessera.Core.Messages;

namespace Tessera.Core.Input.Decoding;

internal static class CsiSequenceDecoder
{
    public static bool TryDecodeMessage(char final, IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;

        if (MouseSequenceDecoder.TryDecodeSgrMouse(final, parameters, out message))
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

        if (DecoderCommon.IsCsiCursorFinal(final) && TryDecodeCsiCursorKey(final, parameters, out message))
        {
            return true;
        }

        if (final == '~' && TryDecodeCsiTildeKey(parameters, out message))
        {
            return true;
        }

        return false;
    }

    public static bool TryDecodeSs3Key(char final, KeyModifiers modifiers, out KeyCode keyCode)
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
            _ => KeyCode.Unknown
        };

        return keyCode != KeyCode.Unknown;
    }

    private static bool TryDecodeBracketedPaste(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if (parameters.Count == 0 || parameters[0] is not { } code)
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
        if (parameters.Count < 3 || parameters[0] != 8 || parameters[1] is not { } rows ||
            parameters[2] is not { } cols)
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
            || parameters[0] is not { } mode
            || parameters[1] is not { } stateRaw)
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
            _ => ModeReportState.Unknown
        };

        message = new ModeReportMsg(mode, state);
        return true;
    }

    private static bool TryDecodeFocus(char final, out IMessage? message)
    {
        message = final switch
        {
            'I' => new FocusInMsg(),
            'O' => new FocusOutMsg(),
            _ => null
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
            _ => KeyCode.Unknown
        };

        if (keyCode == KeyCode.Unknown)
        {
            return false;
        }

        var modifier = DecoderCommon.GetCsiModifierParameter(parameters, final);
        var modifiers = DecoderCommon.ParseModifiers(modifier);
        message = new KeyPressMsg(keyCode, string.Empty, modifiers);
        return true;
    }

    private static bool TryDecodeCsiTildeKey(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if (parameters.Count == 0 || parameters[0] is not { } code)
        {
            return false;
        }

        if (code == 27 && parameters.Count >= 3 && parameters[2] is { } modifyOtherKeyCodePoint)
        {
            return DecoderCommon.TryCreateKeyMessageFromCodePoint(
                modifyOtherKeyCodePoint,
                DecoderCommon.ParseModifiers(parameters[1]),
                null,
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
            _ => KeyCode.Unknown
        };

        if (keyCode == KeyCode.Unknown)
        {
            return false;
        }

        var modifiers = parameters.Count > 1
            ? DecoderCommon.ParseModifiers(parameters[1])
            : KeyModifiers.None;

        message = new KeyPressMsg(keyCode, string.Empty, modifiers);
        return true;
    }

    private static bool TryDecodeCsiUnicodeKey(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        message = null;
        if (parameters.Count == 0 || parameters[0] is not { } codePoint)
        {
            return false;
        }

        var modifiers = parameters.Count > 1
            ? DecoderCommon.ParseModifiers(parameters[1])
            : KeyModifiers.None;
        var eventType = parameters.Count > 2 ? parameters[2] : null;

        return DecoderCommon.TryCreateKeyMessageFromCodePoint(codePoint, modifiers, eventType, out message);
    }

    private static bool TryDecodeCsiBackTab(IReadOnlyList<int?> parameters, out IMessage? message)
    {
        var modifier = DecoderCommon.GetCsiModifierParameter(parameters, 'Z');
        var modifiers = modifier is null
            ? KeyModifiers.Shift
            : DecoderCommon.ParseModifiers(modifier) | KeyModifiers.Shift;

        message = new KeyPressMsg(KeyCode.Tab, string.Empty, modifiers);
        return true;
    }
}
