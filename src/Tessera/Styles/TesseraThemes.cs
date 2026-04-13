namespace Tessera.Styles;

/// <summary>
///     Provides built-in Tessera theme palettes.
/// </summary>
public static class TesseraThemes
{
    /// <summary>
    ///     Creates a Catppuccin theme variant.
    /// </summary>
    /// <param name="variant">The Catppuccin variant.</param>
    /// <returns>A <see cref="TesseraTheme" /> configured from the selected Catppuccin palette.</returns>
    public static TesseraTheme Catppuccin(CatppuccinVariant variant = CatppuccinVariant.Mocha)
    {
        var palette = variant switch
        {
            CatppuccinVariant.Latte => new Palette(
                0xEFF1F5,
                0xE6E9EF,
                0xDCE0E8,
                0x4C4F69,
                0x5C5F77,
                0x6C6F85,
                0x9CA0B0,
                0x8C8FA1,
                0x1E66F5,
                0x8839EF,
                0xEA76CB,
                0x209FB5,
                0x40A02B,
                0xDF8E1D,
                0xD20F39),
            CatppuccinVariant.Frappe => new Palette(
                0x303446,
                0x292C3C,
                0x232634,
                0xC6D0F5,
                0xB5BFE2,
                0xA5ADCE,
                0x737994,
                0x838BA7,
                0x8CAAEE,
                0xCA9EE6,
                0xF4B8E4,
                0x85C1DC,
                0xA6D189,
                0xE5C890,
                0xE78284),
            CatppuccinVariant.Macchiato => new Palette(
                0x24273A,
                0x1E2030,
                0x181926,
                0xCAD3F5,
                0xB8C0E0,
                0xA5ADCB,
                0x6E738D,
                0x8087A2,
                0x8AADF4,
                0xC6A0F6,
                0xF5BDE6,
                0x7DC4E4,
                0xA6DA95,
                0xEED49F,
                0xED8796),
            _ => new Palette(
                0x1E1E2E,
                0x181825,
                0x11111B,
                0xCDD6F4,
                0xBAC2DE,
                0xA6ADC8,
                0x6C7086,
                0x7F849C,
                0x89B4FA,
                0xCBA6F7,
                0xF5C2E7,
                0x74C7EC,
                0xA6E3A1,
                0xF9E2AF,
                0xF38BA8)
        };

        return CreateTheme(palette);
    }

    /// <summary>
    ///     Creates a Rosé Pine theme variant.
    /// </summary>
    /// <param name="variant">The Rosé Pine variant.</param>
    /// <returns>A <see cref="TesseraTheme" /> configured from the selected Rosé Pine palette.</returns>
    public static TesseraTheme RosePine(RosePineVariant variant = RosePineVariant.Main)
    {
        var palette = variant switch
        {
            RosePineVariant.Moon => new Palette(
                0x232136,
                0x2A273F,
                0x393552,
                0xE0DEF4,
                0xC4A7E7,
                0x908CAA,
                0x6E6A86,
                0x908CAA,
                0x9CCFD8,
                0xC4A7E7,
                0xEA9A97,
                0x3E8FB0,
                0x9CCFD8,
                0xF6C177,
                0xEB6F92),
            RosePineVariant.Dawn => new Palette(
                0xFAF4ED,
                0xFFF8F0,
                0xF2E9DE,
                0x575279,
                0x797593,
                0x9893A5,
                0xBEBBCB,
                0x9893A5,
                0x56949F,
                0x907AA9,
                0xD7827E,
                0x286983,
                0x56949F,
                0xEA9D34,
                0xB4637A),
            _ => new Palette(
                0x191724,
                0x1F1D2E,
                0x26233A,
                0xE0DEF4,
                0xC4A7E7,
                0x908CAA,
                0x6E6A86,
                0x908CAA,
                0x9CCFD8,
                0xC4A7E7,
                0xEBBCBA,
                0x31748F,
                0x9CCFD8,
                0xF6C177,
                0xEB6F92)
        };

        return CreateTheme(palette);
    }

