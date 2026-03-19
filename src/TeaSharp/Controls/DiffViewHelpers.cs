using TeaSharp.Controls.Internal;

namespace TeaSharp.Controls;

internal static class DiffViewHelpers
{
    public static void BuildEntries(string[] oldLines, string[] newLines, List<DiffLineEntry> entries)
    {
        entries.Clear();

        var n = oldLines.Length;
        var m = newLines.Length;
        var dp = new int[n + 1, m + 1];
        for (var i = n - 1; i >= 0; i--)
        {
            for (var j = m - 1; j >= 0; j--)
            {
                dp[i, j] = string.Equals(oldLines[i], newLines[j], StringComparison.Ordinal)
                    ? dp[i + 1, j + 1] + 1
                    : Math.Max(dp[i + 1, j], dp[i, j + 1]);
            }
        }

        var oldLineNumber = 1;
        var newLineNumber = 1;
        var oldIndex = 0;
        var newIndex = 0;

        while (oldIndex < n && newIndex < m)
        {
            if (string.Equals(oldLines[oldIndex], newLines[newIndex], StringComparison.Ordinal))
            {
                entries.Add(new DiffLineEntry(oldLineNumber, newLineNumber, DiffLineKind.Unchanged, oldLines[oldIndex], newLines[newIndex]));
                oldIndex++;
                newIndex++;
                oldLineNumber++;
                newLineNumber++;
            }
            else if (dp[oldIndex + 1, newIndex] >= dp[oldIndex, newIndex + 1])
            {
                entries.Add(new DiffLineEntry(oldLineNumber, 0, DiffLineKind.Removed, oldLines[oldIndex], string.Empty));
                oldIndex++;
                oldLineNumber++;
            }
            else
            {
                entries.Add(new DiffLineEntry(0, newLineNumber, DiffLineKind.Added, string.Empty, newLines[newIndex]));
                newIndex++;
                newLineNumber++;
            }
        }

        while (oldIndex < n)
        {
            entries.Add(new DiffLineEntry(oldLineNumber, 0, DiffLineKind.Removed, oldLines[oldIndex], string.Empty));
            oldIndex++;
            oldLineNumber++;
        }

        while (newIndex < m)
        {
            entries.Add(new DiffLineEntry(0, newLineNumber, DiffLineKind.Added, string.Empty, newLines[newIndex]));
            newIndex++;
            newLineNumber++;
        }
    }

    public static string FormatSideBySide(DiffLineEntry entry, int width)
    {
        var leftWidth = Math.Max(1, (width - 3) / 2);
        var rightWidth = Math.Max(1, width - leftWidth - 3);
        var left = entry.OldLineNumber > 0
            ? $"{entry.OldLineNumber,4}: {entry.OldText}"
            : string.Empty;
        var right = entry.NewLineNumber > 0
            ? $"{entry.NewLineNumber,4}: {entry.NewText}"
            : string.Empty;

        left = TrimWithEllipsis(left, leftWidth).PadRight(leftWidth);
        right = TrimWithEllipsis(right, rightWidth);
        return $"{left} | {right}";
    }

    private static string TrimWithEllipsis(string value, int maxWidth)
    {
        if (maxWidth <= 0 || string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var width = ControlTextLayout.MeasureDisplayWidth(value);
        if (width <= maxWidth)
        {
            return value;
        }

        if (maxWidth == 1)
        {
            return "…";
        }

        var target = maxWidth - 1;
        var taken = 0;
        var index = 0;
        while (index < value.Length && taken < target)
        {
            var slice = value[index..(index + 1)];
            var nextWidth = ControlTextLayout.MeasureDisplayWidth(slice);
            if (taken + nextWidth > target)
            {
                break;
            }

            taken += nextWidth;
            index++;
        }

        return $"{value[..index]}…";
    }
}
