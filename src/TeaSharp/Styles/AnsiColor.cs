namespace TeaSharp.Styles;

public readonly record struct AnsiColor
{
    private AnsiColor(AnsiColorMode mode, byte red, byte green, byte blue, int index)
    {
        Mode = mode;
        Red = red;
        Green = green;
        Blue = blue;
        Index = index;
    }

    public AnsiColorMode Mode { get; }

    public byte Red { get; }

    public byte Green { get; }

    public byte Blue { get; }

    public int Index { get; }

    public static AnsiColor Default => new(AnsiColorMode.Default, 0, 0, 0, 0);

    public static AnsiColor Indexed(int index)
    {
        if (index is < 0 or > 255)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "ANSI indexed color must be in range 0..255.");
        }

        return new AnsiColor(AnsiColorMode.Indexed, 0, 0, 0, index);
    }

    public static AnsiColor Rgb(byte red, byte green, byte blue)
    {
        return new AnsiColor(AnsiColorMode.Rgb, red, green, blue, 0);
    }

    public static AnsiColor Black => Indexed(0);
    public static AnsiColor RedColor => Indexed(1);
    public static AnsiColor GreenColor => Indexed(2);
    public static AnsiColor Yellow => Indexed(3);
    public static AnsiColor BlueColor => Indexed(4);
    public static AnsiColor Magenta => Indexed(5);
    public static AnsiColor Cyan => Indexed(6);
    public static AnsiColor White => Indexed(7);
    public static AnsiColor BrightBlack => Indexed(8);
    public static AnsiColor BrightRed => Indexed(9);
    public static AnsiColor BrightGreen => Indexed(10);
    public static AnsiColor BrightYellow => Indexed(11);
    public static AnsiColor BrightBlue => Indexed(12);
    public static AnsiColor BrightMagenta => Indexed(13);
    public static AnsiColor BrightCyan => Indexed(14);
    public static AnsiColor BrightWhite => Indexed(15);

    internal string ToForegroundParameter()
    {
        return Mode switch
        {
            AnsiColorMode.Default => "39",
            AnsiColorMode.Indexed => $"38;5;{Index}",
            AnsiColorMode.Rgb => $"38;2;{Red};{Green};{Blue}",
            _ => "39",
        };
    }

    internal string ToBackgroundParameter()
    {
        return Mode switch
        {
            AnsiColorMode.Default => "49",
            AnsiColorMode.Indexed => $"48;5;{Index}",
            AnsiColorMode.Rgb => $"48;2;{Red};{Green};{Blue}",
            _ => "49",
        };
    }
}
