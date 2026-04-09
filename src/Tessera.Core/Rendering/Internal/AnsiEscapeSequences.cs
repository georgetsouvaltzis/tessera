using Tessera.Core.Abstractions;

namespace Tessera.Core.Rendering.Internal;

internal static class AnsiEscapeSequences
{
    public static string SequenceForMouseMode(MouseMode mode)
    {
        return mode switch
        {
            MouseMode.CellMotion => "\u001b[?1000h\u001b[?1002h\u001b[?1003l\u001b[?1006h",
            MouseMode.AllMotion => "\u001b[?1000h\u001b[?1002l\u001b[?1003h\u001b[?1006h",
            _ => "\u001b[?1000l\u001b[?1002l\u001b[?1003l\u001b[?1006l",
        };
    }

    public static string SequenceForCursorStyle(CursorStyle style)
    {
        var parameter = style switch
        {
            CursorStyle.BlinkingBlock => 1,
            CursorStyle.SteadyBlock => 2,
            CursorStyle.BlinkingUnderline => 3,
            CursorStyle.SteadyUnderline => 4,
            CursorStyle.BlinkingBar => 5,
            CursorStyle.SteadyBar => 6,
            _ => 0,
        };

        return $"\u001b[{parameter} q";
    }

    public static string TerminalColor(int setCode, int resetCode, string? color)
    {
        return color is null
            ? $"\u001b]{resetCode};\u001b\\"
            : $"\u001b]{setCode};{color}\u001b\\";
    }

    public static string Progress(TerminalProgress? progress)
    {
        if (progress is not TerminalProgress current || current.State == TerminalProgressState.None)
        {
            return "\u001b]9;4;0\u001b\\";
        }

        if (current.State == TerminalProgressState.Indeterminate)
        {
            return "\u001b]9;4;3\u001b\\";
        }

        var clamped = Math.Clamp(current.Value, 0, 100);
        var state = current.State switch
        {
            TerminalProgressState.Default => 1,
            TerminalProgressState.Error => 2,
            TerminalProgressState.Warning => 4,
            _ => 0,
        };

        return state == 0
            ? "\u001b]9;4;0\u001b\\"
            : $"\u001b]9;4;{state};{clamped}\u001b\\";
    }

    public static int KeyboardEnhancementFlags(KeyboardEnhancementOptions options, bool includeKittyKeyboardBaseFlag)
    {
        var flags = includeKittyKeyboardBaseFlag ? 0b1 : 0;
        if (options.ReportEventTypes)
        {
            flags |= 0b10;
        }

        return flags;
    }
}
