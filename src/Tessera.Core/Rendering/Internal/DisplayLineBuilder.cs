using System.Globalization;

namespace Tessera.Core.Rendering.Internal;

internal static class DisplayLineBuilder
{
    public static IReadOnlyList<DisplayLine> WrapText(string text, int maxColumns)
    {
        if (maxColumns <= 0)
        {
            return [FromText(text, maxColumns)];
        }

        if (string.IsNullOrEmpty(text))
        {
            return [new DisplayLine([], [])];
        }

        var lines = new List<DisplayLine>();
        var current = new List<string?>(Math.Min(text.Length, maxColumns));
        var currentStyles = new List<string?>(Math.Min(text.Length, maxColumns));
        var sgrState = SgrStyleState.Default;
        var activeStyle = string.Empty;
        var index = 0;

        while (index < text.Length)
        {
            if (SgrParser.TryRead(text, ref index, ref sgrState, out var updatedStyle))
            {
                activeStyle = updatedStyle;
                continue;
            }

            var element = StringInfo.GetNextTextElement(text, index);
            index += element.Length;
            var width = DisplayWidth.MeasureTextElementWidth(element);
            if (width <= 0)
            {
                AttachZeroWidth(current, element);
                continue;
            }

            if (!CanFit(current.Count, width, maxColumns))
            {
                if (current.Count > 0)
                {
                    lines.Add(new DisplayLine([.. current], [.. currentStyles]));
                    current.Clear();
                    currentStyles.Clear();
                }

                if (width > maxColumns)
                {
                    continue;
                }
            }

            AddCell(current, currentStyles, element, activeStyle, width);
        }

        if (current.Count > 0)
        {
            lines.Add(new DisplayLine([.. current], [.. currentStyles]));
        }

        if (lines.Count == 0)
        {
            lines.Add(new DisplayLine([], []));
        }

        return lines;
    }

    public static DisplayLine FromText(string text, int maxColumns)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new DisplayLine([], []);
        }

        var cells = new List<string?>(text.Length);
        var styles = new List<string?>(text.Length);
        var sgrState = SgrStyleState.Default;
        var activeStyle = string.Empty;
        var index = 0;

        while (index < text.Length)
        {
            if (SgrParser.TryRead(text, ref index, ref sgrState, out var updatedStyle))
            {
                activeStyle = updatedStyle;
                continue;
            }

            var element = StringInfo.GetNextTextElement(text, index);
            index += element.Length;
            var width = DisplayWidth.MeasureTextElementWidth(element);
            if (width <= 0)
            {
                var attachIndex = FindPreviousBaseCell(cells);
                if (attachIndex >= 0)
                {
                    cells[attachIndex] += element;
                }
                else if (CanFit(cells.Count, 1, maxColumns))
                {
                    cells.Add(element);
                    styles.Add(activeStyle);
                }

                continue;
            }

            if (!CanFit(cells.Count, width, maxColumns))
            {
                break;
            }

            AddCell(cells, styles, element, activeStyle, width);
        }

        return new DisplayLine([.. cells], [.. styles]);
    }

    private static void AddCell(List<string?> cells, List<string?> styles, string element, string activeStyle, int width)
    {
        cells.Add(element);
        styles.Add(activeStyle);
        if (width == 2)
        {
            cells.Add(null);
            styles.Add(null);
        }
    }

    private static void AttachZeroWidth(List<string?> cells, string element)
    {
        var attachIndex = FindPreviousBaseCell(cells);
        if (attachIndex >= 0)
        {
            cells[attachIndex] += element;
        }
    }

    private static bool CanFit(int currentColumns, int incomingWidth, int maxColumns)
    {
        return maxColumns <= 0 || currentColumns + incomingWidth <= maxColumns;
    }

    private static int FindPreviousBaseCell(List<string?> cells)
    {
        for (var i = cells.Count - 1; i >= 0; i--)
        {
            if (cells[i] is not null)
            {
                return i;
            }
        }

        return -1;
    }
}
