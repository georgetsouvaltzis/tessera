namespace TeaSharp.Styles;

/// <summary>
/// Represents a semantic theme for TeaSharp controls.
/// </summary>
public sealed class TeaTheme
{
    public TeaTheme()
    {
    }

    public TeaTheme(
        TeaThemeTextTokens text,
        TeaThemeSurfaceTokens surface,
        TeaThemeBorderTokens border,
        TeaThemeStateTokens state,
        TeaThemeAccentTokens accent,
        TeaThemeSelectionTokens selection,
        TeaThemeFocusTokens focus)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Surface = surface ?? throw new ArgumentNullException(nameof(surface));
        Border = border ?? throw new ArgumentNullException(nameof(border));
        State = state ?? throw new ArgumentNullException(nameof(state));
        Accent = accent ?? throw new ArgumentNullException(nameof(accent));
        Selection = selection ?? throw new ArgumentNullException(nameof(selection));
        Focus = focus ?? throw new ArgumentNullException(nameof(focus));
    }

    public TeaThemeTextTokens Text { get; init; } = new();

    public TeaThemeSurfaceTokens Surface { get; init; } = new();

    public TeaThemeBorderTokens Border { get; init; } = new();

    public TeaThemeStateTokens State { get; init; } = new();

    public TeaThemeAccentTokens Accent { get; init; } = new();

    public TeaThemeSelectionTokens Selection { get; init; } = new();

    public TeaThemeFocusTokens Focus { get; init; } = new();
}
