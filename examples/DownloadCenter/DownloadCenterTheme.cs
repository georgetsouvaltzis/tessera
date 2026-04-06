using Tessera.Styles;

namespace Tessera.Examples.DownloadCenter;

internal static class DownloadCenterTheme
{
    public static TesseraTheme Default => new()
    {
        Text = new TesseraThemeTextTokens
        {
            Primary = Foreground(0xEAF2FF),
            Secondary = Foreground(0xA5B4D4),
            Muted = Foreground(0x6B7899),
            Inverse = Foreground(0x101426),
        },
        Surface = new TesseraThemeSurfaceTokens
        {
            Base = Background(0x0B1020),
            Panel = Background(0x121A30),
            Overlay = Background(0x17213B),
        },
        Border = new TesseraThemeBorderTokens
        {
            Default = Foreground(0x34517B),
            Strong = Foreground(0x4A77B8),
            Focused = Foreground(0x7FDBFF).WithBold(),
            Error = Foreground(0xFF7C8A).WithBold(),
        },
        State = new TesseraThemeStateTokens
        {
            Success = Foreground(0x5EF0A5).WithBold(),
            Warning = Foreground(0xFFD166).WithBold(),
            Error = Foreground(0xFF7C8A).WithBold(),
            Info = Foreground(0x7FDBFF).WithBold(),
        },
        Accent = new TesseraThemeAccentTokens
        {
            Primary = Foreground(0x7FDBFF).WithBold(),
            Secondary = Foreground(0xD5B3FF).WithBold(),
        },
        Selection = new TesseraThemeSelectionTokens
        {
            Background = Background(0x234A77),
            Foreground = Foreground(0xF5FBFF).WithBold(),
        },
        Focus = new TesseraThemeFocusTokens
        {
            Ring = Foreground(0xFF9B71).WithBold(),
            Title = Foreground(0xFF9B71).WithBold(),
            Border = Foreground(0xFF9B71).WithBold(),
            Marker = "◆",
        },
    };

    public static TesseraStyle Foreground(int rgb) => TesseraStyle.Empty.WithForeground(Hex(rgb));

    public static TesseraStyle Background(int rgb) => TesseraStyle.Empty.WithBackground(Hex(rgb));

    public static TesseraStyle Chip(int foregroundRgb, int backgroundRgb)
    {
        return Foreground(foregroundRgb).Merge(Background(backgroundRgb)).WithBold();
    }

    private static AnsiColor Hex(int rgb)
    {
        var r = (byte)((rgb >> 16) & 0xFF);
        var g = (byte)((rgb >> 8) & 0xFF);
        var b = (byte)(rgb & 0xFF);
        return AnsiColor.Rgb(r, g, b);
    }
}
