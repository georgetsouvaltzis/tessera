using System.ComponentModel;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Input;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class EventDecoder : IEventDecoder
{
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
            return timeoutExpired
                ? new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false)
                : new DecodeResult(0, null, true);
        }

        return buffer[1] switch
        {
            (byte)'[' => DecodeCsi(buffer, timeoutExpired),
            (byte)'O' => DecodeSs3(buffer, timeoutExpired),
            (byte)']' => OscDcsSequenceDecoder.DecodeOsc(buffer, timeoutExpired),
            (byte)'P' => OscDcsSequenceDecoder.DecodeDcs(buffer, timeoutExpired),
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
            if (second is 0x7F or 0x08)
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

            if (DecoderCommon.TryDecodeControlByte(second, out var controlMessage) && controlMessage is KeyPressMsg controlKey)
            {
                return new DecodeResult(
                    2,
                    controlKey with { Modifiers = controlKey.Modifiers | KeyModifiers.Alt },
                    false);
            }
        }

        if (DecoderCommon.TryDecodeRune(buffer[1..], out var ch, out var runeLen, out var needMoreData))
        {
            return new DecodeResult(1 + runeLen, new KeyPressMsg(KeyCode.Character, ch, KeyModifiers.Alt), false);
        }

        if (!timeoutExpired)
        {
            return new DecodeResult(0, null, true);
        }

        return needMoreData
            ? new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false)
            : new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false);
    }

    private static DecodeResult DecodeCsi(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (buffer.Length >= 3 && buffer[2] == (byte)'M')
        {
            return MouseSequenceDecoder.DecodeX10Mouse(buffer, timeoutExpired);
        }

        if (!DecoderCommon.TryFindFinalByte(buffer, 2, out var finalIndex))
        {
            if (!timeoutExpired)
            {
                return new DecodeResult(0, null, true);
            }

            // Do not resolve a bare CSI introducer on timeout; the next chunk may continue
            // a control sequence and consuming ESC here can leak the remainder as text.
            if (buffer.Length == 2)
            {
                return new DecodeResult(0, null, true);
            }

            // Keep partial control-prefixed CSI payload buffered after timeout so split control
            // sequences (for example SGR mouse reports and mode reports) cannot degrade into
            // literal text fragments.
            if (buffer.Length > 2)
            {
                var prefix = buffer[2];
                if (prefix is (byte)'<' or (byte)'?' or (byte)'>')
                {
                    return new DecodeResult(0, null, true);
                }
            }

            return new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false);
        }

        var consumed = finalIndex + 1;
        var final = (char)buffer[finalIndex];
        if (final == 'u' && buffer.Length > 2 && buffer[2] == (byte)'?')
        {
            var flags = DecoderCommon.TryParseInteger(buffer[3..finalIndex], out var parsedFlags)
                ? parsedFlags
                : 0;
            return new DecodeResult(consumed, new KeyboardEnhancementsMsg(flags), false);
        }

        var parameters = DecoderCommon.ParseIntegerParameters(buffer[2..finalIndex]);
        if (CsiSequenceDecoder.TryDecodeMessage(final, parameters, out var message))
        {
            return new DecodeResult(consumed, message, false);
        }

        return new DecodeResult(consumed, new UnknownInputMsg(DecoderCommon.ToAscii(buffer[..consumed])), false);
    }

    private static DecodeResult DecodeSs3(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (!DecoderCommon.TryFindFinalByte(buffer, 2, out var finalIndex))
        {
            return timeoutExpired
                ? new DecodeResult(1, new KeyPressMsg(KeyCode.Escape), false)
                : new DecodeResult(0, null, true);
        }

        var consumed = finalIndex + 1;
        var final = (char)buffer[finalIndex];
        var parameters = DecoderCommon.ParseIntegerParameters(buffer[2..finalIndex]);
        var modifiers = parameters.Count == 0 ? KeyModifiers.None : DecoderCommon.ParseModifiers(parameters[^1]);

        if (CsiSequenceDecoder.TryDecodeSs3Key(final, modifiers, out var keyCode))
        {
            return new DecodeResult(consumed, new KeyPressMsg(keyCode, string.Empty, modifiers), false);
        }

        return new DecodeResult(consumed, new UnknownInputMsg(DecoderCommon.ToAscii(buffer[..consumed])), false);
    }

    private static DecodeResult DecodePlain(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        var first = buffer[0];
        if (DecoderCommon.TryDecodeControlByte(first, out var controlMessage))
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

    private static DecodeResult DecodeUtf8(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        if (DecoderCommon.TryDecodeRune(buffer, out var str, out var len, out var needMoreData))
        {
            return new DecodeResult(len, new KeyPressMsg(KeyCode.Character, str), false);
        }

        if (needMoreData && !timeoutExpired)
        {
            return new DecodeResult(0, null, true);
        }

        return new DecodeResult(1, new UnknownInputMsg($"0x{buffer[0]:X2}"), false);
    }
}