    private static TesseraTheme CreateTheme(Palette palette)
    {
        var baseForeground = StyleForeground(palette.TextPrimary);
        var baseBackground = StyleBackground(palette.BaseSurface);

        return new TesseraTheme
        {
            Text =
                new TesseraThemeTextTokens
                {
                    Primary = baseForeground,
                    Secondary = StyleForeground(palette.TextSecondary),
                    Muted = StyleForeground(palette.TextMuted),
                    Inverse = StyleForegroundBackground(palette.BaseSurface, palette.TextPrimary)
                },
            Surface =
                new TesseraThemeSurfaceTokens
                {
                    Base = baseBackground,
                    Panel = StyleBackground(palette.PanelSurface),
                    Overlay = StyleBackground(palette.OverlaySurface)
                },
            Border =
                new TesseraThemeBorderTokens
                {
                    Default = StyleForeground(palette.BorderDefault),
                    Strong = StyleForeground(palette.BorderStrong),
                    Focused = StyleForeground(palette.Focus),
                    Error = StyleForeground(palette.Error)
                },
            State =
                new TesseraThemeStateTokens
                {
                    Success = StyleForeground(palette.Success),
                    Warning = StyleForeground(palette.Warning),
                    Error = StyleForeground(palette.Error),
                    Info = StyleForeground(palette.Info)
                },
            Accent =
                new TesseraThemeAccentTokens
                {
                    Primary = StyleForeground(palette.AccentPrimary),
                    Secondary = StyleForeground(palette.AccentSecondary)
                },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = StyleForeground(palette.TextPrimary),
                Background = StyleBackground(palette.Focus)
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = StyleForeground(palette.Focus),
                Title = StyleForeground(palette.Focus).WithBold(),
                Border = StyleForeground(palette.Focus).WithBold(),
                Marker = "*"
            }
        };
    }

    private static TesseraStyle StyleForeground(int color)
    {
        var (r, g, b) = SplitColor(color);
        return TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(r, g, b));
    }

    private static TesseraStyle StyleBackground(int color)
    {
        var (r, g, b) = SplitColor(color);
        return TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(r, g, b));
    }

    private static TesseraStyle StyleForegroundBackground(int foreground, int background)
    {
        var (fgR, fgG, fgB) = SplitColor(foreground);
        var (bgR, bgG, bgB) = SplitColor(background);
        return TesseraStyle.Empty
            .WithForeground(AnsiColor.Rgb(fgR, fgG, fgB))
            .WithBackground(AnsiColor.Rgb(bgR, bgG, bgB));
    }

    private static (byte R, byte G, byte B) SplitColor(int color)
    {
        var red = (byte)((color >> 16) & 0xFF);
        var green = (byte)((color >> 8) & 0xFF);
        var blue = (byte)(color & 0xFF);
        return (red, green, blue);
    }

    private readonly record struct Palette(
        int baseSurface,
        int panelSurface,
        int overlaySurface,
        int textPrimary,
        int textSecondary,
        int textMuted,
        int borderDefault,
        int borderStrong,
        int focus,
        int accentPrimary,
        int accentSecondary,
        int info,
        int success,
        int warning,
        int error)
    {
        public int BaseSurface { get; } = baseSurface;
        public int PanelSurface { get; } = panelSurface;
        public int OverlaySurface { get; } = overlaySurface;
        public int TextPrimary { get; } = textPrimary;
        public int TextSecondary { get; } = textSecondary;
        public int TextMuted { get; } = textMuted;
        public int BorderDefault { get; } = borderDefault;
        public int BorderStrong { get; } = borderStrong;
        public int Focus { get; } = focus;
        public int AccentPrimary { get; } = accentPrimary;
        public int AccentSecondary { get; } = accentSecondary;
        public int Info { get; } = info;
        public int Success { get; } = success;
        public int Warning { get; } = warning;
        public int Error { get; } = error;
    }
}
