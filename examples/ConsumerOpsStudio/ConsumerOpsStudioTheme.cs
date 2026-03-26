using TeaSharp.Styles;

internal static class ConsumerOpsStudioTheme
{
    public static TeaTheme Default { get; } = CreateDefault();

    public static TeaTheme Alert { get; } = CreateAlert();

    private static TeaTheme CreateDefault()
    {
        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = Fg(220, 226, 238),
                Secondary = Fg(176, 193, 214),
                Muted = Fg(133, 148, 171),
                Inverse = FgBg(12, 23, 37, 220, 226, 238),
            },
            Surface = new TeaThemeSurfaceTokens
            {
                Base = Bg(12, 23, 37),
                Panel = Bg(19, 33, 51),
                Overlay = Bg(26, 44, 65),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = Fg(73, 111, 148),
                Strong = Fg(107, 150, 194),
                Focused = Fg(88, 200, 228),
                Error = Fg(244, 114, 118),
            },
            State = new TeaThemeStateTokens
            {
                Success = Fg(94, 220, 141),
                Warning = Fg(250, 195, 90),
                Error = Fg(244, 114, 118),
                Info = Fg(88, 200, 228),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = Fg(88, 200, 228),
                Secondary = Fg(124, 145, 249),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = Fg(12, 23, 37),
                Background = Bg(88, 200, 228),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = Fg(88, 200, 228).WithBold(),
                Title = Fg(88, 200, 228).WithBold(),
                Border = Fg(88, 200, 228).WithBold(),
                Marker = "◆",
            },
        };
    }

    private static TeaTheme CreateAlert()
    {
        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = Fg(254, 230, 228),
                Secondary = Fg(252, 196, 193),
                Muted = Fg(235, 169, 165),
                Inverse = FgBg(48, 17, 18, 254, 230, 228),
            },
            Surface = new TeaThemeSurfaceTokens
            {
                Base = Bg(48, 17, 18),
                Panel = Bg(70, 24, 28),
                Overlay = Bg(96, 33, 39),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = Fg(184, 86, 92),
                Strong = Fg(221, 115, 122),
                Focused = Fg(250, 195, 90),
                Error = Fg(255, 134, 140),
            },
            State = new TeaThemeStateTokens
            {
                Success = Fg(160, 224, 180),
                Warning = Fg(250, 195, 90),
                Error = Fg(255, 134, 140),
                Info = Fg(252, 166, 122),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = Fg(252, 166, 122),
                Secondary = Fg(250, 195, 90),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = Fg(48, 17, 18),
                Background = Bg(252, 166, 122),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = Fg(250, 195, 90).WithBold(),
                Title = Fg(250, 195, 90).WithBold(),
                Border = Fg(250, 195, 90).WithBold(),
                Marker = "◆",
            },
        };
    }

    private static TeaStyle Fg(byte red, byte green, byte blue)
    {
        return TeaStyle.Empty.WithForeground(AnsiColor.Rgb(red, green, blue));
    }

    private static TeaStyle Bg(byte red, byte green, byte blue)
    {
        return TeaStyle.Empty.WithBackground(AnsiColor.Rgb(red, green, blue));
    }

    private static TeaStyle FgBg(byte foregroundRed, byte foregroundGreen, byte foregroundBlue, byte backgroundRed, byte backgroundGreen, byte backgroundBlue)
    {
        return TeaStyle.Empty
            .WithForeground(AnsiColor.Rgb(foregroundRed, foregroundGreen, foregroundBlue))
            .WithBackground(AnsiColor.Rgb(backgroundRed, backgroundGreen, backgroundBlue));
    }
}
