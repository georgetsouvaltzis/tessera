using TeaSharp.Styles;

namespace TeaSharp.Examples.MusicDeck;

internal static class MusicDeckTheme
{
    public static TeaTheme DefaultTheme { get; } = new()
    {
        Text = new TeaThemeTextTokens
        {
            Primary = Foreground(0xF6EDE3),
            Secondary = Foreground(0xD8BFA8),
            Muted = Foreground(0x9A7A69),
            Inverse = ForegroundBackground(0x1B1010, 0xF6EDE3),
        },
        Surface = new TeaThemeSurfaceTokens
        {
            Base = Background(0x130C12),
            Panel = Background(0x1B1018),
            Overlay = Background(0x2A1820),
        },
        Border = new TeaThemeBorderTokens
        {
            Default = Foreground(0x5B3848),
            Strong = Foreground(0x8B576C),
            Focused = Foreground(0xF3C77A),
            Error = Foreground(0xF28D74),
        },
        State = new TeaThemeStateTokens
        {
            Success = Foreground(0x9FD9A3),
            Warning = Foreground(0xF3C77A),
            Error = Foreground(0xF28D74),
            Info = Foreground(0xD9A7FF),
        },
        Accent = new TeaThemeAccentTokens
        {
            Primary = Foreground(0xF1B577),
            Secondary = Foreground(0xE79BA8),
        },
        Selection = new TeaThemeSelectionTokens
        {
            Foreground = Foreground(0x1B1010),
            Background = Background(0xF1B577),
        },
        Focus = new TeaThemeFocusTokens
        {
            Ring = Foreground(0xF3C77A).WithBold(),
            Title = Foreground(0xF3C77A).WithBold(),
            Border = Foreground(0xF3C77A).WithBold(),
            Marker = "✦",
        },
    };

    public static TeaStyle Foreground(int color)
    {
        var (red, green, blue) = Split(color);
        return TeaStyle.Empty.WithForeground(AnsiColor.Rgb(red, green, blue));
    }

    public static TeaStyle Background(int color)
    {
        var (red, green, blue) = Split(color);
        return TeaStyle.Empty.WithBackground(AnsiColor.Rgb(red, green, blue));
    }

    public static TeaStyle ForegroundBackground(int foreground, int background)
    {
        var (fgRed, fgGreen, fgBlue) = Split(foreground);
        var (bgRed, bgGreen, bgBlue) = Split(background);
        return TeaStyle.Empty
            .WithForeground(AnsiColor.Rgb(fgRed, fgGreen, fgBlue))
            .WithBackground(AnsiColor.Rgb(bgRed, bgGreen, bgBlue));
    }

    public static TeaStyle Chip(int foreground, int background, bool bold = true)
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
