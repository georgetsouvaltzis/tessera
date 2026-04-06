using Tessera.Styles;

namespace Tessera.Examples.OpsWatch;

internal enum OpsWatchThemeKind
{
    Veridian,
    Tidal,
    Redline,
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
            textPrimary: 0xE5FFF6,
            textSecondary: 0x9FD8C8,
            textMuted: 0x5C7D76,
            inverseForeground: 0x04100E,
            inverseBackground: 0xE5FFF6,
            surfaceBase: 0x04100E,
            surfacePanel: 0x0A1715,
            surfaceOverlay: 0x10221F,
            borderDefault: 0x1F5C53,
            borderStrong: 0x37917E,
            borderFocused: 0x7EFFD3,
            borderError: 0xFF7B6B,
            success: 0x7BF2AA,
            warning: 0xFFD166,
            error: 0xFF7B6B,
            info: 0x57D9FF,
            accentPrimary: 0x6CFFD1,
            accentSecondary: 0x57D9FF,
            selectionForeground: 0x04100E,
            selectionBackground: 0x7EFFD3),
        HeroTitleColor: 0xE5FFF6,
        HeroClockColor: 0x57D9FF,
        HeroBadgeForeground: 0x04100E,
        HeroBadgeBackground: 0x6CFFD1,
        HeroCommandColor: 0xFFD166,
        FrameMutedColor: 0x1F5C53,
        FrameStrongColor: 0x37917E,
        PulsePrimaryColor: 0x6CFFD1,
        PulseSecondaryColor: 0x57D9FF,
        PulseTertiaryColor: 0xFFD166,
        CpuColor: 0x7BF2AA,
        MemoryColor: 0x57D9FF,
        NetworkColor: 0xFFD166,
        DiskColor: 0xFF7B6B,
        FooterChipForeground: 0x04100E,
        FooterChipBackground: 0x6CFFD1);

    public static OpsWatchThemePalette Tidal { get; } = new(
        OpsWatchThemeKind.Tidal,
        "Tidal",
        CreateTheme(
            textPrimary: 0xF2F8FF,
            textSecondary: 0xB7C9E8,
            textMuted: 0x65789C,
            inverseForeground: 0x08111F,
            inverseBackground: 0xE8F2FF,
            surfaceBase: 0x08111F,
            surfacePanel: 0x111B2E,
            surfaceOverlay: 0x162844,
            borderDefault: 0x2A4D7A,
            borderStrong: 0x3D7BCC,
            borderFocused: 0x71C7FF,
            borderError: 0xFF8F70,
            success: 0x8AF2C3,
            warning: 0xFFD37A,
            error: 0xFF8F70,
            info: 0x71C7FF,
            accentPrimary: 0x71C7FF,
            accentSecondary: 0xA6B7FF,
            selectionForeground: 0x08111F,
            selectionBackground: 0x71C7FF),
        HeroTitleColor: 0xF2F8FF,
        HeroClockColor: 0xA6B7FF,
        HeroBadgeForeground: 0x08111F,
        HeroBadgeBackground: 0x71C7FF,
        HeroCommandColor: 0x8AF2C3,
        FrameMutedColor: 0x2A4D7A,
        FrameStrongColor: 0x3D7BCC,
        PulsePrimaryColor: 0x71C7FF,
        PulseSecondaryColor: 0xA6B7FF,
        PulseTertiaryColor: 0x8AF2C3,
        CpuColor: 0x8AF2C3,
        MemoryColor: 0x71C7FF,
        NetworkColor: 0xFFD37A,
        DiskColor: 0xFF8F70,
        FooterChipForeground: 0x08111F,
        FooterChipBackground: 0x71C7FF);

    public static OpsWatchThemePalette Redline { get; } = new(
        OpsWatchThemeKind.Redline,
        "Redline",
        CreateTheme(
            textPrimary: 0xFFF5EC,
            textSecondary: 0xE7C3A8,
            textMuted: 0x8D7365,
            inverseForeground: 0x1A0D09,
            inverseBackground: 0xFFF5EC,
            surfaceBase: 0x1A0D09,
            surfacePanel: 0x26130E,
            surfaceOverlay: 0x321A13,
            borderDefault: 0x754436,
            borderStrong: 0xB86A52,
            borderFocused: 0xFFB17A,
            borderError: 0xFF6B57,
            success: 0xF2C572,
            warning: 0xFFB17A,
            error: 0xFF6B57,
            info: 0xFF8F70,
            accentPrimary: 0xFF8F70,
            accentSecondary: 0xF2C572,
            selectionForeground: 0x1A0D09,
            selectionBackground: 0xFFB17A),
        HeroTitleColor: 0xFFF5EC,
        HeroClockColor: 0xF2C572,
        HeroBadgeForeground: 0x1A0D09,
        HeroBadgeBackground: 0xFF8F70,
        HeroCommandColor: 0xFFB17A,
        FrameMutedColor: 0x754436,
        FrameStrongColor: 0xB86A52,
        PulsePrimaryColor: 0xFF8F70,
        PulseSecondaryColor: 0xF2C572,
        PulseTertiaryColor: 0xFFB17A,
        CpuColor: 0xF2C572,
        MemoryColor: 0xFFB17A,
        NetworkColor: 0xFF8F70,
        DiskColor: 0xFF6B57,
        FooterChipForeground: 0x1A0D09,
        FooterChipBackground: 0xFF8F70);

    public static IReadOnlyList<OpsWatchThemePalette> All { get; } = [Veridian, Tidal, Redline];

    public static OpsWatchThemePalette Resolve(OpsWatchThemeKind kind)
    {
        return kind switch
        {
            OpsWatchThemeKind.Tidal => Tidal,
            OpsWatchThemeKind.Redline => Redline,
            _ => Veridian,
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
            Text = new TesseraThemeTextTokens
            {
                Primary = Foreground(textPrimary),
                Secondary = Foreground(textSecondary),
                Muted = Foreground(textMuted),
                Inverse = ForegroundBackground(inverseForeground, inverseBackground),
            },
            Surface = new TesseraThemeSurfaceTokens
            {
                Base = Background(surfaceBase),
                Panel = Background(surfacePanel),
                Overlay = Background(surfaceOverlay),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = Foreground(borderDefault),
                Strong = Foreground(borderStrong),
                Focused = Foreground(borderFocused),
                Error = Foreground(borderError),
            },
            State = new TesseraThemeStateTokens
            {
                Success = Foreground(success),
                Warning = Foreground(warning),
                Error = Foreground(error),
                Info = Foreground(info),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = Foreground(accentPrimary),
                Secondary = Foreground(accentSecondary),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = Foreground(selectionForeground),
                Background = Background(selectionBackground),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = Foreground(borderFocused).WithBold(),
                Title = Foreground(borderFocused).WithBold(),
                Border = Foreground(borderFocused).WithBold(),
                Marker = "*",
            },
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
