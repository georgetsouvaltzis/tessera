using TeaSharp.Styles;

namespace TeaSharp.Examples.WorkspaceApp;

internal static class WorkspaceTheme
{
    public static TeaTheme Default => new()
    {
        Text = new TeaThemeTextTokens
        {
            Primary = Foreground(0xEEF7FF),
            Secondary = Foreground(0xAFC4DE),
            Muted = Foreground(0x67829E),
            Inverse = Foreground(0x0A1320),
        },
        Surface = new TeaThemeSurfaceTokens
        {
            Base = Background(0x09111C),
            Panel = Background(0x101C2C),
            Overlay = Background(0x16263C),
        },
        Border = new TeaThemeBorderTokens
        {
            Default = Foreground(0x345A82),
            Strong = Foreground(0x5F92D1),
            Focused = Foreground(0x7DE3FF).WithBold(),
            Error = Foreground(0xFF8D74).WithBold(),
        },
        State = new TeaThemeStateTokens
        {
            Success = Foreground(0x86F4B5).WithBold(),
            Warning = Foreground(0xFFD46B).WithBold(),
            Error = Foreground(0xFF8D74).WithBold(),
            Info = Foreground(0x7DE3FF).WithBold(),
        },
        Accent = new TeaThemeAccentTokens
        {
            Primary = Foreground(0x7DE3FF).WithBold(),
            Secondary = Foreground(0xFFD46B).WithBold(),
        },
        Selection = new TeaThemeSelectionTokens
        {
            Background = Background(0x244566),
            Foreground = Foreground(0xF7FBFF).WithBold(),
        },
        Focus = new TeaThemeFocusTokens
        {
            Ring = Foreground(0xFFD46B).WithBold(),
            Title = Foreground(0xFFD46B).WithBold(),
            Border = Foreground(0x7DE3FF).WithBold(),
            Marker = "◈",
        },
    };

    public static TeaStyle Foreground(int rgb) => TeaStyle.Empty.WithForeground(Hex(rgb));

    public static TeaStyle Background(int rgb) => TeaStyle.Empty.WithBackground(Hex(rgb));

    public static TeaStyle Surface(int foregroundRgb, int backgroundRgb)
        => Foreground(foregroundRgb).Merge(Background(backgroundRgb));

    private static AnsiColor Hex(int rgb)
    {
        var r = (byte)((rgb >> 16) & 0xFF);
        var g = (byte)((rgb >> 8) & 0xFF);
        var b = (byte)(rgb & 0xFF);
        return AnsiColor.Rgb(r, g, b);
    }
}
