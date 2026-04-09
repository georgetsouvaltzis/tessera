using System.Buffers;
using System.Globalization;
using System.Text;

namespace Tessera.Core.Rendering.Internal;

internal static class DisplayWidth
{
    private static readonly (int Start, int End)[] WideRanges =
    [
        (0x1100, 0x115F), (0x231A, 0x231B), (0x2329, 0x232A), (0x23E9, 0x23EC), (0x23F0, 0x23F0), (0x23F3, 0x23F3),
        (0x25FD, 0x25FE), (0x2614, 0x2615), (0x2648, 0x2653), (0x267F, 0x267F), (0x2693, 0x2693), (0x26A1, 0x26A1),
        (0x26AA, 0x26AB), (0x26BD, 0x26BE), (0x26C4, 0x26C5), (0x26CE, 0x26CE), (0x26D4, 0x26D4), (0x26EA, 0x26EA),
        (0x26F2, 0x26F3), (0x26F5, 0x26F5), (0x26FA, 0x26FA), (0x26FD, 0x26FD), (0x2705, 0x2705), (0x270A, 0x270B),
        (0x2728, 0x2728), (0x274C, 0x274C), (0x274E, 0x274E), (0x2753, 0x2755), (0x2757, 0x2757), (0x2795, 0x2797),
        (0x27B0, 0x27B0), (0x27BF, 0x27BF), (0x2B1B, 0x2B1C), (0x2B50, 0x2B50), (0x2B55, 0x2B55), (0x2E80, 0x303E),
        (0x3040, 0xA4CF), (0xAC00, 0xD7A3), (0xF900, 0xFAFF), (0xFE10, 0xFE19), (0xFE30, 0xFE6F), (0xFF00, 0xFF60),
        (0xFFE0, 0xFFE6), (0x1F300, 0x1FAFF), (0x20000, 0x2FFFD), (0x30000, 0x3FFFD),
    ];

    public static int MeasureTextElementWidth(string textElement)
    {
        if (string.IsNullOrEmpty(textElement))
        {
            return 0;
        }

        if (Rune.DecodeFromUtf16(textElement, out var rune, out _) != OperationStatus.Done)
        {
            return 1;
        }

        if (rune.Value == 0 || Rune.IsControl(rune))
        {
            return 0;
        }

        var category = Rune.GetUnicodeCategory(rune);
        if (category is UnicodeCategory.NonSpacingMark or UnicodeCategory.EnclosingMark or UnicodeCategory.Format)
        {
            return 0;
        }

        return IsWide(rune) ? 2 : 1;
    }

    private static bool IsWide(Rune rune)
    {
        var value = rune.Value;
        foreach (var (start, end) in WideRanges)
        {
            if (value >= start && value <= end)
            {
                return true;
            }
        }

        return false;
    }
}
