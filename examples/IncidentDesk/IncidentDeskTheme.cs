using TeaSharp.Styles;

namespace TeaSharp.Examples.IncidentDesk;

internal static class IncidentDeskTheme
{
    public static TeaTheme DefaultTheme { get; } = new()
    {
        Text = new TeaThemeTextTokens
        {
            Primary = Foreground(0xF7EBDD),
            Secondary = Foreground(0xD6BFAF),
            Muted = Foreground(0x8E7A74),
            Inverse = ForegroundBackground(0x130F14, 0xF7EBDD),
        },
        Surface = new TeaThemeSurfaceTokens
        {
            Base = Background(0x130F14),
            Panel = Background(0x1B141A),
            Overlay = Background(0x241A22),
        },
        Border = new TeaThemeBorderTokens
        {
            Default = Foreground(0x4B3338),
            Strong = Foreground(0x8B5A46),
            Focused = Foreground(0xF3B276),
            Error = Foreground(0xFF8672),
        },
        State = new TeaThemeStateTokens
        {
            Success = Foreground(0x8FDBA9),
            Warning = Foreground(0xF7C36E),
            Error = Foreground(0xFF8672),
            Info = Foreground(0x8AC8E6),
        },
        Accent = new TeaThemeAccentTokens
        {
            Primary = Foreground(0xF3B276),
            Secondary = Foreground(0xE58E73),
        },
        Selection = new TeaThemeSelectionTokens
        {
            Foreground = Foreground(0xFFF4E8),
            Background = Background(0x6A2F22),
        },
        Focus = new TeaThemeFocusTokens
        {
            Ring = Foreground(0xF3B276).WithBold(),
            Title = Foreground(0xF3B276).WithBold(),
            Border = Foreground(0xF3B276).WithBold(),
            Marker = "◈",
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
