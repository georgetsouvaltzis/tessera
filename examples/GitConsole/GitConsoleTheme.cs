using Tessera.Styles;

namespace Tessera.Examples.GitConsole;

internal static class GitConsoleTheme
{
    public static TesseraTheme DefaultTheme { get; } = new()
    {
        Text =
            new TesseraThemeTextTokens
            {
                Primary = Foreground(0xE6EEF7),
                Secondary = Foreground(0x9DB1C7),
                Muted = Foreground(0x6C8097),
                Inverse = ForegroundBackground(0x0E1620, 0xE6EEF7)
            },
        Surface =
            new TesseraThemeSurfaceTokens
            {
                Base = Background(0x091018),
                Panel = Background(0x0E1620),
                Overlay = Background(0x13202D)
            },
        Border =
            new TesseraThemeBorderTokens
            {
                Default = Foreground(0x2A425B),
                Strong = Foreground(0x4B6D8F),
                Focused = Foreground(0x86D1FF),
                Error = Foreground(0xFF7D81)
            },
        State =
            new TesseraThemeStateTokens
            {
                Success = Foreground(0x61E294),
                Warning = Foreground(0xF2C572),
                Error = Foreground(0xFF7D81),
                Info = Foreground(0x67C6FF)
            },
        Accent = new TesseraThemeAccentTokens { Primary = Foreground(0x7AE2CF), Secondary = Foreground(0x92B4FF) },
        Selection =
            new TesseraThemeSelectionTokens { Foreground = Foreground(0xF4FAFF), Background = Background(0x1F5A7A) },
        Focus = new TesseraThemeFocusTokens
        {
            Ring = Foreground(0x86D1FF).WithBold(),
            Title = Foreground(0x86D1FF).WithBold(),
            Border = Foreground(0x86D1FF).WithBold(),
            Marker = "◆"
        }
    };

    public static TesseraStyle Foreground(int color)
    {
        var (red, green, blue) = Split(color);
        return TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(red, green, blue));
    }

    public static TesseraStyle Background(int color)
    {
        var (red, green, blue) = Split(color);
        return TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(red, green, blue));
    }

    public static TesseraStyle ForegroundBackground(int foreground, int background)
    {
        var (fgRed, fgGreen, fgBlue) = Split(foreground);
        var (bgRed, bgGreen, bgBlue) = Split(background);
        return TesseraStyle.Empty
            .WithForeground(AnsiColor.Rgb(fgRed, fgGreen, fgBlue))
            .WithBackground(AnsiColor.Rgb(bgRed, bgGreen, bgBlue));
    }

    public static TesseraStyle Chip(int foreground, int background, bool bold = false)
    {
        var style = ForegroundBackground(foreground, background);
        return bold ? style.WithBold() : style;
    }

    private static (byte Red, byte Green, byte Blue) Split(int color)
    {
        var red = (byte)((color >> 16) & 0xFF);
        var green = (byte)((color >> 8) & 0xFF);
        var blue = (byte)(color & 0xFF);
        return (red, green, blue);
    }
}
