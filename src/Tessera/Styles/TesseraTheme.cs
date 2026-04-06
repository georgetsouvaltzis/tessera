namespace Tessera.Styles;

/// <summary>
/// Represents a semantic theme for Tessera controls.
/// </summary>
public sealed class TesseraTheme
{
    public TesseraTheme()
    {
    }

    public TesseraTheme(
        TesseraThemeTextTokens text,
        TesseraThemeSurfaceTokens surface,
        TesseraThemeBorderTokens border,
        TesseraThemeStateTokens state,
        TesseraThemeAccentTokens accent,
        TesseraThemeSelectionTokens selection,
        TesseraThemeFocusTokens focus)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Surface = surface ?? throw new ArgumentNullException(nameof(surface));
        Border = border ?? throw new ArgumentNullException(nameof(border));
        State = state ?? throw new ArgumentNullException(nameof(state));
        Accent = accent ?? throw new ArgumentNullException(nameof(accent));
        Selection = selection ?? throw new ArgumentNullException(nameof(selection));
        Focus = focus ?? throw new ArgumentNullException(nameof(focus));
    }

    public TesseraThemeTextTokens Text { get; init; } = new();

    public TesseraThemeSurfaceTokens Surface { get; init; } = new();

    public TesseraThemeBorderTokens Border { get; init; } = new();

    public TesseraThemeStateTokens State { get; init; } = new();

    public TesseraThemeAccentTokens Accent { get; init; } = new();

    public TesseraThemeSelectionTokens Selection { get; init; } = new();

    public TesseraThemeFocusTokens Focus { get; init; } = new();
}
