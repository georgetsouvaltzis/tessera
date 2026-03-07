using System.Buffers;
using System.Globalization;
using System.Text;

namespace TeaSharp.Core.Rendering;

internal sealed class DisplayLine
{
    private const string ContinuationMarker = "\u0000";
    private readonly string?[] _cells;
    private readonly string?[] _styles;

    public DisplayLine(string?[] cells, string?[] styles)
    {
        _cells = cells;
        _styles = styles;
    }

    public int ColumnCount => _cells.Length;

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
            if (TryReadSgr(text, ref index, ref sgrState, out var updatedStyle))
            {
                activeStyle = updatedStyle;
                continue;
            }

            var element = StringInfo.GetNextTextElement(text, index);
            index += element.Length;
            var width = DisplayWidth.MeasureTextElementWidth(element);
            if (width <= 0)
            {
                var attachIndex = FindPreviousBaseCell(current);
                if (attachIndex >= 0)
                {
                    current[attachIndex] += element;
                }

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

            current.Add(element);
            currentStyles.Add(activeStyle);
            if (width == 2)
            {
                current.Add(null);
                currentStyles.Add(null);
            }
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
            if (TryReadSgr(text, ref index, ref sgrState, out var updatedStyle))
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

            cells.Add(element);
            styles.Add(activeStyle);
            if (width == 2)
            {
                cells.Add(null);
                styles.Add(null);
            }
        }

        return new DisplayLine([.. cells], [.. styles]);
    }

    public string SignatureAt(int column)
    {
        if (column < 0 || column >= _cells.Length)
        {
            return string.Empty;
        }

        if (_cells[column] is null)
        {
            return ContinuationMarker;
        }

        var style = _styles[column];
        return string.IsNullOrEmpty(style)
            ? _cells[column]!
            : $"{style}\u001f{_cells[column]}";
    }

    public string? CellAt(int column)
    {
        if (column < 0 || column >= _cells.Length)
        {
            return null;
        }

        return _cells[column];
    }

    public int CellWidthAt(int column)
    {
        if (column < 0 || column >= _cells.Length)
        {
            return 1;
        }

        if (_cells[column] is null)
        {
            return 1;
        }

        return column + 1 < _cells.Length && _cells[column + 1] is null
            ? 2
            : 1;
    }

    public string StyleAt(int column)
    {
        if (column < 0 || column >= _styles.Length || _cells[column] is null)
        {
            return string.Empty;
        }

        return _styles[column] ?? string.Empty;
    }

    private static bool CanFit(int currentColumns, int incomingWidth, int maxColumns)
    {
        if (maxColumns <= 0)
        {
            return true;
        }

        return currentColumns + incomingWidth <= maxColumns;
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

    private static bool TryReadSgr(
        string text,
        ref int index,
        ref SgrStyleState state,
        out string currentStyle)
    {
        currentStyle = state.ToEscapeSequence();

        if (text[index] != '\u001b')
        {
            return false;
        }

        if (index + 2 >= text.Length || text[index + 1] != '[')
        {
            return false;
        }

        var cursor = index + 2;
        while (cursor < text.Length && text[cursor] != 'm')
        {
            var ch = text[cursor];
            if (!char.IsDigit(ch) && ch != ';' && ch != ':')
            {
                return false;
            }

            cursor++;
        }

        if (cursor >= text.Length || text[cursor] != 'm')
        {
            return false;
        }

        var parameters = ParseSgrParameters(text.AsSpan(index + 2, cursor - (index + 2)));
        state.Apply(parameters);
        currentStyle = state.ToEscapeSequence();
        index = cursor + 1;
        return true;
    }

    private static int[] ParseSgrParameters(ReadOnlySpan<char> parameters)
    {
        if (parameters.Length == 0)
        {
            return [0];
        }

        var values = new List<int>(8);
        var value = 0;
        var hasValue = false;

        foreach (var ch in parameters)
        {
            if (char.IsDigit(ch))
            {
                value = (value * 10) + (ch - '0');
                hasValue = true;
                continue;
            }

            if (ch is ';' or ':')
            {
                values.Add(hasValue ? value : 0);
                value = 0;
                hasValue = false;
            }
        }

        values.Add(hasValue ? value : 0);
        return [.. values];
    }

    private struct SgrStyleState
    {
        public static SgrStyleState Default => new();

        private bool _bold;
        private bool _dim;
        private bool _italic;
        private bool _underline;
        private bool _blink;
        private bool _strikethrough;
        private bool _conceal;
        private bool _overline;
        private bool _inverse;
        private string? _foreground;
        private string? _background;

        public void Apply(int[] codes)
        {
            if (codes.Length == 0)
            {
                Reset();
                return;
            }

            for (var i = 0; i < codes.Length; i++)
            {
                var code = codes[i];
                switch (code)
                {
                    case 0:
                        Reset();
                        break;
                    case 1:
                        _bold = true;
                        break;
                    case 2:
                        _dim = true;
                        break;
                    case 3:
                        _italic = true;
                        break;
                    case 4:
                        _underline = true;
                        break;
                    case 5:
                    case 6:
                        _blink = true;
                        break;
                    case 9:
                        _strikethrough = true;
                        break;
                    case 8:
                        _conceal = true;
                        break;
                    case 53:
                        _overline = true;
                        break;
                    case 7:
                        _inverse = true;
                        break;
                    case 22:
                        _bold = false;
                        _dim = false;
                        break;
                    case 23:
                        _italic = false;
                        break;
                    case 24:
                        _underline = false;
                        break;
                    case 25:
                        _blink = false;
                        break;
                    case 29:
                        _strikethrough = false;
                        break;
                    case 28:
                        _conceal = false;
                        break;
                    case 55:
                        _overline = false;
                        break;
                    case 27:
                        _inverse = false;
                        break;
                    case 39:
                        _foreground = null;
                        break;
                    case 49:
                        _background = null;
                        break;
                    default:
                        if (code is >= 30 and <= 37 or >= 90 and <= 97)
                        {
                            _foreground = code.ToString(CultureInfo.InvariantCulture);
                        }
                        else if (code is >= 40 and <= 47 or >= 100 and <= 107)
                        {
                            _background = code.ToString(CultureInfo.InvariantCulture);
                        }
                        else if (code == 38)
                        {
                            _foreground = ParseExtendedColorParameter(codes, ref i, foreground: true);
                        }
                        else if (code == 48)
                        {
                            _background = ParseExtendedColorParameter(codes, ref i, foreground: false);
                        }

                        break;
                }
            }
        }

        public string ToEscapeSequence()
        {
            var parts = new List<string>(8);

            if (_bold)
            {
                parts.Add("1");
            }

            if (_dim)
            {
                parts.Add("2");
            }

            if (_italic)
            {
                parts.Add("3");
            }

            if (_underline)
            {
                parts.Add("4");
            }

            if (_blink)
            {
                parts.Add("5");
            }

            if (_strikethrough)
            {
                parts.Add("9");
            }

            if (_conceal)
            {
                parts.Add("8");
            }

            if (_overline)
            {
                parts.Add("53");
            }

            if (_inverse)
            {
                parts.Add("7");
            }

            if (!string.IsNullOrEmpty(_foreground))
            {
                parts.Add(_foreground);
            }

            if (!string.IsNullOrEmpty(_background))
            {
                parts.Add(_background);
            }

            if (parts.Count == 0)
            {
                return string.Empty;
            }

            return $"\u001b[{string.Join(";", parts)}m";
        }

        private void Reset()
        {
            _bold = false;
            _dim = false;
            _italic = false;
            _underline = false;
            _blink = false;
            _strikethrough = false;
            _conceal = false;
            _overline = false;
            _inverse = false;
            _foreground = null;
            _background = null;
        }

        private static string? ParseExtendedColorParameter(int[] codes, ref int index, bool foreground)
        {
            if (index + 1 >= codes.Length)
            {
                return null;
            }

            var prefix = foreground ? 38 : 48;
            var mode = codes[index + 1];

            if (mode == 5 && index + 2 < codes.Length)
            {
                var colorIndex = Math.Clamp(codes[index + 2], 0, 255);
                index += 2;
                return $"{prefix};5;{colorIndex}";
            }

            if (mode == 2 && index + 4 < codes.Length)
            {
                var red = Math.Clamp(codes[index + 2], 0, 255);
                var green = Math.Clamp(codes[index + 3], 0, 255);
                var blue = Math.Clamp(codes[index + 4], 0, 255);
                index += 4;
                return $"{prefix};2;{red};{green};{blue}";
            }

            return null;
        }
    }
}

