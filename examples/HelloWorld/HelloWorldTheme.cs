using TeaSharp.Styles;

namespace TeaSharp.Examples.HelloWorld;

internal static class HelloWorldTheme
{
    public static TeaTheme Default => new()
    {
        Text = new TeaThemeTextTokens
        {
            Primary = Foreground(0xF6F8FF),
            Secondary = Foreground(0xA8B2D8),
            Muted = Foreground(0x6E7597),
            Inverse = Foreground(0x0A1020),
        },
        Surface = new TeaThemeSurfaceTokens
        {
            Base = Background(0x090D1A),
            Panel = Background(0x11172C),
            Overlay = Background(0x18203D),
        },
        Border = new TeaThemeBorderTokens
        {
            Default = Foreground(0x394A7C),
            Strong = Foreground(0x6D84FF),
            Focused = Foreground(0x67F7C7).WithBold(),
            Error = Foreground(0xFF8E72).WithBold(),
        },
        State = new TeaThemeStateTokens
        {
            Success = Foreground(0x67F7C7).WithBold(),
            Warning = Foreground(0xFFD166).WithBold(),
            Error = Foreground(0xFF8E72).WithBold(),
            Info = Foreground(0x8AD8FF).WithBold(),
        },
        Accent = new TeaThemeAccentTokens
        {
            Primary = Foreground(0x67F7C7).WithBold(),
            Secondary = Foreground(0xFFB86B).WithBold(),
        },
        Selection = new TeaThemeSelectionTokens
        {
            Background = Background(0x293B67),
            Foreground = Foreground(0xF8FBFF).WithBold(),
        },
        Focus = new TeaThemeFocusTokens
        {
            Ring = Foreground(0xFFB86B).WithBold(),
            Title = Foreground(0xFFB86B).WithBold(),
            Border = Foreground(0x67F7C7).WithBold(),
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
