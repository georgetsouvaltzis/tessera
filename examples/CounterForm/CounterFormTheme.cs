using Tessera.Styles;

namespace Tessera.Examples.CounterForm;

internal sealed record CounterFormPalette(
    string Name,
    TesseraTheme Theme,
    TesseraStyle CountStyle,
    TesseraStyle SummaryStyle,
    TesseraStyle PositiveButtonStyle,
    TesseraStyle NegativeButtonStyle,
    TesseraStyle NeutralButtonStyle);

internal static class CounterFormTheme
{
    public static CounterFormPalette Default => Aurora;

    public static CounterFormPalette Aurora { get; } = CreatePalette(
        "Aurora",
        0x100E1D,
        0x17142A,
        0x241F42,
        0x6654C8,
        0x92F7D6,
        0xFF8F6B,
        0xA78BFA);

    public static CounterFormPalette Ember { get; } = CreatePalette(
        "Ember",
        0x160D0B,
        0x261310,
        0x351D1A,
        0xF08548,
        0xFFD07A,
        0xFF6E6E,
        0xFFAE63);

    public static CounterFormPalette Tide { get; } = CreatePalette(
        "Tide",
        0x09131B,
        0x0D1D29,
        0x123043,
        0x3F87B7,
        0x83E8FF,
        0x8DF7C4,
        0x4FC3F7);

    public static IReadOnlyList<CounterFormPalette> All { get; } = [Aurora, Ember, Tide];

    public static CounterFormPalette Resolve(string? name)
    {
        return All.FirstOrDefault(palette => string.Equals(palette.Name, name, StringComparison.Ordinal))
               ?? Default;
    }

    public static TesseraStyle Foreground(int rgb)
    {
        return TesseraStyle.Empty.WithForeground(Hex(rgb));
    }

    public static TesseraStyle Background(int rgb)
    {
        return TesseraStyle.Empty.WithBackground(Hex(rgb));
    }

    public static TesseraStyle Surface(int foregroundRgb, int backgroundRgb)
    {
        return Foreground(foregroundRgb).Merge(Background(backgroundRgb));
    }

    private static CounterFormPalette CreatePalette(
        string name,
        int baseRgb,
        int panelRgb,
        int overlayRgb,
        int borderRgb,
        int accentRgb,
        int secondaryRgb,
        int countBackgroundRgb)
    {
        var theme = new TesseraTheme
        {
            Text =
                new TesseraThemeTextTokens
                {
                    Primary = Foreground(0xF7F7FF),
                    Secondary = Foreground(0xC9C4EF),
                    Muted = Foreground(0x8A82B7),
                    Inverse = Foreground(0x090C16)
                },
            Surface =
                new TesseraThemeSurfaceTokens
                {
                    Base = Background(baseRgb),
                    Panel = Background(panelRgb),
                    Overlay = Background(overlayRgb)
                },
            Border =
                new TesseraThemeBorderTokens
                {
                    Default = Foreground(borderRgb),
                    Strong = Foreground(accentRgb),
                    Focused = Foreground(accentRgb).WithBold(),
                    Error = Foreground(0xFF7B7B).WithBold()
                },
            State =
                new TesseraThemeStateTokens
                {
                    Success = Foreground(accentRgb).WithBold(),
                    Warning = Foreground(0xFFD166).WithBold(),
                    Error = Foreground(0xFF7B7B).WithBold(),
                    Info = Foreground(secondaryRgb).WithBold()
                },
            Accent =
                new TesseraThemeAccentTokens
                {
                    Primary = Foreground(accentRgb).WithBold(),
                    Secondary = Foreground(secondaryRgb).WithBold()
                },
            Selection = new TesseraThemeSelectionTokens
            {
                Background = Background(borderRgb),
                Foreground = Foreground(0xF7FAFF).WithBold()
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = Foreground(secondaryRgb).WithBold(),
                Title = Foreground(secondaryRgb).WithBold(),
                Border = Foreground(accentRgb).WithBold(),
                Marker = "◆"
            }
        };

        return new CounterFormPalette(
            name,
            theme,
            Surface(0x090C16, countBackgroundRgb).WithBold(),
            Foreground(accentRgb),
            Surface(0x091016, accentRgb).WithBold(),
            Surface(0x0D0F16, secondaryRgb).WithBold(),
            Surface(0x090C16, 0xFFFFFF).WithBold());
    }

    private static AnsiColor Hex(int rgb)
    {
        var r = (byte)((rgb >> 16) & 0xFF);
        var g = (byte)((rgb >> 8) & 0xFF);
        var b = (byte)(rgb & 0xFF);
        return AnsiColor.Rgb(r, g, b);
    }
}
