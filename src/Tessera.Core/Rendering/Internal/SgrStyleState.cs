using System.Globalization;

namespace Tessera.Core.Rendering;

internal struct SgrStyleState
{
    public static SgrStyleState Default => new();

    private bool _bold;
    private bool _dim;
    private bool _italic;
    private bool _underline;
    private bool _doubleUnderline;
    private bool _blink;
    private bool _strikethrough;
    private bool _conceal;
    private bool _overline;
    private bool _framed;
    private bool _encircled;
    private bool _inverse;
    private string? _foreground;
    private string? _background;

    public void Apply(ReadOnlySpan<int> codes)
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
                case 0: Reset(); break;
                case 1: _bold = true; break;
                case 2: _dim = true; break;
                case 3: _italic = true; break;
                case 4: _underline = true; _doubleUnderline = false; break;
                case 21: _doubleUnderline = true; _underline = false; break;
                case 5 or 6: _blink = true; break;
                case 9: _strikethrough = true; break;
                case 8: _conceal = true; break;
                case 53: _overline = true; break;
                case 51: _framed = true; _encircled = false; break;
                case 52: _encircled = true; _framed = false; break;
                case 7: _inverse = true; break;
                case 22: _bold = false; _dim = false; break;
                case 23: _italic = false; break;
                case 24: _underline = false; _doubleUnderline = false; break;
                case 25: _blink = false; break;
                case 29: _strikethrough = false; break;
                case 28: _conceal = false; break;
                case 55: _overline = false; break;
                case 54: _framed = false; _encircled = false; break;
                case 27: _inverse = false; break;
                case 39: _foreground = null; break;
                case 49: _background = null; break;
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
        if (_bold) parts.Add("1");
        if (_dim) parts.Add("2");
        if (_italic) parts.Add("3");
        if (_doubleUnderline) parts.Add("21");
        else if (_underline) parts.Add("4");
        if (_blink) parts.Add("5");
        if (_strikethrough) parts.Add("9");
        if (_conceal) parts.Add("8");
        if (_overline) parts.Add("53");
        if (_encircled) parts.Add("52");
        else if (_framed) parts.Add("51");
        if (_inverse) parts.Add("7");
        if (!string.IsNullOrEmpty(_foreground)) parts.Add(_foreground);
        if (!string.IsNullOrEmpty(_background)) parts.Add(_background);
        return parts.Count == 0 ? string.Empty : $"\u001b[{string.Join(";", parts)}m";
    }

    private void Reset()
    {
        _bold = _dim = _italic = _underline = _doubleUnderline = _blink = _strikethrough = _conceal = _overline = _framed = _encircled = _inverse = false;
        _foreground = null;
        _background = null;
    }

    private static string? ParseExtendedColorParameter(ReadOnlySpan<int> codes, ref int index, bool foreground)
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
