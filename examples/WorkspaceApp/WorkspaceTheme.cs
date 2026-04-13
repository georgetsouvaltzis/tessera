using Tessera.Styles;

namespace Tessera.Examples.WorkspaceApp;

internal static class WorkspaceTheme
{
    public static TesseraTheme Default => new()
    {
        Text =
            new TesseraThemeTextTokens
            {
                Primary = Foreground(0xEEF7FF),
                Secondary = Foreground(0xAFC4DE),
                Muted = Foreground(0x67829E),
                Inverse = Foreground(0x0A1320)
            },
        Surface =
            new TesseraThemeSurfaceTokens
            {
                Base = Background(0x09111C),
                Panel = Background(0x101C2C),
                Overlay = Background(0x16263C)
            },
        Border =
            new TesseraThemeBorderTokens
            {
                Default = Foreground(0x345A82),
                Strong = Foreground(0x5F92D1),
                Focused = Foreground(0x7DE3FF).WithBold(),
                Error = Foreground(0xFF8D74).WithBold()
            },
        State =
            new TesseraThemeStateTokens
            {
                Success = Foreground(0x86F4B5).WithBold(),
                Warning = Foreground(0xFFD46B).WithBold(),
                Error = Foreground(0xFF8D74).WithBold(),
                Info = Foreground(0x7DE3FF).WithBold()
            },
        Accent =
            new TesseraThemeAccentTokens
            {
                Primary = Foreground(0x7DE3FF).WithBold(),
                Secondary = Foreground(0xFFD46B).WithBold()
            },
        Selection =
            new TesseraThemeSelectionTokens
            {
                Background = Background(0x244566),
                Foreground = Foreground(0xF7FBFF).WithBold()
            },
        Focus = new TesseraThemeFocusTokens
        {
            Ring = Foreground(0xFFD46B).WithBold(),
            Title = Foreground(0xFFD46B).WithBold(),
            Border = Foreground(0x7DE3FF).WithBold(),
            Marker = "◈"
        }
    };

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

    private static AnsiColor Hex(int rgb)
    {
        var r = (byte)((rgb >> 16) & 0xFF);
        var g = (byte)((rgb >> 8) & 0xFF);
        var b = (byte)(rgb & 0xFF);
        return AnsiColor.Rgb(r, g, b);
    }
}
