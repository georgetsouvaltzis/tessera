namespace Tessera.Styles;

/// <summary>
/// Represents an ANSI terminal color in default, indexed, or RGB form.
/// </summary>
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

    /// <summary>
    /// Gets the color encoding mode.
    /// </summary>
    public AnsiColorMode Mode { get; }

    /// <summary>
    /// Gets the red component when <see cref="Mode"/> is <see cref="AnsiColorMode.Rgb"/>.
    /// </summary>
    public byte Red { get; }

    /// <summary>
    /// Gets the green component when <see cref="Mode"/> is <see cref="AnsiColorMode.Rgb"/>.
    /// </summary>
    public byte Green { get; }

    /// <summary>
    /// Gets the blue component when <see cref="Mode"/> is <see cref="AnsiColorMode.Rgb"/>.
    /// </summary>
    public byte Blue { get; }

    /// <summary>
    /// Gets the ANSI palette index when <see cref="Mode"/> is <see cref="AnsiColorMode.Indexed"/>.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the terminal default color.
    /// </summary>
    public static AnsiColor Default => new(AnsiColorMode.Default, 0, 0, 0, 0);

    /// <summary>
    /// Creates an indexed ANSI color.
    /// </summary>
    /// <param name="index">The index value.</param>
    /// <returns>The indexed ANSI color.</returns>
    public static AnsiColor Indexed(int index)
    {
        if (index is < 0 or > 255)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "ANSI indexed color must be in range 0..255.");
        }

        return new AnsiColor(AnsiColorMode.Indexed, 0, 0, 0, index);
    }

    /// <summary>
    /// Creates a 24-bit RGB color.
    /// </summary>
    /// <param name="red">The red value.</param>
    /// <param name="green">The green value.</param>
    /// <param name="blue">The blue value.</param>
    /// <returns>The RGB color.</returns>
    public static AnsiColor Rgb(byte red, byte green, byte blue)
    {
        return new AnsiColor(AnsiColorMode.Rgb, red, green, blue, 0);
    }

    /// <summary>
    /// Gets the indexed black color.
    /// </summary>
    public static AnsiColor Black => Indexed(0);

    /// <summary>
    /// Gets the indexed red color.
    /// </summary>
    public static AnsiColor RedColor => Indexed(1);

    /// <summary>
    /// Gets the indexed green color.
    /// </summary>
    public static AnsiColor GreenColor => Indexed(2);

    /// <summary>
    /// Gets the indexed yellow color.
    /// </summary>
    public static AnsiColor Yellow => Indexed(3);

    /// <summary>
    /// Gets the indexed blue color.
    /// </summary>
    public static AnsiColor BlueColor => Indexed(4);

    /// <summary>
    /// Gets the indexed magenta color.
    /// </summary>
    public static AnsiColor Magenta => Indexed(5);

    /// <summary>
    /// Gets the indexed cyan color.
    /// </summary>
    public static AnsiColor Cyan => Indexed(6);

    /// <summary>
    /// Gets the indexed white color.
    /// </summary>
    public static AnsiColor White => Indexed(7);

    /// <summary>
    /// Gets the indexed bright black color.
    /// </summary>
    public static AnsiColor BrightBlack => Indexed(8);

    /// <summary>
    /// Gets the indexed bright red color.
    /// </summary>
    public static AnsiColor BrightRed => Indexed(9);

    /// <summary>
    /// Gets the indexed bright green color.
    /// </summary>
    public static AnsiColor BrightGreen => Indexed(10);

    /// <summary>
    /// Gets the indexed bright yellow color.
    /// </summary>
    public static AnsiColor BrightYellow => Indexed(11);

    /// <summary>
    /// Gets the indexed bright blue color.
    /// </summary>
    public static AnsiColor BrightBlue => Indexed(12);

    /// <summary>
    /// Gets the indexed bright magenta color.
    /// </summary>
    public static AnsiColor BrightMagenta => Indexed(13);

    /// <summary>
    /// Gets the indexed bright cyan color.
    /// </summary>
    public static AnsiColor BrightCyan => Indexed(14);

    /// <summary>
    /// Gets the indexed bright white color.
    /// </summary>
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
