namespace TeaSharp.Styles;

/// <summary>
/// Provides built-in TeaSharp theme palettes.
/// </summary>
public static class TeaThemes
{
    /// <summary>
    /// Creates a Catppuccin theme variant.
    /// </summary>
    /// <param name="variant">The Catppuccin variant.</param>
    /// <returns>A <see cref="TeaTheme"/> configured from the selected Catppuccin palette.</returns>
    public static TeaTheme Catppuccin(CatppuccinVariant variant = CatppuccinVariant.Mocha)
    {
        var palette = variant switch
        {
            CatppuccinVariant.Latte => new Palette(
                baseSurface: 0xEFF1F5,
                panelSurface: 0xE6E9EF,
                overlaySurface: 0xDCE0E8,
                textPrimary: 0x4C4F69,
                textSecondary: 0x5C5F77,
                textMuted: 0x6C6F85,
                borderDefault: 0x9CA0B0,
                borderStrong: 0x8C8FA1,
                focus: 0x1E66F5,
                accentPrimary: 0x8839EF,
                accentSecondary: 0xEA76CB,
                info: 0x209FB5,
                success: 0x40A02B,
                warning: 0xDF8E1D,
                error: 0xD20F39),
            CatppuccinVariant.Frappe => new Palette(
                baseSurface: 0x303446,
                panelSurface: 0x292C3C,
                overlaySurface: 0x232634,
                textPrimary: 0xC6D0F5,
                textSecondary: 0xB5BFE2,
                textMuted: 0xA5ADCE,
                borderDefault: 0x737994,
                borderStrong: 0x838BA7,
                focus: 0x8CAAEE,
                accentPrimary: 0xCA9EE6,
                accentSecondary: 0xF4B8E4,
                info: 0x85C1DC,
                success: 0xA6D189,
                warning: 0xE5C890,
                error: 0xE78284),
            CatppuccinVariant.Macchiato => new Palette(
                baseSurface: 0x24273A,
                panelSurface: 0x1E2030,
                overlaySurface: 0x181926,
                textPrimary: 0xCAD3F5,
                textSecondary: 0xB8C0E0,
                textMuted: 0xA5ADCB,
                borderDefault: 0x6E738D,
                borderStrong: 0x8087A2,
                focus: 0x8AADF4,
                accentPrimary: 0xC6A0F6,
                accentSecondary: 0xF5BDE6,
                info: 0x7DC4E4,
                success: 0xA6DA95,
                warning: 0xEED49F,
                error: 0xED8796),
            _ => new Palette(
                baseSurface: 0x1E1E2E,
                panelSurface: 0x181825,
                overlaySurface: 0x11111B,
                textPrimary: 0xCDD6F4,
                textSecondary: 0xBAC2DE,
                textMuted: 0xA6ADC8,
                borderDefault: 0x6C7086,
                borderStrong: 0x7F849C,
                focus: 0x89B4FA,
                accentPrimary: 0xCBA6F7,
                accentSecondary: 0xF5C2E7,
                info: 0x74C7EC,
                success: 0xA6E3A1,
                warning: 0xF9E2AF,
                error: 0xF38BA8),
        };

        return CreateTheme(palette);
    }

    /// <summary>
    /// Creates a Rosé Pine theme variant.
    /// </summary>
    /// <param name="variant">The Rosé Pine variant.</param>
    /// <returns>A <see cref="TeaTheme"/> configured from the selected Rosé Pine palette.</returns>
    public static TeaTheme RosePine(RosePineVariant variant = RosePineVariant.Main)
    {
        var palette = variant switch
        {
            RosePineVariant.Moon => new Palette(
                baseSurface: 0x232136,
                panelSurface: 0x2A273F,
                overlaySurface: 0x393552,
                textPrimary: 0xE0DEF4,
                textSecondary: 0xC4A7E7,
                textMuted: 0x908CAA,
                borderDefault: 0x6E6A86,
                borderStrong: 0x908CAA,
                focus: 0x9CCFD8,
                accentPrimary: 0xC4A7E7,
                accentSecondary: 0xEA9A97,
                info: 0x3E8FB0,
                success: 0x9CCFD8,
                warning: 0xF6C177,
                error: 0xEB6F92),
            RosePineVariant.Dawn => new Palette(
                baseSurface: 0xFAF4ED,
                panelSurface: 0xFFF8F0,
                overlaySurface: 0xF2E9DE,
                textPrimary: 0x575279,
                textSecondary: 0x797593,
                textMuted: 0x9893A5,
                borderDefault: 0xBEBBCB,
                borderStrong: 0x9893A5,
                focus: 0x56949F,
                accentPrimary: 0x907AA9,
                accentSecondary: 0xD7827E,
                info: 0x286983,
                success: 0x56949F,
                warning: 0xEA9D34,
                error: 0xB4637A),
            _ => new Palette(
                baseSurface: 0x191724,
                panelSurface: 0x1F1D2E,
                overlaySurface: 0x26233A,
                textPrimary: 0xE0DEF4,
                textSecondary: 0xC4A7E7,
                textMuted: 0x908CAA,
                borderDefault: 0x6E6A86,
                borderStrong: 0x908CAA,
                focus: 0x9CCFD8,
                accentPrimary: 0xC4A7E7,
                accentSecondary: 0xEBBCBA,
                info: 0x31748F,
                success: 0x9CCFD8,
                warning: 0xF6C177,
                error: 0xEB6F92),
        };

        return CreateTheme(palette);
    }

    private static TeaTheme CreateTheme(Palette palette)
    {
        var baseForeground = StyleForeground(palette.TextPrimary);
        var baseBackground = StyleBackground(palette.BaseSurface);

        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = baseForeground,
                Secondary = StyleForeground(palette.TextSecondary),
                Muted = StyleForeground(palette.TextMuted),
                Inverse = StyleForegroundBackground(palette.BaseSurface, palette.TextPrimary),
            },
            Surface = new TeaThemeSurfaceTokens
            {
                Base = baseBackground,
                Panel = StyleBackground(palette.PanelSurface),
                Overlay = StyleBackground(palette.OverlaySurface),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = StyleForeground(palette.BorderDefault),
                Strong = StyleForeground(palette.BorderStrong),
                Focused = StyleForeground(palette.Focus),
                Error = StyleForeground(palette.Error),
            },
            State = new TeaThemeStateTokens
            {
                Success = StyleForeground(palette.Success),
                Warning = StyleForeground(palette.Warning),
                Error = StyleForeground(palette.Error),
                Info = StyleForeground(palette.Info),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = StyleForeground(palette.AccentPrimary),
                Secondary = StyleForeground(palette.AccentSecondary),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = StyleForeground(palette.TextPrimary),
                Background = StyleBackground(palette.Focus),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = StyleForeground(palette.Focus),
                Title = StyleForeground(palette.Focus).WithBold(),
                Border = StyleForeground(palette.Focus).WithBold(),
                Marker = "*",
            },
        };
    }

    private static TeaStyle StyleForeground(int color)
    {
        var (r, g, b) = SplitColor(color);
        return TeaStyle.Empty.WithForeground(AnsiColor.Rgb(r, g, b));
    }

    private static TeaStyle StyleBackground(int color)
    {
        var (r, g, b) = SplitColor(color);
        return TeaStyle.Empty.WithBackground(AnsiColor.Rgb(r, g, b));
    }

    private static TeaStyle StyleForegroundBackground(int foreground, int background)
    {
        var (fgR, fgG, fgB) = SplitColor(foreground);
        var (bgR, bgG, bgB) = SplitColor(background);
        return TeaStyle.Empty
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