internal static class DisplayWidth
{
    private static readonly (int Start, int End)[] WideRanges =
    [
        (0x1100, 0x115F),
        (0x231A, 0x231B),
        (0x2329, 0x232A),
        (0x23E9, 0x23EC),
        (0x23F0, 0x23F0),
        (0x23F3, 0x23F3),
        (0x25FD, 0x25FE),
        (0x2614, 0x2615),
        (0x2648, 0x2653),
        (0x267F, 0x267F),
        (0x2693, 0x2693),
        (0x26A1, 0x26A1),
        (0x26AA, 0x26AB),
        (0x26BD, 0x26BE),
        (0x26C4, 0x26C5),
        (0x26CE, 0x26CE),
        (0x26D4, 0x26D4),
        (0x26EA, 0x26EA),
        (0x26F2, 0x26F3),
        (0x26F5, 0x26F5),
        (0x26FA, 0x26FA),
        (0x26FD, 0x26FD),
        (0x2705, 0x2705),
        (0x270A, 0x270B),
        (0x2728, 0x2728),
        (0x274C, 0x274C),
        (0x274E, 0x274E),
        (0x2753, 0x2755),
        (0x2757, 0x2757),
        (0x2795, 0x2797),
        (0x27B0, 0x27B0),
        (0x27BF, 0x27BF),
        (0x2B1B, 0x2B1C),
        (0x2B50, 0x2B50),
        (0x2B55, 0x2B55),
        (0x2E80, 0x303E),
        (0x3040, 0xA4CF),
        (0xAC00, 0xD7A3),
        (0xF900, 0xFAFF),
        (0xFE10, 0xFE19),
        (0xFE30, 0xFE6F),
        (0xFF00, 0xFF60),
        (0xFFE0, 0xFFE6),
        (0x1F300, 0x1FAFF),
        (0x20000, 0x2FFFD),
        (0x30000, 0x3FFFD),
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

        if (rune.Value == 0)
        {
            return 0;
        }

        if (Rune.IsControl(rune))
        {
            return 0;
        }

        var category = Rune.GetUnicodeCategory(rune);
        if (category is UnicodeCategory.NonSpacingMark
            or UnicodeCategory.EnclosingMark
            or UnicodeCategory.Format)
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
