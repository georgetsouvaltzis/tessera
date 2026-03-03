using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Input;

public readonly record struct DecodeResult(int Consumed, IMessage? Message, bool NeedMoreData);

public sealed class EventDecoder
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

        if (TryMatch(buffer, "\u001b[A", out var up))
        {
            return new DecodeResult(up, new KeyPressMsg(KeyCode.Up), false);
        }

        if (TryMatch(buffer, "\u001b[B", out var down))
        {
            return new DecodeResult(down, new KeyPressMsg(KeyCode.Down), false);
        }

        if (TryMatch(buffer, "\u001b[C", out var right))
        {
            return new DecodeResult(right, new KeyPressMsg(KeyCode.Right), false);
        }

        if (TryMatch(buffer, "\u001b[D", out var left))
        {
            return new DecodeResult(left, new KeyPressMsg(KeyCode.Left), false);
        }

        if (TryMatch(buffer, "\u001b[200~", out var pasteStart))
        {
            return new DecodeResult(pasteStart, new PasteStartMsg(), false);
        }

        if (TryMatch(buffer, "\u001b[201~", out var pasteEnd))
        {
            return new DecodeResult(pasteEnd, new PasteEndMsg(), false);
        }

        if (TryParseWindowSize(buffer, out var consumed, out var msg))
        {
            return new DecodeResult(consumed, msg, false);
        }

        if (buffer.Length >= 2)
        {
            var ch = DecodeRune(buffer[1..], out var runeLen);
            if (ch is not null)
            {
                return new DecodeResult(1 + runeLen, new KeyPressMsg(KeyCode.Character, ch, KeyModifiers.Alt), false);
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

    private static bool TryParseWindowSize(ReadOnlySpan<byte> buffer, out int consumed, out IMessage? message)
    {
        consumed = 0;
        message = null;

        // CSI 8 ; rows ; cols t
        var text = Encoding.ASCII.GetString(buffer);
        if (!text.StartsWith("\u001b[8;", StringComparison.Ordinal))
        {
            return false;
        }

        var end = text.IndexOf('t');
        if (end < 0)
        {
            return false;
        }

        var payload = text[4..end];
        var parts = payload.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var rows) || !int.TryParse(parts[1], out var cols))
        {
            return false;
        }

        consumed = end + 1;
        message = new WindowSizeMsg(cols, rows);
        return true;
    }

    private static bool TryMatch(ReadOnlySpan<byte> buffer, string token, out int consumed)
    {
        consumed = 0;
        var bytes = Encoding.ASCII.GetBytes(token);
        if (buffer.Length < bytes.Length)
        {
            return false;
        }

        if (buffer[..bytes.Length].SequenceEqual(bytes))
        {
            consumed = bytes.Length;
            return true;
        }

        return false;
    }
}
