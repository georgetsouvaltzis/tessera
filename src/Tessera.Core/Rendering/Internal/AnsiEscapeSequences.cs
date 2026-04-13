using Tessera.Core.Abstractions;

namespace Tessera.Core.Rendering.Internal;

internal static class AnsiEscapeSequences
{
    public static string SequenceForMouseMode(MouseMode mode)
    {
        return mode switch
        {
            MouseMode.CellMotion => "\e[?1000h\e[?1002h\e[?1003l\e[?1006h",
            MouseMode.AllMotion => "\e[?1000h\e[?1002l\e[?1003h\e[?1006h",
            _ => "\e[?1000l\e[?1002l\e[?1003l\e[?1006l"
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
            _ => 0
        };

        return $"\e[{parameter} q";
    }

    public static string TerminalColor(int setCode, int resetCode, string? color)
    {
        return color is null
            ? $"\e]{resetCode};\e\\"
            : $"\e]{setCode};{color}\e\\";
    }

    public static string Progress(TerminalProgress? progress)
    {
        if (progress is not { } current || current.State == TerminalProgressState.None)
        {
            return "\e]9;4;0\e\\";
        }

        if (current.State == TerminalProgressState.Indeterminate)
        {
            return "\e]9;4;3\e\\";
        }

        var clamped = Math.Clamp(current.Value, 0, 100);
        var state = current.State switch
        {
            TerminalProgressState.Default => 1,
            TerminalProgressState.Error => 2,
            TerminalProgressState.Warning => 4,
            _ => 0
        };

        return state == 0
            ? "\e]9;4;0\e\\"
            : $"\e]9;4;{state};{clamped}\e\\";
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
