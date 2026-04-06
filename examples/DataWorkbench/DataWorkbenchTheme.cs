using Tessera.Styles;

namespace Tessera.Examples.DataWorkbench;

internal enum DataWorkbenchThemeKind
{
    Citrine,
    Cobalt,
    Ember,
}

internal sealed record DataWorkbenchPalette(
    DataWorkbenchThemeKind Kind,
    string Label,
    TesseraTheme Theme,
    int HeroTitle,
    int HeroClock,
    int HeroBadgeForeground,
    int HeroBadgeBackground,
    int HeroAccent,
    int FrameMuted,
    int FrameStrong,
    int StatPrimary,
    int StatSecondary,
    int StatTertiary,
    int HighlightA,
    int HighlightB,
    int HighlightC,
    int FooterChipForeground,
    int FooterChipBackground);

internal static class DataWorkbenchTheme
{
    public static DataWorkbenchPalette Default => Citrine;

    public static DataWorkbenchPalette Citrine { get; } = new(
        DataWorkbenchThemeKind.Citrine,
        "Citrine",
        CreateTheme(
            textPrimary: 0xFFF8ED,
            textSecondary: 0xE7CE9C,
            textMuted: 0x8E7A55,
            inverseForeground: 0x120C05,
            inverseBackground: 0xFFF8ED,
            surfaceBase: 0x120C05,
            surfacePanel: 0x1B1208,
            surfaceOverlay: 0x24190D,
            borderDefault: 0x6D5130,
            borderStrong: 0xC68F4A,
            borderFocused: 0xFFD98A,
            borderError: 0xFF7A59,
            success: 0x8EEFAF,
            warning: 0xFFD26D,
            error: 0xFF7A59,
            info: 0x68C7FF,
            accentPrimary: 0xFFD98A,
            accentSecondary: 0x68C7FF,
            selectionForeground: 0x120C05,
            selectionBackground: 0xFFD98A),
        HeroTitle: 0xFFF8ED,
        HeroClock: 0x68C7FF,
        HeroBadgeForeground: 0x120C05,
        HeroBadgeBackground: 0xFFD98A,
        HeroAccent: 0x8EEFAF,
        FrameMuted: 0x6D5130,
        FrameStrong: 0xC68F4A,
        StatPrimary: 0xFFD98A,
        StatSecondary: 0x68C7FF,
        StatTertiary: 0x8EEFAF,
        HighlightA: 0xFFD26D,
        HighlightB: 0x68C7FF,
        HighlightC: 0xFF7A59,
        FooterChipForeground: 0x120C05,
        FooterChipBackground: 0xFFD98A);

    public static DataWorkbenchPalette Cobalt { get; } = new(
        DataWorkbenchThemeKind.Cobalt,
        "Cobalt",
        CreateTheme(
            textPrimary: 0xF6F8FF,
            textSecondary: 0xC1C9E8,
            textMuted: 0x7080A7,
            inverseForeground: 0x090F1E,
            inverseBackground: 0xF6F8FF,
            surfaceBase: 0x090F1E,
            surfacePanel: 0x111A31,
            surfaceOverlay: 0x18264B,
            borderDefault: 0x31507D,
            borderStrong: 0x5D8EF6,
            borderFocused: 0x7CE7FF,
            borderError: 0xFF8A76,
            success: 0x8AF2C3,
            warning: 0xFFD77D,
            error: 0xFF8A76,
            info: 0x7CE7FF,
            accentPrimary: 0x5D8EF6,
            accentSecondary: 0x7CE7FF,
            selectionForeground: 0x090F1E,
            selectionBackground: 0x7CE7FF),
        HeroTitle: 0xF6F8FF,
        HeroClock: 0x7CE7FF,
        HeroBadgeForeground: 0x090F1E,
        HeroBadgeBackground: 0x7CE7FF,
        HeroAccent: 0x8AF2C3,
        FrameMuted: 0x31507D,
        FrameStrong: 0x5D8EF6,
        StatPrimary: 0x7CE7FF,
        StatSecondary: 0xB89BFF,
        StatTertiary: 0x8AF2C3,
        HighlightA: 0x5D8EF6,
        HighlightB: 0xFFD77D,
        HighlightC: 0xFF8A76,
        FooterChipForeground: 0x090F1E,
        FooterChipBackground: 0x7CE7FF);

    public static DataWorkbenchPalette Ember { get; } = new(
        DataWorkbenchThemeKind.Ember,
        "Ember",
        CreateTheme(
            textPrimary: 0xFFF4F1,
            textSecondary: 0xE8B8B0,
            textMuted: 0x956D69,
            inverseForeground: 0x160908,
            inverseBackground: 0xFFF4F1,
            surfaceBase: 0x160908,
            surfacePanel: 0x21100E,
            surfaceOverlay: 0x2D1814,
            borderDefault: 0x75433A,
            borderStrong: 0xD06F61,
            borderFocused: 0xFFB59E,
            borderError: 0xFF6A57,
            success: 0x90E1A8,
            warning: 0xFFC577,
            error: 0xFF6A57,
            info: 0x7AD7FF,
            accentPrimary: 0xD06F61,
            accentSecondary: 0xFFB59E,
            selectionForeground: 0x160908,
            selectionBackground: 0xFFB59E),
        HeroTitle: 0xFFF4F1,
        HeroClock: 0xFFC577,
        HeroBadgeForeground: 0x160908,
        HeroBadgeBackground: 0xFFB59E,
        HeroAccent: 0x90E1A8,
        FrameMuted: 0x75433A,
        FrameStrong: 0xD06F61,
        StatPrimary: 0xFFB59E,
        StatSecondary: 0x7AD7FF,
        StatTertiary: 0x90E1A8,
        HighlightA: 0xFFC577,
        HighlightB: 0xFFB59E,
        HighlightC: 0xFF6A57,
        FooterChipForeground: 0x160908,
        FooterChipBackground: 0xFFB59E);

    public static IReadOnlyList<DataWorkbenchPalette> All { get; } = [Citrine, Cobalt, Ember];

    public static DataWorkbenchPalette Resolve(DataWorkbenchThemeKind kind)
    {
        return kind switch
        {
            DataWorkbenchThemeKind.Cobalt => Cobalt,
            DataWorkbenchThemeKind.Ember => Ember,
            _ => Citrine,
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

    public static TesseraStyle Chip(int foreground, int background, bool bold = true)
    {
        var style = Foreground(foreground).Merge(Background(background));
        return bold ? style.WithBold() : style;
    }

    public static TesseraStyle ForegroundBackground(int foreground, int background)
    {
        return Foreground(foreground).Merge(Background(background));
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
                Ring = Foreground(borderFocused),
                Title = Foreground(borderFocused).WithBold(),
                Border = Foreground(borderFocused).WithBold(),
                Marker = "◆",
            },
        };
    }

    private static (byte Red, byte Green, byte Blue) Split(int color)
    {
        return ((byte)((color >> 16) & 0xFF), (byte)((color >> 8) & 0xFF), (byte)(color & 0xFF));
    }
}
