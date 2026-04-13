using Tessera.Styles;

namespace Tessera.Examples.OpsWatch;

internal enum OpsWatchThemeKind
{
    Veridian,
    Tidal,
    Redline
}

internal sealed record OpsWatchThemePalette(
    OpsWatchThemeKind Kind,
    string Label,
    TesseraTheme Theme,
    int HeroTitleColor,
    int HeroClockColor,
    int HeroBadgeForeground,
    int HeroBadgeBackground,
    int HeroCommandColor,
    int FrameMutedColor,
    int FrameStrongColor,
    int PulsePrimaryColor,
    int PulseSecondaryColor,
    int PulseTertiaryColor,
    int CpuColor,
    int MemoryColor,
    int NetworkColor,
    int DiskColor,
    int FooterChipForeground,
    int FooterChipBackground);

internal static class OpsWatchTheme
{
    public static OpsWatchThemePalette Default => Veridian;

    public static OpsWatchThemePalette Veridian { get; } = new(
        OpsWatchThemeKind.Veridian,
        "Veridian",
        CreateTheme(
            0xE5FFF6,
            0x9FD8C8,
            0x5C7D76,
            0x04100E,
            0xE5FFF6,
            0x04100E,
            0x0A1715,
            0x10221F,
            0x1F5C53,
            0x37917E,
            0x7EFFD3,
            0xFF7B6B,
            0x7BF2AA,
            0xFFD166,
            0xFF7B6B,
            0x57D9FF,
            0x6CFFD1,
            0x57D9FF,
            0x04100E,
            0x7EFFD3),
        0xE5FFF6,
        0x57D9FF,
        0x04100E,
        0x6CFFD1,
        0xFFD166,
        0x1F5C53,
        0x37917E,
        0x6CFFD1,
        0x57D9FF,
        0xFFD166,
        0x7BF2AA,
        0x57D9FF,
        0xFFD166,
        0xFF7B6B,
        0x04100E,
        0x6CFFD1);

    public static OpsWatchThemePalette Tidal { get; } = new(
        OpsWatchThemeKind.Tidal,
        "Tidal",
        CreateTheme(
            0xF2F8FF,
            0xB7C9E8,
            0x65789C,
            0x08111F,
            0xE8F2FF,
            0x08111F,
            0x111B2E,
            0x162844,
            0x2A4D7A,
            0x3D7BCC,
            0x71C7FF,
            0xFF8F70,
            0x8AF2C3,
            0xFFD37A,
            0xFF8F70,
            0x71C7FF,
            0x71C7FF,
            0xA6B7FF,
            0x08111F,
            0x71C7FF),
        0xF2F8FF,
        0xA6B7FF,
        0x08111F,
        0x71C7FF,
        0x8AF2C3,
        0x2A4D7A,
        0x3D7BCC,
        0x71C7FF,
        0xA6B7FF,
        0x8AF2C3,
        0x8AF2C3,
        0x71C7FF,
        0xFFD37A,
        0xFF8F70,
        0x08111F,
        0x71C7FF);

    public static OpsWatchThemePalette Redline { get; } = new(
        OpsWatchThemeKind.Redline,
        "Redline",
        CreateTheme(
            0xFFF5EC,
            0xE7C3A8,
            0x8D7365,
            0x1A0D09,
            0xFFF5EC,
            0x1A0D09,
            0x26130E,
            0x321A13,
            0x754436,
            0xB86A52,
            0xFFB17A,
            0xFF6B57,
            0xF2C572,
            0xFFB17A,
            0xFF6B57,
            0xFF8F70,
            0xFF8F70,
            0xF2C572,
            0x1A0D09,
            0xFFB17A),
        0xFFF5EC,
        0xF2C572,
        0x1A0D09,
        0xFF8F70,
        0xFFB17A,
        0x754436,
        0xB86A52,
        0xFF8F70,
        0xF2C572,
        0xFFB17A,
        0xF2C572,
        0xFFB17A,
        0xFF8F70,
        0xFF6B57,
        0x1A0D09,
        0xFF8F70);

    public static IReadOnlyList<OpsWatchThemePalette> All { get; } = [Veridian, Tidal, Redline];

    public static OpsWatchThemePalette Resolve(OpsWatchThemeKind kind)
    {
        return kind switch
        {
            OpsWatchThemeKind.Tidal => Tidal,
            OpsWatchThemeKind.Redline => Redline,
            _ => Veridian
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

    private static TesseraTheme CreateTheme(
        int textPrimary,
        int textSecondary,
        int textMuted,
        int inverseForeground,
        int inverseBackground,
        int surfaceBase,
        int surfacePanel,
        int surfaceOverlay,
        int borderDefault,
        int borderStrong,
        int borderFocused,
        int borderError,
        int success,
        int warning,
        int error,
        int info,
        int accentPrimary,
        int accentSecondary,
        int selectionForeground,
        int selectionBackground)
    {
        return new TesseraTheme
        {
            Text =
                new TesseraThemeTextTokens
                {
                    Primary = Foreground(textPrimary),
                    Secondary = Foreground(textSecondary),
                    Muted = Foreground(textMuted),
                    Inverse = ForegroundBackground(inverseForeground, inverseBackground)
                },
            Surface =
                new TesseraThemeSurfaceTokens
                {
                    Base = Background(surfaceBase),
                    Panel = Background(surfacePanel),
                    Overlay = Background(surfaceOverlay)
                },
            Border =
                new TesseraThemeBorderTokens
                {
                    Default = Foreground(borderDefault),
                    Strong = Foreground(borderStrong),
                    Focused = Foreground(borderFocused),
                    Error = Foreground(borderError)
                },
            State =
                new TesseraThemeStateTokens
                {
                    Success = Foreground(success),
                    Warning = Foreground(warning),
                    Error = Foreground(error),
                    Info = Foreground(info)
                },
            Accent =
                new TesseraThemeAccentTokens
                {
                    Primary = Foreground(accentPrimary),
                    Secondary = Foreground(accentSecondary)
                },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = Foreground(selectionForeground),
                Background = Background(selectionBackground)
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = Foreground(borderFocused).WithBold(),
                Title = Foreground(borderFocused).WithBold(),
                Border = Foreground(borderFocused).WithBold(),
                Marker = "*"
            }
        };
    }

    private static (byte Red, byte Green, byte Blue) Split(int color)
    {
        var red = (byte)((color >> 16) & 0xFF);
        var green = (byte)((color >> 8) & 0xFF);
        var blue = (byte)(color & 0xFF);
        return (red, green, blue);
    }
}
