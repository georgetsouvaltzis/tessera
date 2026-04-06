using Tessera.Styles;

namespace Tessera.Examples.TransitBoard;

internal enum TransitBoardThemeKind
{
    Meridian,
    Harbor,
    Afterglow,
}

internal sealed record TransitBoardPalette(
    TransitBoardThemeKind Kind,
    string Label,
    TesseraTheme Theme,
    int HeroTitle,
    int HeroClock,
    int HeroAccent,
    int Divider,
    int PlatformForeground,
    int PlatformBackground,
    int RouteForeground,
    int RouteBackground,
    int SelectionForeground,
    int SelectionBackground,
    int Delay,
    int Warning,
    int Success,
    int NoticeMuted,
    int FooterForeground,
    int FooterBackground);

internal static class TransitBoardTheme
{
    public static TransitBoardPalette Default { get; } = Resolve(TransitBoardThemeKind.Meridian);

    public static TransitBoardPalette Resolve(TransitBoardThemeKind kind)
    {
        return kind switch
        {
            TransitBoardThemeKind.Harbor => CreateHarbor(),
            TransitBoardThemeKind.Afterglow => CreateAfterglow(),
            _ => CreateMeridian(),
        };
    }

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

    public static TesseraStyle Chip(int foreground, int background, bool bold = true)
    {
        var style = ForegroundBackground(foreground, background);
        return bold ? style.WithBold() : style;
    }

    private static TransitBoardPalette CreateMeridian()
    {
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = Foreground(0xF3F4EF),
                Secondary = Foreground(0xBFCFCE),
                Muted = Foreground(0x708B8F),
                Inverse = ForegroundBackground(0x081116, 0xF3F4EF),
            },
            Surface = new TesseraThemeSurfaceTokens
            {
                Base = Background(0x081116),
                Panel = Background(0x0C181E),
                Overlay = Background(0x0F2028),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = Foreground(0x274149),
                Strong = Foreground(0x4A7A80),
                Focused = Foreground(0x7BE4D4),
                Error = Foreground(0xFF8A72),
            },
            State = new TesseraThemeStateTokens
            {
                Success = Foreground(0xA5F08F),
                Warning = Foreground(0xFFD36E),
                Error = Foreground(0xFF8A72),
                Info = Foreground(0x80D6FF),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = Foreground(0x7BE4D4),
                Secondary = Foreground(0x80D6FF),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = Foreground(0x081116),
                Background = Background(0xF1F3E7),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = Foreground(0x7BE4D4).WithBold(),
                Title = Foreground(0x7BE4D4).WithBold(),
                Border = Foreground(0x7BE4D4).WithBold(),
                Marker = "◆",
            },
        };

        return new TransitBoardPalette(
            TransitBoardThemeKind.Meridian,
            "Meridian",
            theme,
            0xF3F4EF,
            0x80D6FF,
            0xFFD36E,
            0x2E555C,
            0x081116,
            0xA5F08F,
            0x081116,
            0x7BE4D4,
            0x081116,
            0xF1F3E7,
            0xFF8A72,
            0xFFD36E,
            0xA5F08F,
            0x7E9498,
            0x081116,
            0xC8F6EF);
    }

    private static TransitBoardPalette CreateHarbor()
    {
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = Foreground(0xF9F2E7),
                Secondary = Foreground(0xDCC9B8),
                Muted = Foreground(0x857366),
                Inverse = ForegroundBackground(0x140D0E, 0xF9F2E7),
            },
            Surface = new TesseraThemeSurfaceTokens
            {
                Base = Background(0x140D0E),
                Panel = Background(0x1A1315),
                Overlay = Background(0x21191C),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = Foreground(0x4F393B),
                Strong = Foreground(0x8C6462),
                Focused = Foreground(0xF7B37A),
                Error = Foreground(0xFF7B67),
            },
            State = new TesseraThemeStateTokens
            {
                Success = Foreground(0x8DE0B4),
                Warning = Foreground(0xF6C86E),
                Error = Foreground(0xFF7B67),
                Info = Foreground(0x79C7F2),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = Foreground(0xF7B37A),
                Secondary = Foreground(0x79C7F2),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = Foreground(0x140D0E),
                Background = Background(0xF9F2E7),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = Foreground(0xF7B37A).WithBold(),
                Title = Foreground(0xF7B37A).WithBold(),
                Border = Foreground(0xF7B37A).WithBold(),
                Marker = "◆",
            },
        };

        return new TransitBoardPalette(
            TransitBoardThemeKind.Harbor,
            "Harbor",
            theme,
            0xF9F2E7,
            0x79C7F2,
            0xF6C86E,
            0x67484A,
            0x140D0E,
            0x8DE0B4,
            0x140D0E,
            0xF7B37A,
            0x140D0E,
            0xF9F2E7,
            0xFF7B67,
            0xF6C86E,
            0x8DE0B4,
            0x9E8B7E,
            0x140D0E,
            0xF7E1C2);
    }

    private static TransitBoardPalette CreateAfterglow()
    {
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = Foreground(0xF6EEFF),
                Secondary = Foreground(0xC9B8E4),
                Muted = Foreground(0x8171A0),
                Inverse = ForegroundBackground(0x110A1D, 0xF6EEFF),
            },
            Surface = new TesseraThemeSurfaceTokens
            {
                Base = Background(0x110A1D),
                Panel = Background(0x171024),
                Overlay = Background(0x1D142A),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = Foreground(0x41325E),
                Strong = Foreground(0x7562A0),
                Focused = Foreground(0x64EAD6),
                Error = Foreground(0xFF7AAE),
            },
            State = new TesseraThemeStateTokens
            {
                Success = Foreground(0x85F0B4),
                Warning = Foreground(0xFFD86E),
                Error = Foreground(0xFF7AAE),
                Info = Foreground(0x6ED3FF),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = Foreground(0x64EAD6),
                Secondary = Foreground(0x6ED3FF),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = Foreground(0x110A1D),
                Background = Background(0xF6EEFF),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = Foreground(0x64EAD6).WithBold(),
                Title = Foreground(0x64EAD6).WithBold(),
                Border = Foreground(0x64EAD6).WithBold(),
                Marker = "◆",
            },
        };

        return new TransitBoardPalette(
            TransitBoardThemeKind.Afterglow,
            "Afterglow",
            theme,
            0xF6EEFF,
            0x6ED3FF,
            0xFFD86E,
            0x574077,
            0x110A1D,
            0x85F0B4,
            0x110A1D,
            0x64EAD6,
            0x110A1D,
            0xF6EEFF,
            0xFF7AAE,
            0xFFD86E,
            0x85F0B4,
            0x9387B5,
            0x110A1D,
            0xD9C1FF);
    }

    private static (byte Red, byte Green, byte Blue) Split(int color)
    {
        var red = (byte)((color >> 16) & 0xFF);
        var green = (byte)((color >> 8) & 0xFF);
        var blue = (byte)(color & 0xFF);
        return (red, green, blue);
    }
}
