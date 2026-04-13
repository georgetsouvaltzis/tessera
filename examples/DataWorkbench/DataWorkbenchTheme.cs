using Tessera.Styles;

namespace Tessera.Examples.DataWorkbench;

internal enum DataWorkbenchThemeKind
{
    Citrine,
    Cobalt,
    Ember
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
            0xFFF8ED,
            0xE7CE9C,
            0x8E7A55,
            0x120C05,
            0xFFF8ED,
            0x120C05,
            0x1B1208,
            0x24190D,
            0x6D5130,
            0xC68F4A,
            0xFFD98A,
            0xFF7A59,
            0x8EEFAF,
            0xFFD26D,
            0xFF7A59,
            0x68C7FF,
            0xFFD98A,
            0x68C7FF,
            0x120C05,
            0xFFD98A),
        0xFFF8ED,
        0x68C7FF,
        0x120C05,
        0xFFD98A,
        0x8EEFAF,
        0x6D5130,
        0xC68F4A,
        0xFFD98A,
        0x68C7FF,
        0x8EEFAF,
        0xFFD26D,
        0x68C7FF,
        0xFF7A59,
        0x120C05,
        0xFFD98A);

    public static DataWorkbenchPalette Cobalt { get; } = new(
        DataWorkbenchThemeKind.Cobalt,
        "Cobalt",
        CreateTheme(
            0xF6F8FF,
            0xC1C9E8,
            0x7080A7,
            0x090F1E,
            0xF6F8FF,
            0x090F1E,
            0x111A31,
            0x18264B,
            0x31507D,
            0x5D8EF6,
            0x7CE7FF,
            0xFF8A76,
            0x8AF2C3,
            0xFFD77D,
            0xFF8A76,
            0x7CE7FF,
            0x5D8EF6,
            0x7CE7FF,
            0x090F1E,
            0x7CE7FF),
        0xF6F8FF,
        0x7CE7FF,
        0x090F1E,
        0x7CE7FF,
        0x8AF2C3,
        0x31507D,
        0x5D8EF6,
        0x7CE7FF,
        0xB89BFF,
        0x8AF2C3,
        0x5D8EF6,
        0xFFD77D,
        0xFF8A76,
        0x090F1E,
        0x7CE7FF);

    public static DataWorkbenchPalette Ember { get; } = new(
        DataWorkbenchThemeKind.Ember,
        "Ember",
        CreateTheme(
            0xFFF4F1,
            0xE8B8B0,
            0x956D69,
            0x160908,
            0xFFF4F1,
            0x160908,
            0x21100E,
            0x2D1814,
            0x75433A,
            0xD06F61,
            0xFFB59E,
            0xFF6A57,
            0x90E1A8,
            0xFFC577,
            0xFF6A57,
            0x7AD7FF,
            0xD06F61,
            0xFFB59E,
            0x160908,
            0xFFB59E),
        0xFFF4F1,
        0xFFC577,
        0x160908,
        0xFFB59E,
        0x90E1A8,
        0x75433A,
        0xD06F61,
        0xFFB59E,
        0x7AD7FF,
        0x90E1A8,
        0xFFC577,
        0xFFB59E,
        0xFF6A57,
        0x160908,
        0xFFB59E);

    public static IReadOnlyList<DataWorkbenchPalette> All { get; } = [Citrine, Cobalt, Ember];

    public static DataWorkbenchPalette Resolve(DataWorkbenchThemeKind kind)
    {
        return kind switch
        {
            DataWorkbenchThemeKind.Cobalt => Cobalt,
            DataWorkbenchThemeKind.Ember => Ember,
            _ => Citrine
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
                Ring = Foreground(borderFocused),
                Title = Foreground(borderFocused).WithBold(),
                Border = Foreground(borderFocused).WithBold(),
                Marker = "◆"
            }
        };
    }

    private static (byte Red, byte Green, byte Blue) Split(int color)
    {
        return ((byte)((color >> 16) & 0xFF), (byte)((color >> 8) & 0xFF), (byte)(color & 0xFF));
    }
}
